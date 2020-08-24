using SkiaSharp;
using Phiddle.Core.Extensions;
using Phiddle.Core.Settings;

namespace Phiddle.Core.Graphics
{
    public class Label : Window
    {
        private string text;
        private SKPoint textOffset;

        public float TextPad { get; set; }

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
                var width = textBounds.Width + TextPad * 2;
                var height = textBounds.Height + TextPad * 2;
                Size = new SKSize(width, height);
                textOffset = new SKPoint(TextPad - textBounds.Left, TextPad - textBounds.Top);
            }
        }

        public Label(SKPoint pos, string text, SettingsWindowLabel settings) : base(pos, SKSize.Empty, settings) 
        {
  //          PaintBackground = settings.PaintBackground.ToSKPaint();
  //          PaintText = settings.PaintText.ToSKPaint();
            TextPad = settings.TextPad;
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
