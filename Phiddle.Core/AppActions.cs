using System;
using Phiddle.Core.Settings;
using Phiddle.Core.Services;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Phiddle.Core
{
    /// <summary>
    /// Any action that can be user initiaded
    /// </summary>
    public enum ActionId
    {
        [Display(Name = "Exit Application", ShortName = "Exit")]
        ApplicationExit,

        [Display(Name = "Select Next Tool", ShortName = "Next")]
        ToolSelectNext,

        [Display(Name = "Toggle Label Placement", ShortName = "Label")]
        LabelTogglePlacement,

        [Display(Name = "Toogle Info Window", ShortName = "Info")]
        WindowInfoToggleVisible,

        [Display(Name = "Toggle Zoom Window", ShortName = "Zoom")]
        WindowZoomToggleVisible,

        [Display(Name = "Toogle Help Lines", ShortName = "Lines")]
        HelpLinesToggleVisible,

        [Display(Name = "Toggle Golden Ratio Marks", ShortName = "Golden")]
        ToolMarksGoldenRatioToggleVisible,

        [Display(Name = "Toggle Endpoint Marks", ShortName = "Endpoint")]
        ToolMarksEndpointToggleVisible,

        [Display(Name = "Toggle Third Marks", ShortName = "Third")]
        ToolMarksThirdToggleVisible,

        [Display(Name = "Toggle Middle Marks", ShortName = "Middle")]
        ToolMarksMiddleToggleVisible,
    }

    public delegate void ActionDelegate();

    public class AppActions
    {
        public Dictionary<ActionId, ActionDelegate> Actions { get; set; }

        public void Invoke(ActionId actionId)
        {
            Actions[actionId].Invoke();
        }
    }
}
