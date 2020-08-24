using SkiaSharp;

namespace Phiddle.Core.Graphics
{
    public interface IDrawable
    {
        bool Enabled { get; }
        void Draw(SKCanvas c);
    }
}