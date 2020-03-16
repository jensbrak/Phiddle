using SkiaSharp;

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
    public abstract class MarkBase
    {
        protected MarkBase(MarkCategory markCategory)
        {
            Category = markCategory;
            Visible = false;
        }
        public MarkCategory Category { get; set; }
        public float[] Pos { get; set; }
        public SKPaint PaintMark { get; set; }
        public float Size { get; set; }
        public bool Visible { get; set; }
    }

    public class MarkEndpoint : MarkBase
    {
        public MarkEndpoint() : base(MarkCategory.Endpoint)
        {
            Pos = new float[] { 0, 1 };
            Size = Defaults.ToolEndpointSize;
            PaintMark = Defaults.ToolBasePaint;
            Visible = Defaults.ToolMarkVisibleEndpoint;
       }
    }
    public class MarkGoldenRatio : MarkBase
    {
        public MarkGoldenRatio() : base(MarkCategory.GoldenRatio)
        {
            Pos = new float[] { 1 - Constants.PhiInv, Constants.PhiInv };
            Size = Defaults.ToolMarkSize;
            PaintMark = Defaults.ToolPaintMarkPhi;
            Visible = Defaults.ToolMarkVisibleGoldenRatio;
        }
    }
    public class MarkMiddle : MarkBase
    {
        public MarkMiddle() : base(MarkCategory.Middle)
        {
            Pos = new float[] { 1f/2f };
            Size = Defaults.ToolMarkSize;
            PaintMark = Defaults.ToolBasePaint;
            Visible = Defaults.ToolMarkVisibleMiddle;
        }
    }

    public class MarkThird : MarkBase
    {
        public MarkThird() : base(MarkCategory.Third)
        {
            Pos = new float[] { 1f/3f, 2f/3f };
            Size = Defaults.ToolMarkSize;
            PaintMark = Defaults.ToolBasePaint;
            Visible = Defaults.ToolMarkVisibleThird;
        }
    }
}
