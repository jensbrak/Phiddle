using SkiaSharp;

namespace Phiddle.Core.Graphics
{
    public class WindowZoom : WindowBase
    {
        private SKImage screenshot;
        private float zoomFactor;

        public bool CrosshairVisible { get; set; }
        public float CrosshairSize { get; set; } = Defaults.WindowZoomCrosshairSize;
        public SKPaint PaintCrosshair { get; set; } = Defaults.WindowZoomPaintCrosshair;

        public WindowZoom(SKRect bounds) : base(bounds)
        {
            Visible = Defaults.WindowZoomVisible;
            CrosshairVisible = Defaults.WindowZoomCrosshairVisible;
            Transparent = false;
            PaintBorder = Defaults.WindowBasePaintBorder;
            PaintBackground = Defaults.WindowBasePaintBackground;
        }

        public void UpdateZoom(SKImage screenshot, float zoomFactor)
        {
            this.screenshot = screenshot;
            this.zoomFactor = zoomFactor;
        }

        public override void Draw(SKCanvas c)
        {
            if (!Visible)
            {
                return;
            }


            if (screenshot != null)
            {
                // Save view matrix, scale the screenshot to make it zoomed in and then restore
                c.Save();
                c.Scale(zoomFactor);

                c.DrawImage(screenshot, Bounds.Left - 1, Bounds.Top - 1);
                c.Restore();

                // Crosshair
                if (CrosshairVisible)
                {
                    var xs = CrosshairSize / 2;
                    c.DrawLine(Bounds.MidX - xs, Bounds.MidY, Bounds.MidX + xs, Bounds.MidY, PaintCrosshair);
                    c.DrawLine(Bounds.MidX, Bounds.MidY - xs, Bounds.MidX, Bounds.MidY + xs, PaintCrosshair);
                }
            }
            base.Draw(c);
        }
    }
}