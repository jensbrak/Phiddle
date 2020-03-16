using System.Collections.Generic;
using Phiddle.Core.Graphics;
using Phiddle.Core.Extensions;
using SkiaSharp;
using System;

namespace Phiddle.Core.Measure
{
    public abstract class ToolBase : ITool
    {
        protected WindowBase frame;
        protected readonly float boundsVisualPadding;
        protected readonly float boundsGripMargin;
        protected LabelLocation labelPlacement;
        protected Endpoint p0; // top-left
        protected Endpoint p1; // bottom-right
        protected Endpoint p2; // bottom-left
        protected Endpoint p3; // top-right
        protected MarkBase[] marks;

        public Endpoint ActiveEndpoint
        {
            get
            {
                if (p0.Focused)
                {
                    return p0;
                }
                else if (p1.Focused)
                {
                    return p1;
                }
                else if (p2.Focused)
                {
                    return p2;
                }
                else if (p3.Focused)
                {
                    return p3;
                }

                return null;
            }
        }
        public SKPaint PaintTool { get; set; }
        public bool Visible { get; set; }
        public bool Locked { get; set; }
        public bool Movable { get; set; }
        public bool Moving { get; set; }
        public bool Resizable { get; set; }
        public bool Resizing { get; set; }
        public Label Label { get; set; }
        public LabelLocation LabelLocation
        {
            get => labelPlacement;
            set
            {
                labelPlacement = value;
                UpdateLabel();
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="screenService"></param>
        protected ToolBase()
        {
            Visible = false;
            p0 = new Endpoint(SKPoint.Empty);
            p1 = new Endpoint(SKPoint.Empty);
            p2 = new Endpoint(SKPoint.Empty);
            p3 = new Endpoint(SKPoint.Empty);
            boundsVisualPadding = Defaults.ToolBoundsVisualPadding;
            boundsGripMargin = Defaults.ToolEndpointSize;
            frame = new WindowBase(new SKRect()) { PaintBorder = Defaults.ToolBasePaintBounds };
            PaintTool = Defaults.ToolBasePaint;
        }

        public void ToggleMarks(MarkCategory c)
        {
            foreach (var m in marks)
            {
                if (m.Category == c)
                {
                    m.Visible = !m.Visible;
                    break;  // Should be only one of each
                }
            }
        }

        /// <summary>
        /// Resize the tool at endpoint P1
        /// </summary>
        /// <param name="p">The new position of P1</param>
        public virtual void Resize(SKPoint p)
        {
            // No endpoint focused means nothing to resize
            if (!(p0.Focused || p1.Focused || p2.Focused || p3.Focused))
            {
                return;
            }

            // Get new position with respect to if tool is locked or not
            var pNew = Locked ? GetLockedPos(p) : p;

            // Update the right endpoint and dependent one correspondingly
            if (p0.Focused)
            {
                p0.Pos = pNew;  // moving
                // p1.Pos fixed
                p2.Pos = new SKPoint(p0.Pos.X, p2.Pos.Y);
                p3.Pos = new SKPoint(p3.Pos.X, p0.Pos.Y);
            }
            else if (p1.Focused)
            {
                // p0.Pos fixed
                p1.Pos = pNew; // moving
                p2.Pos = new SKPoint(p2.Pos.X, p1.Pos.Y);
                p3.Pos = new SKPoint(p1.Pos.X, p3.Pos.Y);
            }
            else if (p2.Focused)
            {
                p2.Pos = pNew; // moving
                p0.Pos = new SKPoint(p2.Pos.X, p0.Pos.Y);
                p1.Pos = new SKPoint(p1.Pos.X, p2.Pos.Y);
                // p3.Pos fixed
            }
            else if (p3.Focused)
            {
                p3.Pos = pNew;
                p0.Pos = new SKPoint(p0.Pos.X, p3.Pos.Y);
                p1.Pos = new SKPoint(p3.Pos.X, p1.Pos.Y);
                //p2.Pos fixed
            }
            // Make sure rect gets properly adjusted with margins
            var left = Math.Min(Math.Min(p0.X, p1.X), Math.Min(p2.X, p3.X));
            var top = Math.Min(Math.Min(p0.Y, p1.Y), Math.Min(p2.Y, p3.Y));
            var right = Math.Max(Math.Max(p0.X, p1.X), Math.Max(p2.X, p3.X));
            var bottom = Math.Max(Math.Max(p0.Y, p1.Y), Math.Max(p2.Y, p3.Y));

            // Make bounds for tool a little larger than the tool itself, so that drawing bounds will not obscure the tool
            frame.Bounds = new SKRect(
                left - boundsVisualPadding,
                top - boundsVisualPadding,
                right + boundsVisualPadding,
                bottom + boundsVisualPadding);

            // Make label be correctly positioned
            UpdateLabel();
        }

        /// <summary>
        /// Move the the tool the given amount
        /// </summary>
        /// <param name="p">The relative movement</param>
        public virtual void Move(SKPoint p)
        {
            // p is delta movement
            p0.Pos += p;
            p1.Pos += p;
            p2.Pos += p;
            p3.Pos += p;
            frame.Bounds = new SKRect(
                frame.Bounds.Left + p.X,
                frame.Bounds.Top + p.Y,
                frame.Bounds.Right + p.X,
                frame.Bounds.Bottom + p.Y);
            UpdateLabel();
        }

        /// <summary>
        /// Check if given point is within bounds. Instead of returning the result, update tool.
        /// Use <see cref="Movable"/> to check if last point was within bounds, ie tool in focus. 
        /// </summary>
        /// <param name="p">The point to check</param>
        public virtual void CheckBounds(SKPoint p)
        {
            // Check movement bounds
            var b = frame.Bounds;
            b.Inflate(-2 * boundsGripMargin, -2 * boundsGripMargin);
            Movable = b.Inside(p);

            // Check resize bounds
            p0.CheckBounds(p);
            p1.CheckBounds(p);
            p2.CheckBounds(p);
            p3.CheckBounds(p);
            Resizable = p0.Focused || p1.Focused || p2.Focused || p3.Focused;
        }

        /// <summary>
        /// Update tool at given point with next action. The action cycle is currently:
        /// 1. Hidden --> 
        /// 2. Resizing  --> 
        /// 3. Passive -->
        /// 1. Hidden ...
        ///   
        /// However, if the tool is 'Passive' it means that it is visible, has ben initially
        /// resized/placed and can be moved or resized (again).
        /// If this is possible is determined by mous emovement - but it is next action that 
        /// actually initiate it. .
        /// </summary>
        /// <param name="p"></param>
        public virtual void NextAction(SKPoint p)
        {
            // From hidden to initial resize:
            if (!Visible)
            {
                Console.WriteLine($"Phiddle.Core.ToolBase.NextAction: action = Start resizing");
                // Start resizing
                p0.Pos = p;
                p1.Pos = p;
                p2.Pos = p;
                p3.Pos = p;

                p1.Focused = true;
                Resizing = true;
                Visible = true;
                return;
            }

            // From resizing to passive
            if (Resizing)
            {
                Console.WriteLine($"Phiddle.Core.ToolBase.NextAction: action = Stop resizing (first)");
                // Finish resizing
                Resizing = false;
                return;
            }

            // From moving to passive
            if (Moving)
            {
                // Finish moving
                Console.WriteLine($"Phiddle.Core.ToolBase.NextAction: action = Stop moving");
                Moving = false;
                return;
            }

            // Passive and movable so start moving
            if (Movable)
            {
                Console.WriteLine($"Phiddle.Core.ToolBase.NextAction: action = Start moving");
                // Start moving
                Moving = true;
                Movable = false;
                return;
            }

            // Passive and resizable so start resizing (once again) 
            if (Resizable)
            {
                Console.WriteLine($"Phiddle.Core.ToolBase.NextAction: action = Start resizing (again)");
                // Start resizing
                Resizing = true;
                Resizable = false;
                return;
            }

            Console.WriteLine($"Phiddle.Core.ToolBase.NextAction: action = Hiding");
            // Passive and neither movable or resizable so go back to hidden
            Visible = false;
            Moving = false;
            Resizing = false;
        }

        /// <summary>
        /// Draw the tool and its associated elements on the given canvas
        /// </summary>
        /// <param name="c">The canvas to draw the tool on</param>
        public virtual void Draw(SKCanvas c)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return the current measurements the tool might have in its current state
        /// </summary>
        /// <returns>The measurements</returns>
        public virtual Dictionary<Measurement, float> GetMeasurements()
        {
            return new Dictionary<Measurement, float>();
        }

        /// <summary>
        /// Refresh the tool, ie make sure it is up to date
        /// </summary>
        protected void UpdateLabel()
        {
            Label.Pos = GetLabelPos();
            Label.Text = GetLabelText();
        }

        protected virtual void DrawBase(SKCanvas c)
        {
            // Do we have a tool that can be moved?
            if (Visible && Movable)
            {
                frame.Draw(c);
            }

            if (Visible && Resizable && !Resizing)
            {
                p0.Draw(c);
                p1.Draw(c);
                p2.Draw(c);
                p3.Draw(c);
            }

            // Show label?
            if (Visible && LabelLocation != LabelLocation.Off)
            {
                Label.Draw(c);
            }
        }

        // Some internals: a tool needs to be able to draw marks and calculate label properties
        protected virtual void DrawMarks(SKCanvas c)
        {
            throw new NotImplementedException("DrawMarks");
        }
        protected virtual string GetLabelText()
        {
            throw new NotImplementedException("DrawMarks");
        }
        protected virtual SKPoint GetLabelPos()
        {
            throw new NotImplementedException("DrawMarks");
        }
        protected virtual SKPoint GetLockedPos(SKPoint p)
        {
            throw new NotImplementedException("DrawMarks");
        }
        protected virtual MarkBase[] DefaultMarks()
        {
            return new MarkBase[] { };
        }
    }
}