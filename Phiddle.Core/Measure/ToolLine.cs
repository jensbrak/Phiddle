using SkiaSharp;
using System;
using Phiddle.Core.Extensions;
using Phiddle.Core.Graphics;
using System.Collections.Generic;
using Phiddle.Core.Settings;

namespace Phiddle.Core.Measure
{
    public class ToolLine : Tool
    {
        public ToolLine(SettingsTool settingsTool) : base(settingsTool)
        {
            ToolId = ToolId.Line;
            EnableMarks(settingsTool, MarkId.Endpoint | MarkId.Middle | MarkId.Third | MarkId.Phi);

            // Important to set position and text after constructor call
            Label = new Label(new SKPoint(), string.Empty, settingsTool.Label);
            Label.Text = LabelText();
            Label.Pos = LabelPos();

            // Show two endpoints
            p0.Enabled = true;
            p1.Enabled = true;
        }

        public override Dictionary<Measurement, float> Measure()
        {
            if (!Visible)
            {
                return base.Measure();
            }

            var measurements = new Dictionary<Measurement, float>(1)
            {
                { Measurement.Length, (p0.Pos - p1.Pos).Length },
            };

            return measurements;
        }
        protected override string LabelText()
        {
            return $"L = {(p0.Pos - p1.Pos).Length:0.00}";
        }

        protected override SKPoint LabelPos()
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
                var v = p0.Pos - p1.Pos;

                // Center line means center of line adjusted to center of label
                var offset = new SKPoint(-Label.Bounds.Width / 2, -Label.Bounds.Height / 2);
                var vCenter = p0.Pos - v.Scale(0.5f);
                return vCenter + offset;
            }
        }

        protected override SKPoint LockedPos(SKPoint p)
        {
            // pP is the passive endpoint, ie is diagonal to the endpoint being moved
            var pA = DiagonalToActiveEndpoint.Pos;

            // Get line as a vector relative to new position p
            var v = p - pA;
            // Shortest axis is closest axis to lock on to, 1 is use it, 0 if not
            var dx = Math.Abs(v.X) < Math.Abs(v.Y) ? 1f : 0f;
            var dy = 1f - dx;
            // Use the closest axis
            var dPos = new SKPoint(dx * v.X, dy * v.Y);
            // Change new pos with respect to chosen axis
            var pos = p - dPos;
            return pos;
        }

        protected override void DrawTool(SKCanvas c)
        {
            // Draw the line
            c.DrawLine(p0.Pos, p1.Pos, PaintTool);
        }

        protected override void DrawMarks(SKCanvas c)
        {
            // Get the direction vector perpendicular to the line
            var v = p1.Pos - p0.Pos;
            var n = v.Rotate(1).Normalize();

            // Draw all marks
            foreach (var mark in marks)
            {
                if (!mark.Visible)
                {
                    continue;
                }

                foreach (var m in mark.Pos)
                {
                    // Releative first endpoint and scaled towards second one
                    var pos = p0.Pos + v.Scale(m);

                    // Centered across the line
                    var m0 = n.Scale(mark.Size / 2);
                    var m1 = m0.Rotate(2); // 180 degrees

                    // Draw it
                    c.DrawLine(pos + m0, pos + m1, mark.PaintMark);
                }
            }
        }
    }
}
