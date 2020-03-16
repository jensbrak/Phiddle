using System;
using Foundation;

using AppKit;
using CoreGraphics;
using SkiaSharp.Views.Mac;
using SkiaSharp;
using Phiddle.Core;
using Phiddle.Core.Services;
using Phiddle.Mac.Extensions;
using Phiddle.Mac.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.InteropServices;

namespace Phiddle.Mac
{
	public class PhiddleWindowController : NSWindowController
	{

        public new PhiddleWindow Window { get; private set; }

        private PhiddleCore phiddle;
        private IScreenService  screen;
		
		public PhiddleWindowController() : base()
		{
			InitializePhiddleWindow();
			InitializePhiddleCore();
			InitializePhiddleViews();
			AwakeFromNib();
		}


	[DllImport("/System/Library/Frameworks/ApplicationServices.framework/Versions/A/Frameworks/CoreGraphics.framework/CoreGraphics")]
	private static extern int CGWarpMouseCursorPosition(CGPoint newCursorPosition);

	private void InitializePhiddleWindow()
		{
			// Dimensions
			CGRect displayRect = NSScreen.MainScreen.Frame;
			CGRect viewRect = new CGRect(0, 0, displayRect.Width, displayRect.Height);

			// Window
			var style = NSWindowStyle.Borderless | NSWindowStyle.Resizable;
			Window = new PhiddleWindow(viewRect, style, NSBackingStore.Buffered, true)
			{
				Level = NSWindowLevel.MainMenu + 1,
				//HidesOnDeactivate = true,
				IsOpaque = false,
				HasShadow = false,
				TitlebarAppearsTransparent = true,
				BackgroundColor = NSColor.Clear,
			};

			var pos = new CGPoint(displayRect.Width / 2, displayRect.Height / 2);
			CGWarpMouseCursorPosition(pos);
		}

		private void InitializePhiddleCore()
		{
			phiddle = new PhiddleCore();
			screen = new ScreenServiceMac(Window);
			phiddle.ServiceCollection.AddSingleton(screen);
			phiddle.Initialize();
			Window.Phiddle = phiddle;
		}

		private void InitializePhiddleViews()
        {
			var frame = NSScreen.MainScreen.Frame;
			var viewTool = new PhiddleView(frame, phiddle, Window);
			viewTool.Identifier = "ViewTool";
			viewTool.PaintSurface += HandlePaintGLSurfaceTool;
            //_ = viewTool.BecomeFirstResponder();
			Window.ViewTool = viewTool;

			var il = phiddle.InfoWindowLocation.ToCGPointFlipY();
			var ib = phiddle.InfoWindowSize.ToCGSize();
			var viewRectInfo = new CGRect(il.X, il.Y - ib.Height, ib.Width, ib.Height);

			var viewInfo = new PhiddleView(viewRectInfo, phiddle, Window);
			viewInfo.Identifier = "ViewInfo";
            viewInfo.PaintSurface += HandlePaintGLSurfaceInfo;
            Window.ViewInfo = viewInfo;

			var zl = phiddle.ZoomWindowLocation.ToCGPointFlipY();
			var zb = phiddle.ZoomWindowSize.ToCGSize();
			var viewRectZoom = new CGRect(zl.X, zl.Y - zb.Height, zb.Width, zb.Height);

			var viewZoom = new PhiddleView(viewRectZoom, phiddle, Window);
			viewZoom.Identifier = "ViewZoom";
			viewZoom.PaintSurface += HandlePaintGLSurfaceZoom;
			Window.ViewZoom = viewZoom;
		}

		private void HandlePaintGLSurfaceTool(object sender, SKPaintGLSurfaceEventArgs e)
		{
			var bg = SKColors.Transparent;
			e.Surface.Canvas.Clear(bg);
			phiddle.DrawTool(e.Surface.Canvas);
		}

		private void HandlePaintGLSurfaceInfo(object sender, SKPaintGLSurfaceEventArgs e)
		{
			var bg = SKColors.Transparent;
			e.Surface.Canvas.Clear(bg);
			phiddle.DrawInfoWindow(e.Surface.Canvas);
		}

		private void HandlePaintGLSurfaceZoom(object sender, SKPaintGLSurfaceEventArgs e)
		{
			var bg = SKColors.Transparent;
			e.Surface.Canvas.Clear(bg);
			phiddle.DrawZoomWindow(e.Surface.Canvas);
		}

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
			Window.AwakeFromNib();
        }
    }
}
