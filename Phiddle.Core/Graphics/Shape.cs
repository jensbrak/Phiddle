using System;
using Phiddle.Core.Settings;
using Phiddle.Core.Extensions;
using SkiaSharp;

namespace Phiddle.Core.Graphics
{

    public abstract class Shape : IDragable, IDrawable
    {
        // IMovable properties
        public bool Focused { get; set; }
        public bool Selected { get; set; }
        public SKRect Bounds
        {
            get
            {
                var b = Size.CombineWith(Pos);
                b.Inflate(Pad);
                return b;
            }
        }
        public SKSize Pad { get; set; }

        // IDrawable properties
        public bool Enabled { get; set; }

        // Geometry properties
        public virtual SKPoint Pos { get; set; }
        public virtual SKSize Size { get; set; }

        // Visual style
        public SKPaint PaintBounds { get; set; }

        public Shape(SKPoint pos, SKSize size, SKPaint paintBounds)
        {
            Enabled = false;
            Pos = pos;
            Size = size;
            PaintBounds = paintBounds;
        }

        public virtual void Draw(SKCanvas c)
        {
            // Only draw bounds if focused
            if (!Focused)
            {
                return;
            }

            c.DrawRect(Bounds, PaintBounds);
        }

        // Position changed
        public abstract void OnMove();

        // Size changed
        public abstract void OnResize();

        // Mouse moved
        public virtual void OnMouseMove(SKPoint p)
        {
            Focused = Enabled && Bounds.Inside(p);
        }
    }
}
