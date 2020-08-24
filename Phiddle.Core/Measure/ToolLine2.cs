using System;
using System.Collections.Generic;
using Phiddle.Core.Graphics;
using Phiddle.Core.Settings;
using SkiaSharp;

namespace Phiddle.Core.Measure
{
    public class ToolLine2 : MultiTool
    {
        public Line Line { get; set; }
        public override ToolId ToolId => ToolId.Line2;

        public ToolLine2(SettingsTool settings, SKRectI outerBounds) : base(settings)
        {
            Points = new List<Point>()
            {
                new Point(SKPoint.Empty, settings.SizeEndpoint, settings.PaintEndpoint.ToSKPaint()),
                new Point(SKPoint.Empty, settings.SizeEndpoint, settings.PaintEndpoint.ToSKPaint()),
            };
            Line = new Line(SKPoint.Empty, SKPoint.Empty, settings.PaintTool.ToSKPaint(), outerBounds, settings.SettingsToolFrame.PaintBorder.ToSKPaint(), LineStyle.Normal);
            Measurements = new Dictionary<Measurement, float>(1)
            {
                { Measurement.Length, 0.0f }
            };
        }

        public override void Draw(SKCanvas c)
        {
            Line.Draw(c);
            Points.ForEach(p => p.Draw(c));
        }

        public override void OnMouseMove(SKPoint p)
        {
            foreach (var point in Points)
            {
                point.OnMouseMove(p);
            }
            Line.OnMouseMove(p);
            Focused = Line.Focused;
        }

        protected override void UpdateMeasurements()
        {
            if (IsMeasuring())
            {
                Measurements[Measurement.Length] = Line.Length;
            }
        }
    }
}
