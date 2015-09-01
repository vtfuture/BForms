using System.ComponentModel;

namespace BForms.Models
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
        [Description("red")]
        Red = 5,
        [Description("purple")]
        Purple = 6,
        [Description("black")]
        Black = 7
    }
}