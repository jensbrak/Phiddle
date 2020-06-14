using Phiddle.Core.Graphics;
using SkiaSharp;
using System.Collections.Generic;

namespace Phiddle.Core.Measure
{
    public interface ITool : IDrawable
    {
        Endpoint ActiveEndpoint { get; }
        Label Label { get; set; }
        LabelLocation LabelLocation { get; set; }

        bool Visible { get; set; }
        bool Locked { get; set; }
        bool Movable { get; set; }
        bool Moving { get; set; }
        bool Resizable { get; set; }
        bool Resizing { get; set;}

        void Move(SKPoint p);
        void Resize(SKPoint p);
        void CheckBounds(SKPoint p);
        void NextAction(SKPoint p);
        void ToggleMark(MarkId c);
        Dictionary<Measurement, float> Measure();
    }
}