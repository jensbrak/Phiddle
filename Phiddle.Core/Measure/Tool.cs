using System.Collections.Generic;
using Phiddle.Core.Graphics;
using Phiddle.Core.Extensions;
using SkiaSharp;
using System;
using System.Linq;
using Phiddle.Core.Settings;

namespace Phiddle.Core.Measure
{
    /// <summary>
    /// Endpoint layout:
    /// 
    ///  p0_____p3
    ///   |     |
    ///   |_____|
    ///  p2     p1
    /// 
    /// </summary>
    public abstract class Tool : ITool
    {
        protected readonly float boundsVisualPadding;
        protected readonly float boundsGripMargin;
        protected LabelLocation labelPlacement;
        protected Window frame;
        protected Endpoint p0; // top-left
        protected Endpoint p1; // bottom-right
        protected Endpoint p2; // bottom-left
        protected Endpoint p3; // top-right
        protected Mark[] marks;
        protected float width;
        protected float widthFactor;
        private bool wideLinesOn;

        public Endpoint ActiveEndpoint
        {
            get
            {
                return p0.Focused ? p0 : p1.Focused ? p1 : p2.Focused ? p2 : p3.Focused ? p3 : null;
            }
        }

        public Endpoint DiagonalToActiveEndpoint
        {
            get
            {
                return p0.Focused ? p1 : p1.Focused ? p0 : p2.Focused ? p3 : p3.Focused ? p2 : null;
            }
        }

        public ToolId ToolId { get; protected set; }
        public SKPaint PaintTool { get; set; }
        public bool Visible { get; set; }
        public bool Locked { get; set; }
        public bool Movable { get; set; }
        public bool Moving { get; set; }
        public bool Resizable { get; set; }
        public bool Resizing { get; set; }
        public Label Label { get; set; }
        public bool WideLinesOn
        {
            get => wideLinesOn;
            set
            {
                wideLinesOn = value;
                var newWidth = wideLinesOn ? width * widthFactor : width;
                PaintTool.StrokeWidth = newWidth;
                foreach (var m in marks)
                {
                    m.PaintMark.StrokeWidth = newWidth;
                }

            }
        }

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
        protected Tool(SettingsTool settingsTool)
        {
            Visible = false;
            p0 = new Endpoint(SKPoint.Empty, settingsTool);
            p1 = new Endpoint(SKPoint.Empty, settingsTool);
            p2 = new Endpoint(SKPoint.Empty, settingsTool);
            p3 = new Endpoint(SKPoint.Empty, settingsTool);
            boundsVisualPadding = settingsTool.BoundsPad;
            boundsGripMargin = settingsTool.SizeEndpoint;
            frame = new Window(SKPoint.Empty, SKSize.Empty, settingsTool.SettingsToolFrame);
            frame.PaintBorder.PathEffect = SKPathEffect.CreateDash(new float[] { 10f, 5f }, 15f);
            PaintTool = settingsTool.PaintTool.ToSKPaint();
            width = PaintTool.StrokeWidth;
            widthFactor = settingsTool.ToolWidthFactor;
        }

        public void SetMarksVisibility(MarkId visibleMarks)
        {
            foreach (var m in marks)
            {
                m.Visible = visibleMarks.HasFlag(m.MarkId);
            }
        }

        public void ToggleMark(MarkId c)
        {
            foreach (var m in marks)
            {
                if (m.MarkId == c)
                {
                    m.Visible = !m.Visible;
                    break;  // Should be only one of each
                }
            }
        }

        /// <summary>
        /// Resize the tool by updating focused endpoint to position p
        /// </summary>
        /// <param name="p">The new position of focused endpoint</param>
        public virtual void Resize(SKPoint p)
        {
            // No endpoint focused means nothing to resize
            if (!(p0.Focused || p1.Focused || p2.Focused || p3.Focused))
            {
                return;
            }

            // Get new position with respect to if tool is locked or not
            var pNew = Locked ? LockedPos(p) : p;

            // Update the focused endpoint and dependent ones accoringly
            if (p0.Focused)
            {
                p0.Pos = pNew;  // moving endpoint
                // p1.Pos is fixed
                p2.Pos = new SKPoint(p0.Pos.X, p2.Pos.Y); // p2.X depends on p0.X
                p3.Pos = new SKPoint(p3.Pos.X, p0.Pos.Y); // p3.Y depends on p0.Y
            }
            else if (p1.Focused)
            {
                p1.Pos = pNew; // moving endpoint
                // p0.Pos is fixed
                p3.Pos = new SKPoint(p1.Pos.X, p3.Pos.Y); // p3.X depends on p1.X
                p2.Pos = new SKPoint(p2.Pos.X, p1.Pos.Y); // p2.Y depends on p1.Y
            }
            else if (p2.Focused)
            {
                p2.Pos = pNew; // moving endpoint
                // p3.Pos is fixed
                p0.Pos = new SKPoint(p2.Pos.X, p0.Pos.Y); // p0.X depends on p2.X
                p1.Pos = new SKPoint(p1.Pos.X, p2.Pos.Y); // p1.Y depends on p2.Y
            }
            else if (p3.Focused)
            {
                p3.Pos = pNew; // moving endpoint
                //p2.Pos is fixed
                p1.Pos = new SKPoint(p3.Pos.X, p1.Pos.Y); // p1.X depends on p3.X
                p0.Pos = new SKPoint(p0.Pos.X, p3.Pos.Y); // p0.Y depends on p3.Y
            }

            // Make sure rect gets properly adjusted with margins
            UpdateBounds();

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
            frame.Pos += p;
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
                // Finish resizing
                Resizing = false;
                return;
            }

