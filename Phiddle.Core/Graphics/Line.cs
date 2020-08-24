using System;
using Phiddle.Core.Extensions;
using SkiaSharp;

namespace Phiddle.Core.Graphics
{
    public enum LineStyle
    {
        Normal,
        Extended,
    }

    public class Line : Shape
    {
        // Internal properties
        private SKPoint _p0;
        private SKPoint _p1;
        private SKPoint _ep0;
        private SKPoint _ep1;
        private float _m;
        private float _b;
        private SKRectI _extendedBounds;
        private LineStyle _lineStyle;

        // Geometry properties
        public SKPoint P0
        {
            get => _p0;
            set
            {
                _p0 = value;
                UpdateLine();
            }
        }
        public SKPoint P1
        {
            get => _p1;
            set
            {
                _p1 = value;
                UpdateLine();
            }
        }
        public float Length => (P0 - P1).Length;

        // Visual style
        public LineStyle LineStyle
        {
            get => _lineStyle;
            set
            {
                _lineStyle = value;
                UpdateLine();
            }
        }

        public SKPaint PaintLine { get; set; }

        public Line(SKPoint p0, SKPoint p1, SKPaint paintLine, SKRectI extendedBounds, SKPaint paintBounds, LineStyle lineStyle) : base(p0, new SKSize(p1-p0), paintBounds)
        {
            _p0 = p0;
            _p1 = p1;
            _lineStyle = lineStyle;
            _extendedBounds = extendedBounds;

            PaintLine = paintLine;
            UpdateLine();
        }

        private void UpdateLine()
        {
            if (LineStyle != LineStyle.Extended || P0 == P1)
            {
                // Line won't extend or there's no line, nothing to update
                return;
            }

            // Slope and y-intercept
            _m = (P1.Y - P0.Y) / (P1.X - P0.X);
            _b = P0.Y - _m * P0.X;

            // Extended bounds
            var b0x = -_b / _m; 
            var b0y = _b;
            var b1x = (_extendedBounds.Height - _b) / _m;
            var b1y = _m * _extendedBounds.Width + _b;

            // What bounds do the line cross?
            _ep0 = new SKPoint(b0x > 0f ? 0f : b0x, b0y > 0f ? 0f : b0y);
            _ep1 = new SKPoint(b1x < _extendedBounds.Width ? _extendedBounds.Width : b1x, b1y < _extendedBounds.Height ? _extendedBounds.Height : b1y);
        }

        public override void Draw(SKCanvas c)
        {
            // d504
            base.Draw(c);

            // Nothing to draw?
            if (!Enabled || P0 == P1)
            {
                return;
            }

            // Style of line?
            if (LineStyle == LineStyle.Normal)
            {
                // Normal line, ie endpoints are the line endpoints
                c.DrawLine(P0, P1, PaintLine);
            }
            else
            {
                // Extended line, ie endpoints are the extended endpoints as limited by _extendedBounds
                c.DrawLine(_ep0, _ep1, PaintLine);
            }
        }

        public override void OnMove()
        {
            UpdateLine();
        }

        public override void OnResize()
        {
            UpdateLine();
        }
    }
}
