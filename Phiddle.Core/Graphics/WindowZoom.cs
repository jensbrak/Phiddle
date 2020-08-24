using SkiaSharp;
using Phiddle.Core.Settings;

namespace Phiddle.Core.Graphics
{
    public class WindowZoom : Window
    {
        private SKImage screenshot;
        private float zoomFactor;

        public bool CrosshairVisible { get; set; }
        public float CrosshairSize { get; set; }
        public SKPaint PaintCrosshair { get; set; }

        public WindowZoom(SKPoint pos, SKSize size, SettingsWindowZoom settings) : base(pos, size, settings)
        {
            PaintBorder = settings.PaintBorder.ToSKPaint();
            PaintBackground = settings.PaintBackground.ToSKPaint();
            PaintCrosshair = settings.PaintCrosshair.ToSKPaint();
            CrosshairSize = settings.CrossHairSize;
            CrosshairVisible = true;
            Enabled = true;
            Transparent = false;
        }

        public void UpdateZoom(SKImage screenshot, float zoomFactor)
        {
            this.screenshot = screenshot;
            this.zoomFactor = zoomFactor;
        }

        public override void Draw(SKCanvas c)
        {
            if (!Enabled)
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