using System.ComponentModel.DataAnnotations;

namespace Phiddle.Core.Measure
{
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
    public enum MarkCategory
    {
        [Display(Name = "Endpoint", ShortName = "EP")]
        Endpoint,
        [Display(Name = "Golden Ratio", ShortName = "GR")]
        GoldenRatio,
        [Display(Name = "Middle", ShortName = "½")]
        Middle,
        [Display(Name = "Third", ShortName = "⅓")]
        Third,
    }

}
