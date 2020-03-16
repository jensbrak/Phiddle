using SkiaSharp;
using Phiddle.Core.Extensions;

namespace Phiddle.Core.Graphics
{
    public class Label : WindowBase
    {
        private string text;
        private SKPoint textOffset;

        public float TextPad { get; set; }

        public SKPaint PaintText { get; set; }

        public SKPoint Pos 
        {
            get
            {
                return new SKPoint(Bounds.Left, Bounds.Top);
            }
            set
            {
                Bounds = new SKRect(value.X, value.Y, value.X + Bounds.Width, value.Y + Bounds.Height);
            } 
        }

        public string Text 
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                var textBounds = PaintText.GetTextBounds(value);
                var left = Bounds.Left;
                var top = Bounds.Top;
                var width = textBounds.Width + TextPad * 2;
                var height = textBounds.Height + TextPad * 2;
                Bounds = new SKRect(left, top, left + width, top + height);
                textOffset = new SKPoint(TextPad - textBounds.Left, TextPad - textBounds.Top);
            }
        }

        public Label(SKPoint pos, string text) : base(new SKRect())
        {
            PaintBackground = Defaults.LabelPaintBackgroud;
            PaintText = Defaults.LabelPaintText;
            TextPad = Defaults.LabelTextPad;
            Transparent = false;
            Text = text;
            Pos = pos;
        }
        public override void Draw(SKCanvas c)
        {
            base.Draw(c);
            c.DrawText(Text, Pos + textOffset, PaintText);
        }
    }
}
