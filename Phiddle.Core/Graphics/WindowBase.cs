using SkiaSharp;

namespace Phiddle.Core.Graphics
{
    public class WindowBase : IDrawable
    {
        public SKPaint PaintBorder { get; set; }
        public SKPaint PaintBackground { get; set; }
        public SKRect Bounds { get; set; }
        public bool Transparent { get; set; }

        public bool Visible { get; set; }

        public WindowBase(SKRect bounds)
        {
            bounds.Left += 1;
            bounds.Top += 1;
            Bounds = bounds;
            Transparent = true;
            PaintBorder = Defaults.WindowBasePaintBorder;
            PaintBackground = Defaults.WindowBasePaintBackground;
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