            // From moving to passive
            if (Moving)
            {
                // Finish moving
                Moving = false;
                return;
            }

            // Passive and movable so start moving
            if (Movable)
            {
                // Start moving
                Moving = true;
                Movable = false;
                return;
            }

            // Passive and resizable so start resizing (once again) 
            if (Resizable)
            {
                // Start resizing
                Resizing = true;
                Resizable = false;
                return;
            }

            // Passive and neither movable or resizable so go back to hidden
            Visible = false;
            Moving = false;
            Resizing = false;
        }


        /// <summary>
        /// Return the current measurements the tool might have in its current state
        /// </summary>
        /// <returns>The measurements</returns>
        public virtual Dictionary<Measurement, float> Measure()
        {
            return new Dictionary<Measurement, float>();
        }

        /// <summary>
        /// Refresh the tool, ie make sure it is up to date
        /// </summary>
        protected void UpdateLabel()
        {
            Label.Pos = LabelPos();
            Label.Text = LabelText();
        }

        protected void UpdateBounds()
        {
            var left = Math.Min(Math.Min(p0.X, p1.X), Math.Min(p2.X, p3.X));
            var top = Math.Min(Math.Min(p0.Y, p1.Y), Math.Min(p2.Y, p3.Y));
            var right = Math.Max(Math.Max(p0.X, p1.X), Math.Max(p2.X, p3.X));
            var bottom = Math.Max(Math.Max(p0.Y, p1.Y), Math.Max(p2.Y, p3.Y));

            // Make bounds for tool a little larger than the tool itself, so that drawing bounds will not obscure the tool
            frame.Pos = new SKPoint(left - boundsVisualPadding, top - boundsVisualPadding);
            frame.Size = new SKSize(right - left + boundsVisualPadding * 2f, bottom - top + boundsVisualPadding * 2f);
        }

        /// <summary>
        /// Draw the tool and its associated elements on the given canvas
        /// </summary>
        /// <param name="c">The canvas to draw the tool on</param>
        public virtual void Draw(SKCanvas c)
        {
            if (!Visible || p0 == p1)
            {
                return;
            }
            
            DrawTool(c);
            DrawMarks(c);
            DrawEndpoints(c);
            DrawFrame(c);
            DrawLabel(c);
        }

        protected virtual void DrawFrame(SKCanvas c)
        {
            // Do we have a tool that can be moved?
            if (Visible && Movable)
            {
                frame.Draw(c);
            }
        }

        protected virtual void DrawEndpoints(SKCanvas c)
        {
            if (Visible && Resizable && !Resizing)
            {
                p0.Draw(c);
                p1.Draw(c);
                p2.Draw(c);
                p3.Draw(c);
            }
        }

        protected virtual void DrawLabel(SKCanvas c)
        {
            // Show label?
            if (Visible && LabelLocation != LabelLocation.Off)
            {
                Label.Draw(c);
            }
        }

        protected void EnableMarks(SettingsTool settings, MarkId marksSupported)
        {
            var numMarks = Enum.GetValues(typeof(MarkId)).Cast<Enum>().Count(marksSupported.HasFlag);
            marks = new Mark[numMarks];
            int i = 0;

            foreach (var markId in settings.Marks.Keys)
            {
                if ((markId & marksSupported) != 0)
                {
                    marks[i++] = new Mark(markId, settings.Marks[markId]);
                }
            }
        }

        protected virtual void DrawTool(SKCanvas c)
        {
            throw new NotImplementedException();
        }

        protected virtual void DrawMarks(SKCanvas c)
        {
            throw new NotImplementedException("DrawMarks");
        }

        protected virtual string LabelText()
        {
            throw new NotImplementedException("LabelText");
        }

        protected virtual SKPoint LabelPos()
        {
            throw new NotImplementedException("LabelPos");
        }

        protected virtual SKPoint LockedPos(SKPoint p)
        {
            throw new NotImplementedException("LockedPos");
        }

        public override string ToString()
        {
            return ToolId.GetDisplayName();
        }
    }
}