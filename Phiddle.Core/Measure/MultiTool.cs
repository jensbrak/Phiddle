using System;
using System.Collections.Generic;
using System.Linq;
using Phiddle.Core.Extensions;
using Phiddle.Core.Graphics;
using Phiddle.Core.Settings;
using SkiaSharp;

namespace Phiddle.Core.Measure
{
    public abstract class MultiTool : IDragable, ITool
    {
        protected ToolState _state;
        protected SKPoint _prevPos;

        public virtual ToolId ToolId => throw new NotImplementedException("ToolId");
        public bool Enabled { get => State != ToolState.Hidden; }
        public SKPaint PaintTool { get; set; }

        public ToolState State
        {
            get => _state;
            set
            {
                _state = value;
                // Update shapes
            }
        }

        public Dictionary<Measurement, float> Measurements { get; set; }

        public List<Point> Points { get; set; }
        public bool Focused { get; set; }
        public bool Selected { get; set; }

        public SKRect Bounds => throw new NotImplementedException();

        public SKSize Pad { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public MultiTool(SettingsTool settings)
        {
            _prevPos = SKPoint.Empty;

            State = ToolState.Hidden;

        }

        protected abstract void UpdateMeasurements();

        public abstract void Draw(SKCanvas c);

        public abstract void OnMouseMove(SKPoint p);


        public bool IsMeasuring()
        {
            return State == ToolState.MeasuringNext
                || State == ToolState.MeasuringStart;
        }

        public bool IsMoving()
        {
            return State == ToolState.Moving;
        }

        public void Move(SKPoint p)
        {
            foreach (var grip in Points)
            {
                grip.Pos += p;
            }
        }

        public void NextAction(SKPoint p)
        {
            // Default behaviour of a tool
            switch (State)
            {
                // Hidden -> Measuring Started
                case ToolState.Hidden:
                    State = ToolState.MeasuringStart;
                    break;
                // Measuring Started -> Passive (stop measuring)
                case ToolState.MeasuringStart:
                    State = ToolState.Passive;
                    break;
                // Passive -> <state depends on any focused part of the tool>
                case ToolState.Passive:
                    if (AnyPointFocused())
                    {
                        State = ToolState.MeasuringStart;

                        foreach (var point in Points)
                        {
                            if (point.Focused)
                            {
                                point.Selected = true;
                            }
                        }
                    }
                    else if (Focused)
                    {
                        State = ToolState.Moving;
                    }
                    else
                    {
                        State = ToolState.Hidden;
                    }
                    break;
                case ToolState.Moving:
                    State = ToolState.Passive;
                    break;
                default:
                    throw new InvalidOperationException($"Tool {ToolId} does not support this state");


            }
        }

        public bool AnyPointFocused()
        {
            return Points.FirstOrDefault(p => p.Focused) != null;
        }

        public Point SelectedPoint()
        {
            return Points.FirstOrDefault(p => p.Selected);
        }

        public void Measure(SKPoint p)
        {
            var point = SelectedPoint();

            if (point == null)
            {
                return;
            }

            point.Pos = p;
            UpdateMeasurements();
        }

        public void Refresh(SKPoint p)
        {
            if (IsMeasuring())
            {
                // Update tool with new position of mouse
                Measure(p);
            }
            else if (IsMoving())
            {
                // Update tool position with amount of movement since last time
                var delta = p - _prevPos;
                Move(delta);
            }
            else
            {
                // Not moving nor measuring. Make sure movable points are updated
                foreach (var point in Points)
                {
                    point.OnMouseMove(p);
                }
            }

            // Done refreshing. Save pos for next round
            _prevPos = p;
        }

        public override string ToString()
        {
            return ToolId.GetDisplayName();
        }
    }
}
