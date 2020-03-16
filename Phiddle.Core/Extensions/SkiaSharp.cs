using System;
using SkiaSharp;

namespace Phiddle.Core.Extensions
{
    /// <summary>
    /// Extensions to SkiaSharp classes
    /// </summary>
    public static class SkiaSharp
    {
        private static readonly float radians = (float)(Math.PI / 180d);

        public static SKPoint DistanceTo(this SKPoint p0, SKPoint p1)
        {
            return new SKPoint(Math.Abs(p0.X - p1.X), Math.Abs(p0.Y - p1.Y));
        }

        public static SKPoint Normalize(this SKPoint v)
        {
            var d = v.Length;
            var vn = new SKPoint(v.X / d, v.Y / d);
            return vn;
        }

        public static SKPoint Rotate(this SKPoint v, int steps)
        {
            // Rotate fixed steps - along Y and X axis. 
            // Each step is 90 degrees and step 0 equals no rotation
            // Steps over 3 (ie one full rotation or more) allowed using modulo 4.
            SKPoint vRotated;
            switch (steps % 4)
            {
                case 1:
                    vRotated = new SKPoint(-v.Y, v.X);
                    break;
                case 2:
                    vRotated = new SKPoint(-v.X, -v.Y);
                    break;
                case 3:
                    vRotated = new SKPoint(v.Y, -v.X);
                    break;
                default:
                    vRotated = v;
                    break;
            }

            return vRotated;
        }

        public static SKPoint Rotate(this SKPoint v, float d)
        {
            var r = radians * d;
            var sinD = Math.Sin(r);
            var cosD = Math.Cos(r);

            var x = (float)(v.X * cosD - v.Y * sinD);
            var y = (float)(v.X * sinD + v.Y * cosD);

            var vRotated = new SKPoint(x, y);
            return vRotated;
        }

        public static SKPoint RotateAt(this SKPoint p, SKPoint pivot, float d)
        {
            // Translate to pivot, rotate at point p and then translate back
            return (p - pivot).Rotate(d) + pivot;
        }

        public static SKPoint Scale(this SKPoint v, float s)
        {
            var v1 = new SKPoint(v.X * s, v.Y * s);
            return v1;
        }

        public static (SKPoint, SKPoint) ProjectXY(this SKPoint p0, SKPoint p)
        {
            // Make a direction vector and a filter to get the nearest axis
            var v = p - p0;
            var xs = Math.Abs(v.X) < Math.Abs(v.Y) ? 1 : 0;
            var ys = 1 - xs;

            // a is the short side of the rect
            var a = new SKPoint(xs * v.X, ys * v.Y);
            // an is the long side of the rect but scaled down to the lengths of a
            var an = new SKPoint(Math.Sign(ys * v.X), Math.Sign(xs * v.Y)).Scale(a.Length);

            // The point on closest axis is a, the point on the other axis is an
            var points = (closest: a, Normalize: an);
            return points;
        }

        public static SKRect GetTextBounds(this SKPaint paint, string text)
        {
            SKRect bounds = new SKRect();
            paint.MeasureText(text, ref bounds);
            SKRect extra = new SKRect();
            paint.MeasureText("jqy", ref extra);
            return new SKRect(bounds.Left, bounds.Top, bounds.Right, Math.Max(extra.Bottom, bounds.Bottom));
        }

        public static bool Inside(this SKRect r, SKPoint p)
        {
            var top = Math.Min(r.Top, r.Bottom);
            var left = Math.Min(r.Left, r.Right);
            var bottom = Math.Max(r.Top, r.Bottom);
            var right = Math.Max(r.Left, r.Right);

            return top <= p.Y && left <= p.X && bottom >= p.Y && right >= p.X;
        }


        public static SKRect CombineWith(this SKSize s, SKPoint p)
        {
            return new SKRect(p.X, p.Y, p.X + s.Width, p.Y + s.Height);
        }

        public static void Debug(this SKRect r)
        {
            Console.WriteLine($">>> L={r.Left:0.000000}, T={r.Top:0.000000}, R={r.Right:0.000000}, B={r.Bottom:0.000000}");
        }
    }
}
