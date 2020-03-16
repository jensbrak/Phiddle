using SkiaSharp;
using System;
using Phiddle.Core.Extensions;
using Phiddle.Core.Graphics;
using System.Collections.Generic;

namespace Phiddle.Core.Measure
{
    public class ToolRect : ToolBase
    {
        protected override MarkBase[] DefaultMarks()
        {
            return new MarkBase[]
            {
                new MarkGoldenRatio(),
                new MarkMiddle(),
                new MarkThird(),
            };
        }

        public ToolRect() : base()
        {
            // Important to set position and text after constructor call
#pragma warning disable IDE0017 // Simplify object initialization
            Label = new Label(new SKPoint(), string.Empty);
#pragma warning restore IDE0017 // Simplify object initialization
            Label.Text = GetLabelText();
            Label.Pos = GetLabelPos();

            // Rectangle marks
            marks = DefaultMarks();

            // Show all endpoints
            p0.Visible = true;
            p1.Visible = true;
            p2.Visible = true;
            p3.Visible = true;
        }
        public override void Draw(SKCanvas c)
        {
            if (!Visible || p0 == p1)
            {
                return;
            }

            // Draw marks
            DrawMarks(c);

            // Draw rect
            c.DrawRect(p0.X, p0.Y, p1.X - p0.X, p1.Y - p0.Y, PaintTool);

            // Draw basics
            DrawBase(c);
        }
        public override Dictionary<Measurement, float> GetMeasurements()
        {
            if (!Visible)
            {
                return base.GetMeasurements();
            }

            var v = p1.Pos - p0.Pos;
            var w = Math.Abs(v.X);
            var h = Math.Abs(v.Y);
            var c = 2 * (w + h);
            var a = w * h;

            var measurements = new Dictionary<Measurement, float>()
            {
                { Measurement.Width, w },
                { Measurement.Height, h },
                { Measurement.Area, a },
                { Measurement.Circumference, c },
            };

            return measurements;
        }
        protected override string GetLabelText()
        {
            var m = GetMeasurements();

            if (m.Count == 0)
            {
                return string.Empty;
            }
            else
            {
                return $"W x H = {m[Measurement.Width]:0} x {m[Measurement.Height]:0}, C = {m[Measurement.Circumference]:0}, A = {m[Measurement.Area]:0}";
            }
        }

        protected override SKPoint GetLabelPos()
        {
            // Label as requested by property
            if (LabelLocation == LabelLocation.AboveMouse)
            {
                // Above cursor means moving it up by its height and relative end point
                var offset = new SKPoint(0, -Label.Bounds.Height);
                return p1.Pos + offset;
            }
            else
            {
                // v is direction vector from P0 to P1
                var v = p1.Pos - p0.Pos;

                // Center rect means center of rect adjusted to center of label
                var offset = new SKPoint(Label.Bounds.Width / 2, Label.Bounds.Height / 2);
                var rectCenter = p1.Pos - v.Scale(0.5f); 
                return rectCenter - offset;
            }
        }

        protected override SKPoint GetLockedPos(SKPoint p)
        {
            // Get positions on the X- Y-axis obtained by projecting endpoint of 
            // the vector from p0 to p. The resulting a is the point on the closest axis
            // (ie short side) and an (long side) is it's normal, ie the point on the other axis.
            (SKPoint a, SKPoint an) = p0.Pos.ProjectXY(p);

            // b is what's missing to get Golden Ratio
            SKPoint b = an.Scale(Constants.PhiInv);

            // Finally get the new point 
            var pos = p0.Pos + a + an + b;

            return pos;
        }

        protected override void DrawMarks(SKCanvas c)
        {
            // Get the rect vectors
            var pTR = new SKPoint(p1.X, p0.Y);
            var pBL = new SKPoint(p0.X, p1.Y);
            var vHorizontal = new SKPoint(p1.X - p0.X, 0);
            var vVertical = new SKPoint(0, p1.Y - p0.Y);

            // Draw all visible marks
            foreach (var mark in marks)
            {
                if (!mark.Visible)
                {
                    continue;
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
            }
        }
         public override string ToString()
        {
            return "Rectangle";
        }
    }
}
