using SkiaSharp;

namespace Phiddle.Core.Services
{
    public interface IScreenService
    {
        MouseState MouseState { get; set; }
        SKImage Capture(SKRectI rect);
        SKRectI Dimensions();
        SKPointI MousePosition();
        void Invalidate(SKPointI pos);
    }
}
