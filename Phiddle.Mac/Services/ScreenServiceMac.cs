using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AppKit;
using CoreGraphics;
using Foundation;
using Phiddle.Core;
using Phiddle.Core.Services;
using Phiddle.Mac.Extensions;
using SkiaSharp;
using SkiaSharp.Views.Mac;

namespace Phiddle.Mac.Services
{
    public class ScreenServiceMac : NSObject, IScreenService
    {
        private MouseState mouseState;
        private NSCursor cursor;

        public PhiddleWindow PhiddleWindow { get; set; }

        public MouseState MouseState
        {
            get
            {
                return mouseState;
            }
            set
            {
                if (mouseState == value)
                {
                    return;
                }

                switch (value)
                {
                    case MouseState.Normal:
                        cursor?.Pop();
                        cursor = NSCursor.ArrowCursor;
                        cursor.Push();
                        //Cursors.Default;
                        break;
                    case MouseState.CanGrip:
                        cursor?.Pop();
                        cursor = NSCursor.OpenHandCursor;
                        cursor.Push();
                        //Cursors.Hand;
                        break;
                    case MouseState.Moving:
                        cursor?.Pop();
                        cursor = NSCursor.ClosedHandCursor;
                        cursor.Push();
                        //Cursors.SizeAll;
                        break;
                    case MouseState.Resizing:
                        cursor?.Pop();
                        cursor = NSCursor.ClosedHandCursor;
                        cursor.Push();
                        //Cursors.SizeAll;
                        break;
                    case MouseState.Blocked:
                        cursor?.Pop();
                        cursor = NSCursor.OperationNotAllowedCursor;
                        cursor.Push();
                        //Cursors.No;
                        break;
                    default:
                        break;
                }
                mouseState = value;
            }
        }

        public ScreenServiceMac(PhiddleWindow phiddleWindow)
        {
            PhiddleWindow = phiddleWindow;
        }

        [DllImport("/System/Library/Frameworks/ApplicationServices.framework/Versions/A/Frameworks/CoreGraphics.framework/CoreGraphics")]
        private static extern IntPtr CGWindowListCreateImage(CGRect screenBounds, CGWindowListOption windowOption, uint windowID, CGWindowImageOption imageOption);

        public SKImage Capture(SKRectI rect)
        {
            var tcs = new TaskCompletionSource<SKImage>();

            BeginInvokeOnMainThread(() =>
            {
                try
                {
                    using (var pool = new NSAutoreleasePool())
                    { 
                        IntPtr imageRef = CGWindowListCreateImage(rect.ToCGRect(), CGWindowListOption.All, 0, CGWindowImageOption.Default);
                        SKImage image = new CGImage(imageRef).ToSKImage();
                        tcs.SetResult(image);
                    }
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });

            return tcs.Task.Result;
        }

        public SKRectI Dimensions()
        {
            CGRect screen = PhiddleWindow.Frame;
            return new SKRectI(0, 0, (int)screen.Width, (int)screen.Height);
        }

        public void Invalidate(SKPointI pos)
        {
            BeginInvokeOnMainThread(() =>
            {
                var aPoint = pos.ToCGPointFlipY();
                var view = PhiddleWindow.ContentView.HitTest(aPoint);
                view.NeedsDisplay = true;
            }); 
        }

        public SKPointI MousePosition()
        {
            var tcs = new TaskCompletionSource<SKPointI>();

            BeginInvokeOnMainThread(() =>
            {
                var pos = PhiddleWindow.ViewTool.MousePosition.ToSKPointIFlipY();
                tcs.SetResult(pos);
            });

            return tcs.Task.Result;
        }
    }
}
