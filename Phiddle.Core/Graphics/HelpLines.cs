using SkiaSharp;
using Phiddle.Core.Services;
using Phiddle.Core.Settings;
using Phiddle.Core.Settings;

namespace Phiddle.Core.Graphics
{
    public class HelpLines : IDrawable
    {
        private readonly SKPaint paint;

        public SKPoint Pos { get; set; }
        public SKRect Bounds { get; set; }
        public bool Visible { get; set; }
        public HelpLines(SKRect bounds, AppState appState, SettingsPaint settings)
        {
            Visible = appState.HelpLinesVisible;
            paint = settings.ToSKPaint();
            Bounds = bounds;
        }

        public void Draw(SKCanvas c)
        {
            if (!Visible)
            {
                return;
            }

            c.DrawLine(0f, Pos.Y, Bounds.Right, Pos.Y, paint);
            c.DrawLine(Pos.X, 0, Pos.X, Bounds.Bottom, paint);
        }

    }
}
