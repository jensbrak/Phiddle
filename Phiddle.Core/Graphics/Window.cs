using SkiaSharp;
using Phiddle.Core.Extensions;
using Phiddle.Core.Settings;

namespace Phiddle.Core.Graphics
{
    public class Window : IDrawable
    {
        public SKPaint PaintBorder { get; set; }
        public SKPaint PaintBackground { get; set; }
        public SKPaint PaintText { get; set; }
        public SKRect Bounds { get { return Size.CombineWith(Pos); } }
        public SKPoint Pos { get; set; }
        public SKSize Size { get; set; }
        public bool Transparent { get; set; }

        public bool Enabled { get; set; }

        public Window(SKPoint pos, SKSize size, SettingsWindow settings)
        {
            var compensate = settings.PaintBorder.StrokeWidth;
            pos.Offset(compensate, compensate);
            size -= new SKSize(compensate, compensate);

            // Settings based properties
            PaintBorder = settings.PaintBorder.ToSKPaint();
            PaintBackground = settings.PaintBackground.ToSKPaint();
            PaintText = settings.PaintText.ToSKPaint();
            Transparent = settings.Transparent;

            // Parameter based properties
            Pos = pos;
            Size = size;
        }

        public virtual void Draw(SKCanvas c)
        {
            if (!Transparent)
            {
                c.DrawRect(Bounds, PaintBackground);
            }
            c.DrawRect(Bounds, PaintBorder);
        }
    }
}
