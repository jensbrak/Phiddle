using System.Collections.Generic;
using System.Windows.Forms;
using Phiddle.Core;
using Phiddle.Core.Settings;

namespace Phiddle.Win.Settings
{
    public static class AppInputWin
    {
        public static AppInput Defaults { get; } =
            new AppInput()
            {
                Keys = new Dictionary<ushort, ActionId>(System.Enum.GetNames(typeof(ActionId)).Length)
                {
                    { (ushort)Keys.Escape, ActionId.ApplicationExit },
                    { (ushort)Keys.Space, ActionId.ToolSelectNext },
                    { (ushort)Keys.L, ActionId.LabelTogglePlacement },
                    { (ushort)Keys.Z, ActionId.WindowZoomToggleVisible },
                    { (ushort)Keys.I, ActionId.WindowInfoToggleVisible },
                    { (ushort)Keys.H, ActionId.HelpLinesToggleVisible },
                    { (ushort)Keys.G, ActionId.ToolMarksGoldenRatioToggleVisible },
                    { (ushort)Keys.E, ActionId.ToolMarksEndpointToggleVisible },
                    { (ushort)Keys.M, ActionId.ToolMarksMiddleToggleVisible },
                    { (ushort)Keys.T, ActionId.ToolMarksThirdToggleVisible },
                }
            };
    }
}
