using Phiddle.Core.Graphics;
using SkiaSharp;
using System.Collections.Generic;

namespace Phiddle.Core.Measure
{
    public interface ITool : IDrawable, IPosition
    {
        ToolId ToolId { get; }
        ToolState State { get; set; }
        Dictionary<Measurement, float> Measurements { get; set; }

        bool IsMeasuring();
        bool IsMoving();
        bool AnyPointFocused();

        void Move(SKPoint p);
        void Measure(SKPoint p);
        void NextAction(SKPoint p);
    }
}