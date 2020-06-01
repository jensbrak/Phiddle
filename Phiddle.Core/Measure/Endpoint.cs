using Phiddle.Core.Graphics;
using Phiddle.Core.Services;
using SkiaSharp;
using Phiddle.Core.Settings;

namespace Phiddle.Core.Measure
{
    public class Endpoint : IDrawable
    {
        public IScreenService ScreenService { get; set; }
        public float X 
        { 
            get
            {
                return Pos.X;
            }
        }
        public float Y
        {
            get
            {
                return Pos.Y;
            }
        }
        public float Size { get; set; }
        public SKPaint PaintGrip { get; set; }
        public bool Visible { get; set; }
        public bool Focused { get; set; }
        public SKPoint Pos { get; set; }
        public Endpoint(SKPoint p, SettingsTool settings)
        {
            Visible = false;
            Pos = p;
            Size = settings.SizeEndpoint;
            PaintGrip = settings.PaintEndpoint.ToSKPaint(); 
        }
        public void CheckBounds(SKPoint p)
        {
            Focused = Visible && (Pos - p).Length <= Size;
        }

        public void Move(SKPoint p)
        {
            if (Pos == p)
            {
                return;
            }
            Pos = p;
        }

        public void Draw(SKCanvas c)
        {
            if (!Visible)
            {
                // Nothing to draw
                return;
            }

            var x = Pos.X - Size / 2;
            var y = Pos.Y - Size / 2;
            var w = Size;
            var h = Size;
            var m = 4f; // Margin of arrow
            var al = Size / 3f; // Length of arrow 
            var p = PaintGrip;

            if (Focused)
            {
                p.Style = SKPaintStyle.StrokeAndFill; 
            }
            else
            {
                p.Style = SKPaintStyle.Stroke;
            }

            // Draw standing square
            c.Save();
            c.RotateDegrees(45f, Pos.X, Pos.Y);            
            c.DrawRect(x, y, w, h, p);

            // Draw small arrows at eatch corner
            for (int i = 0; i < 4; i++)
            {
                c.Save();
                c.Translate(-m, -m);
                c.DrawLine(x, y, x + al, y, p);
                c.RotateDegrees(90f, x, y);
                c.DrawLine(x, y, x + al, y, p);
                c.Restore();
                c.RotateDegrees(90f, Pos.X, Pos.Y);
            }

            c.Restore();
        }
    }
}
