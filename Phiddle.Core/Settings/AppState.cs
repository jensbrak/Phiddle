using Phiddle.Core.Measure;

namespace Phiddle.Core.Settings
{
    public class AppState : ISettings
    {
        public ToolId ActiveTool { get; set; } = ToolId.Line;
        public LabelLocation LabelLocation { get; set; } = LabelLocation.Off;
        public bool HelpLinesVisible { get; set; } = false;
        public bool WindowZoomVisible { get; set; } = true;
        public bool WindowInfoVisible { get; set; } = true;
        public MarkId MarksVisible { get; set; } = MarkId.Endpoint | MarkId.Phi;
        public bool ToolWideLinesOn { get; set; } = false;
    }
}