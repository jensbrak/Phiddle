using SkiaSharp;
using Phiddle.Core.Settings;

namespace Phiddle.Core.Measure
{
    /// <summary>
    /// A mark on a tool. The mark has a relative position on 
    /// the tool that ranges from 0 to 1, where 0 is start of
    /// tool and 1 is end of tool (up to implementation of a
    /// specific tool to determine what this means).
    /// The size is also up to the tool to use as seen fit and
    /// can be seen as a recommendation. 
    /// </summary>
    public class Mark
    {
        public Mark(MarkId markId, SettingsMark settings)
        {
            Pos = settings.Pos;
            Size = settings.Size;
            PaintMark = settings.PaintMark.ToSKPaint();
            MarkId = markId;
        }
        public MarkId MarkId { get; set; }
        public float[] Pos { get; set; }
        public SKPaint PaintMark { get; set; }
        public float Size { get; set; }
        public bool Visible { get; set; }
    }
}
