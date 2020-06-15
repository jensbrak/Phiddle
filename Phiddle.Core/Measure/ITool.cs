using Phiddle.Core.Graphics;
using SkiaSharp;
using System.Collections.Generic;

namespace Phiddle.Core.Measure
{
    public interface ITool : IDrawable
    {
        ToolId ToolId { get; }

        void Move(SKPoint p);
        void Resize(SKPoint p);
        void NextAction(SKPoint p);
    }
}