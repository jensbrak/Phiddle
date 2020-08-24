using SkiaSharp;
using Phiddle.Core.Services;
using Phiddle.Core.Settings;

namespace Phiddle.Core.Graphics
{
    public class HelpLines : IDrawable, IPosition
    {
        private readonly SKPaint paint;

        public SKPoint Pos   { get; set; }
        public SKRect Bounds { get; set; }
        public bool Enabled { get; set; }
        public HelpLines(SKRect bounds, AppState appState, SettingsPaint settings)
        {
            Enabled = appState.HelpLinesVisible;
            paint = settings.ToSKPaint();
            Bounds = bounds;
        }

        public void Draw(SKCanvas c)
        {
            if (!Enabled)
            {
                return;
            }

            c.DrawLine(0f, Pos.Y, Bounds.Right, Pos.Y, paint);
            c.DrawLine(Pos.X, 0, Pos.X, Bounds.Bottom, paint);
        }

        public void Refresh(SKPoint p)
        {
            Pos = p;
        }
    }
}
