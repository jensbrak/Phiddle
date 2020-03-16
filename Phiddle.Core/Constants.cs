using System;
using System.ComponentModel.DataAnnotations;

namespace Phiddle.Core
{
    /// <summary>
    /// The different states of a mouse in Phiddle Core. Intended to make mouse appearance platform
    /// independant. It is up to each platform to map this to suitable cursor appearance. See
    /// <see cref="Services.IScreenService.MouseState"/>.
    /// </summary>
    public enum MouseState
    {
        [Display(Name="Normal", ShortName = "N")]
        Normal,
        [Display(Name = "Can grip", ShortName = "G")]
        CanGrip,
        [Display(Name = "Moving", ShortName = "M")]
        Moving,
        [Display(Name = "Resizing", ShortName = "R")]
        Resizing,
        [Display(Name = "Blocked", ShortName = "B")]
        Blocked,
    }

    /// <summary>
    /// P
    /// </summary>
    public static class Constants
    {
        // Application
        public static readonly string AppName = "Phiddle";

        // Math
        public static readonly float PhiInv = (float)(2d / (1d + Math.Sqrt(5d)));
    }
}
