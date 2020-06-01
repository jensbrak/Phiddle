using AppKit;
using CoreGraphics;
using Phiddle.Core;
using Phiddle.Core.Settings;
using Phiddle.Core.Services;
using Phiddle.Mac.Extensions;
using Phiddle.Mac.Settings;
using SkiaSharp.Views.Mac;
using System;

namespace Phiddle.Mac
{
    public class PhiddleView : SKGLView
    {
        private readonly ILogService log;
        private readonly AppInput<NSKey> appInput;
        private readonly PhiddleCore phiddle;
        private NSTrackingArea trackingArea;

        public CGPoint MousePosition { get; set; }

        public PhiddleView(CGRect frame, PhiddleCore phiddle, ISettingsService<AppInput<NSKey>> settingsService, ILogService log) : base(frame)
        {
            this.log = log;
            this.phiddle = phiddle;
            
            appInput = settingsService.Settings;

            if (!settingsService.Loaded)
            {
                log.Warning("PhiddleView", "Could not load settings for App Input, using defaults");
            }

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
            if (!appInput.InputMap.ContainsKey(theEvent.KeyCode))
            {
                return;
            }

            var action = appInput.InputMap[theEvent.KeyCode];
            phiddle.InvokeAction(action);

            if (action == ActionId.ApplicationExit)
            {
                Window.Close();
            }
            else
            {
                NeedsDisplay = true;
            }

        }

        public override void MouseDown(NSEvent theEvent)
        {
            MousePosition = theEvent.LocationInWindow;
            var pos = MousePosition.ToSKPointFlipY();
            phiddle.MouseClicked(pos);
            NeedsDisplay = true;
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
        }
    }
}
