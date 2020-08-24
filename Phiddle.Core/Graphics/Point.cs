using System;
using Phiddle.Core.Settings;
using Phiddle.Core.Extensions;
using SkiaSharp;

namespace Phiddle.Core.Graphics
{

    public class Point : Shape
    {
        public float Diameter
        {
            get => Size.Width;
        }
        public Point(SKPoint pos, float size, SKPaint paint) : base(pos, new SKSize(size, size), paint)
        {
            // Intentionally empty
        }

        public override void Draw(SKCanvas c)
        {
            if (!Enabled)
            {
                // Nothing to draw
                return;
            }

            var x = Pos.X - Diameter / 2;
            var y = Pos.Y - Diameter / 2;
            var w = Diameter;
            var h = Diameter;
            var m = 4f; // Margin of arrow
            var al = Diameter / 3f; // Length of arrow 
            var p = PaintBounds;

            if (Focused)
            {
                p.Style = SKPaintStyle.StrokeAndFill;
            }
            else
            {
                p.Style = SKPaintStyle.Stroke;
            }

            // Draw the grip itself
            c.Save();
            var r = new SKRect(x, y, x + w, y + h);

            c.RotateDegrees(45f, Pos.X, Pos.Y);
            c.DrawRect(r, p);

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

        public override void OnMove()
        {
            throw new NotImplementedException();
        }

        public override void OnResize()
        {
            throw new NotImplementedException();
        }
    }
}
