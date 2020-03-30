using AppKit;
using System.Collections.Generic;
using Phiddle.Core;
using Phiddle.Core.Settings;

namespace Phiddle.Mac.Settings
{
    public static class AppInputMac
    {
        public static AppInput Defaults { get; } =
            new AppInput()
            {
                Keys = new Dictionary<ushort, ActionId>(System.Enum.GetNames(typeof(ActionId)).Length)
                {
                    { (ushort)NSKey.Escape, ActionId.ApplicationExit },
                    { (ushort)NSKey.Space, ActionId.ToolSelectNext },
                    { (ushort)NSKey.L, ActionId.LabelTogglePlacement },
                    { (ushort)NSKey.Z, ActionId.WindowZoomToggleVisible },
                    { (ushort)NSKey.I, ActionId.WindowInfoToggleVisible },
                    { (ushort)NSKey.H, ActionId.HelpLinesToggleVisible },
                    { (ushort)NSKey.G, ActionId.ToolMarksGoldenRatioToggleVisible },
                    { (ushort)NSKey.E, ActionId.ToolMarksEndpointToggleVisible },
                    { (ushort)NSKey.M, ActionId.ToolMarksMiddleToggleVisible },
                    { (ushort)NSKey.T, ActionId.ToolMarksThirdToggleVisible },
                }
            };
    }
}
