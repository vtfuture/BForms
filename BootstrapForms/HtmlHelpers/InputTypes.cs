using System.ComponentModel;

namespace BootstrapForms.HtmlHelpers
{
    public enum BsInputType
    {
        [Description("color")]
        Color,
        [Description("date")]
        Date,
        [Description("datetime")]
        Datetime,
        [Description("datetime-local")]
        DatetimeLocal,
        [Description("email")]
        Email,
        [Description("month")]
        Month,
        [Description("number")]
        Number,
        [Description("range")]
        Range,
        [Description("search")]
        Search,
        [Description("tel")]
        Tel,
        [Description("time")]
        Time,
        [Description("url")]
        Url,
        [Description("week")]
        Week
    }
}