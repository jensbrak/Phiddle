using System;
using SkiaSharp;

namespace Phiddle.Core.Graphics
{
    public interface IDragable
    {
        SKRect Bounds { get; }
        SKSize Pad { get; set; }

        bool Focused { get; set; }
        bool Selected { get; set; }

        void OnMouseMove(SKPoint p);
    }
}
