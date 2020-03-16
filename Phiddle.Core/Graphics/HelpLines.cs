using SkiaSharp;

namespace Phiddle.Core.Graphics
{
    public class HelpLines : IDrawable
    {
        public SKPoint Pos { get; set; }
        public SKRect Bounds { get; set; }
        public bool Visible { get; set; } = Defaults.HelpLinesVisible;
        public HelpLines(SKRect bounds)
        {
            Bounds = bounds;
        }

        public void Draw(SKCanvas c)
        {
            if (!Visible)
            {
                return;
            }

            c.DrawLine(0f, Pos.Y, Bounds.Right, Pos.Y, Defaults.HelpLinesPaint);
            c.DrawLine(Pos.X, 0, Pos.X, Bounds.Bottom, Defaults.HelpLinesPaint);
        }

    }
}
