using AppKit;
using CoreGraphics;
using Phiddle.Core;
using Phiddle.Mac.Extensions;
using SkiaSharp.Views.Mac;

namespace Phiddle.Mac
{
    public class PhiddleView : SKGLView
    {
        private readonly PhiddleCore phiddle;
        private readonly PhiddleWindow phiddleWindow;
        private NSTrackingArea trackingArea;

        public CGPoint MousePosition { get; set; }

        public PhiddleView(CGRect frame, PhiddleCore phiddle, PhiddleWindow phiddleWindow) : base(frame)
        {
            this.phiddle = phiddle;
            this.phiddleWindow = phiddleWindow;
        }

        public override void PrepareOpenGL()
        {
            base.PrepareOpenGL();
            OpenGLContext.SurfaceOpaque = false;
        }
        
        public override bool AcceptsFirstResponder()
        {
            return true;
        }

        /*
        public override bool PerformKeyEquivalent(NSEvent theEvent)
        {
            return true;
        }*/

        public override void AwakeFromNib()
        {
            //base.AwakeFromNib();
            trackingArea = new NSTrackingArea(Frame, NSTrackingAreaOptions.ActiveInKeyWindow | NSTrackingAreaOptions.MouseMoved | NSTrackingAreaOptions.CursorUpdate, this, null);
            AddTrackingArea(trackingArea);
        }

        public override void KeyDown(NSEvent theEvent)
        {
            var key = (NSKey)theEvent.KeyCode;

            switch (key)
            {
                case NSKey.Escape:
                    phiddle.Stop();
                    //NSApplication.SharedApplication.Terminate(this);
                    Window.Close();
                    break;
                case NSKey.Space:
                    phiddle.SelectNextTool();
                    NeedsDisplay = true;
                    break;
                case NSKey.L:
                    phiddle.ToggleLabelPlacement();
                    NeedsDisplay = true;
                    break;
                case NSKey.Z:
                    phiddle.ZoomWindowVisible = !phiddle.ZoomWindowVisible;
                    NeedsDisplay = true;
                    break;
                case NSKey.I:
                    phiddle.InfoWindowVisible = !phiddle.InfoWindowVisible;
                    NeedsDisplay = true;
                    break;
                case NSKey.H:
                    phiddle.HelpLinesVisible = !phiddle.HelpLinesVisible;
                    NeedsDisplay = true;
                    break;
                case NSKey.G:
                    phiddle.ToggleToolMarks(Core.Measure.MarkCategory.GoldenRatio);
                    NeedsDisplay = true;
                    break;
                case NSKey.E:
                    phiddle.ToggleToolMarks(Core.Measure.MarkCategory.Endpoint);
                    NeedsDisplay = true;
                    break;
                case NSKey.M:
                    phiddle.ToggleToolMarks(Core.Measure.MarkCategory.Middle);
                    NeedsDisplay = true;
                    break;
                case NSKey.T:
                    phiddle.ToggleToolMarks(Core.Measure.MarkCategory.Third);
                    NeedsDisplay = true;
                    break;
                default:
                    break;
            }            
        }

        public override void MouseDown(NSEvent theEvent)
        {
            MousePosition = theEvent.LocationInWindow;
            var pos = MousePosition.ToSKPointFlipY();
            phiddle.MouseClicked(pos);
            NeedsDisplay = true;
            //Console.WriteLine($"Phiddle.Mac.Window.MouseDown: mouse position = {pos}");
        }

        public override void MouseMoved(NSEvent theEvent)
        {
            // Check the 'lock' key if tool is locked
            phiddle.ToolLocked = theEvent.ModifierFlags.HasFlag(NSEventModifierMask.ControlKeyMask);

            // Update mouse pos and refresh
            MousePosition = theEvent.LocationInWindow;
            var pos = MousePosition.ToSKPointFlipY();
            phiddle.MouseMoved(pos);
            NeedsDisplay = true;
            //Console.WriteLine($"Phiddle.Mac.Window.MouseMoved ({Identifier}): mouse position = {pos}");
        }
    }
}
