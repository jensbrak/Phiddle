using System;
using AppKit;
using CoreGraphics;
using Phiddle.Core;
using SkiaSharp;
using SkiaSharp.Views.Mac;

namespace Phiddle.Mac
{
    public class PhiddleWindow : NSWindow
    {
        private PhiddleView viewTool;
        private PhiddleView viewInfo;
        private PhiddleView viewZoom;

        public PhiddleView ViewTool
        {
            get
            {
                return viewTool;
            }
            set
            {
                viewTool = value;
                ContentView = viewTool;
            }
        }

        public PhiddleView ViewInfo
        {
            get
            {
                return viewInfo;
            }
            set
            {
                viewInfo = value;
                ContentView.AddSubview(viewInfo);
            }
        }

        public PhiddleView ViewZoom
        {
            get
            {
                return viewZoom;
            }
            set
            {
                viewZoom = value;
                ContentView.AddSubview(viewZoom);
            }
        }

        public PhiddleCore Phiddle { get; set; }

        public override bool CanBecomeMainWindow => true;

        public override bool CanBecomeKeyWindow => true;

        public override bool AcceptsFirstResponder()
        {
            return true;
        }

        public PhiddleWindow(CGRect contentRect, NSWindowStyle aStyle, NSBackingStore bufferingType, bool deferCreation)
            : base(contentRect, aStyle, bufferingType, deferCreation)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            ContentView.AwakeFromNib();
            Phiddle.StartUp();
        }

    }
}
