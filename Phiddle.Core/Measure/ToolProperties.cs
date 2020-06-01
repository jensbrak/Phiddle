using System;
using System.ComponentModel.DataAnnotations;

namespace Phiddle.Core.Measure
{
    public enum ToolId
    {
        [Display(Name = "Line", ShortName = "L")]
        Line,
        [Display(Name = "Rectangle", ShortName = "R")]
        Rect,
        [Display(Name = "Oval", ShortName = "O")]
        Oval
    }

    /// <summary>
    /// The different locations a label can have, relate a Tool
    /// </summary>
    public enum LabelLocation
    {
        [Display(Name = "Off")]
        Off,
        [Display(Name = "Center Tool")]
        CenterTool,
        [Display(Name = "Above Mouse")]
        AboveMouse,
    }

    /// <summary>
    /// The different kind of measurements available
    /// </summary>
    public enum Measurement
    {
        [Display(Name = "Length", ShortName = "L")]
        Length,
        [Display(Name = "Width", ShortName = "W")]
        Width,
        [Display(Name = "Height", ShortName = "H")]
        Height,
        [Display(Name = "Area", ShortName = "A")]
        Area,
        [Display(Name = "Circumference", ShortName = "C")]
        Circumference,
    }

    /// <summary>
    /// The categories of marks for a tool
    /// </summary>
    
    [Flags]public enum MarkId
    {
        [Display(Name = "Endpoint", ShortName = "EP")]
        Endpoint = 1,
        [Display(Name = "Golden Ratio", ShortName = "GR")]
        Phi = 2,
        [Display(Name = "Middle", ShortName = "½")]
        Middle = 4,
        [Display(Name = "Third", ShortName = "⅓")]
        Third = 8,
    }

}
