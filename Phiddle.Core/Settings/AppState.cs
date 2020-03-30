using Phiddle.Core.Measure;

namespace Phiddle.Core.Settings
{
    public class AppState : ISettings
    {
        public int CurrentTool { get; set; } = 0;
        public LabelLocation LabelLocation { get; set; } = LabelLocation.Off;
        public bool HelpLinesVisible { get; set; } = false;
        public bool WindowZoomVisible { get; set; } = true;
        public bool WindowInfoVisible { get; set; } = true;
        public bool MarkEndpointVisible { get; set; } = true;
        public bool MarkGoldenRatioVisible { get; set; } = true;
        public bool MarkMiddleVisible { get; set; } = false;
        public bool MarkThirdVisible { get; set; } = false;
    }
}