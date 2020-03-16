using System;
using System.Threading.Tasks;
using AppKit;
using CoreGraphics;
using Foundation;
using SkiaSharp;
using SkiaSharp.Views.Mac;

namespace Phiddle.Mac.Extensions
{
    public static class PhiddleMac
    {

        // Mac -> SK
        public static SKPoint ToSKPointFlipY
            (this CGPoint p)
        {
            var h = NSScreen.MainScreen.Frame.Height;
            return new SKPoint((float)p.X, (float)(h - p.Y));
        }

        // Mac -> SK
        public static SKPointI ToSKPointIFlipY
            (this CGPoint p)
        {
            var h = NSScreen.MainScreen.Frame.Height;
            return new SKPointI((int)p.X, (int)(h - p.Y));
        }

        // Mac -> SK
        public static SKRect ToSKRect(this CGRect r)
        {
            return new SKRect((float)r.Left, (float)r.Bottom, (float)r.Right, (float)r.Top);
        }

        // Mac -> SK
        public static SKRectI ToSKRectI(this CGRect r)
        {
            // Should they bey rounded or truncated in some dir?
            return new SKRectI((int)r.Left, (int)r.Top, (int)r.Right, (int)r.Bottom);
        }

        // Mac -> SK
        public static SKSize ToSKSize(this CGSize s)
        {
            return new SKSize((float)s.Width, (float)s.Height);
        }

        // Mac -> SK
        public static SKSizeI ToSKSizeI(this CGSize s)
        {
            return new SKSizeI((int)s.Width, (int)s.Height);
        }

        // SK -> Mac
        public static CGPoint ToCGPointFlipY(this SKPoint p)
        {
            var h = NSScreen.MainScreen.Frame.Height;
            return new CGPoint(p.X, h - p.Y);
        }

        // SK -> Mac
        public static CGPoint ToCGPointFlipY(this SKPointI p)
        {
            var h = NSScreen.MainScreen.Frame.Height;
            return new CGPoint(p.X, h - p.Y);
        }

        // SK -> Mac
        public static CGRect ToCGRect(this SKRect r)
        {
            return new CGRect(r.Left, r.Top, r.Right, r.Bottom);
        }

        // SK -> Mac
        public static CGRect ToCGRect(this SKRectI r)
        {
            return new CGRect(r.Left, r.Top, r.Width, r.Height);
        }

        // SK -> Mac
        public static CGSize ToCGSize(this SKSize s)
        {
            return new CGSize(s.Width, s.Height);
        }

        // SK -> Mac
        public static CGSize ToCGSize(this SKSizeI s)
        {
            return new CGSize(s.Width, s.Height);
        }
    }
}
