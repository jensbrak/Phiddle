using Phiddle.Core.Extensions;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace Phiddle.Core.Measure
{
    public class ToolOval : ToolRect
    {
        protected override MarkBase[] DefaultMarks()
        {
            return new MarkBase[]
            {
                new MarkGoldenRatio(),
                new MarkThird(),
                new MarkMiddle(),
                new MarkEndpoint() { Visible = false},
            };
        }

        public ToolOval() : base()
        {
            marks = DefaultMarks();
        }

        public override Dictionary<Measurement, float> GetMeasurements()
        {
            if (!Visible)
            {
                return base.GetMeasurements();
            }

            var v = p1.Pos - p0.Pos;
            var w = Math.Abs(v.X) / 2f;
            var h = Math.Abs(v.Y) / 2f;
            var a = w / 2f;
            var b = h / 2f;
            // Ramanujan approximation of circumference
            var c = (float)Math.PI * (3f * (a + b) - (float)Math.Sqrt((3f * a + b) * (a + 3f * b)));
            var area = a * b * (float)Math.PI;

            var measurements = new Dictionary<Measurement, float>()
            {
                { Measurement.Width, w },
                { Measurement.Height, h },
                { Measurement.Area, area },
                { Measurement.Circumference, c },
            };

            return measurements;
        }
        protected override SKPoint GetLockedPos(SKPoint p)
        {
            // Get positions on the X- Y-axis obtained by projecting endpoint of 
            // the vector from p0 to p. The resulting a is the point on the closest axis
            // (ie short side) and an (long side) is it's normal, ie the point on the other axis.
            (SKPoint a, SKPoint an) = p0.Pos.ProjectXY(p);

            //Get the locked point
            var pos = p0.Pos + a + an;

            return pos;
        }

        protected override void DrawMarks(SKCanvas c)
        {
            // Get the rect vectors
            var pTR = new SKPoint(p1.X, p0.Y);
            var pBL = new SKPoint(p0.X, p1.Y);
            var vHorizontal = new SKPoint(p1.X - p0.X, 0);
            var vVertical = new SKPoint(0, p1.Y - p0.Y);

            // Clip path as defined by the oval
            var clipBounds = new SKRect(
                frame.Bounds.Left + 3,
                frame.Bounds.Top + 3,
                frame.Bounds.Right - 3,
                frame.Bounds.Bottom - 3);
            var clipPath = new SKPath();
            clipPath.AddOval(clipBounds);

            // Draw all visible marks
            foreach (var mark in marks)
            {
                if (!mark.Visible)
                {
                    continue;
                }

                // Clip all marks but Endpoints (they're the bounding box)
                if (mark.Category != MarkCategory.Endpoint)
                {
                    c.Save();
                    c.ClipPath(clipPath);
                }

                foreach (var m in mark.Pos)
                {
                    // Positions relative the the sides
                    var v0 = p0.Pos + vHorizontal.Scale(m);
                    var v1 = pBL + vHorizontal.Scale(m);
                    var h0 = p0.Pos + vVertical.Scale(m);
                    var h1 = pTR + vVertical.Scale(m);

                    // Draw them
                    c.DrawLine(v0, v1, mark.PaintMark);
                    c.DrawLine(h0, h1, mark.PaintMark);
                }

                if (mark.Category != MarkCategory.Endpoint)
                {
                    c.Restore();
                }
            }
        }

        public override void Draw(SKCanvas c)
        {
            if (!Visible || p0 == p1)
            {
                return;
            }

            // Draw marks
            DrawMarks(c);

            // Draw oval
            var rx = (p1.X - p0.X) / 2f;
            var ry = (p1.Y - p0.Y) / 2f;
            var cx = p0.X + rx;
            var cy = p0.Y + ry;
            c.DrawOval(cx, cy, rx, ry, PaintTool);

            // Draw basics
            DrawBase(c);
        }

        public override string ToString()
        {
            return "Oval";
        }
    }
}
