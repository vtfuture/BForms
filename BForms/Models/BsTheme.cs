using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace BForms.Grid
{
    /// <summary>
    /// Provides theme support for BForms components
    /// </summary>
    public enum BsTheme
    {
        [Description("turqoise")]
        Default = 1,
        [Description("blue")]
        Blue = 2,
        [Description("green")]
        Green = 3,
        [Description("orange")]
        Orange = 4,
        [Description("purple")]
        Purple = 5,
        [Description("black")]
        Black = 6
    }
}