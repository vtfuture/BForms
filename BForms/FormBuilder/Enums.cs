using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using BForms.Models;
using BForms.Utilities;

namespace BForms.FormBuilder
{
    public enum FormBuilderControlType
    {
        [FormBuilderControlMetadata(BsControlType = BsControlType.TextBox)]
        [FormBuilderControlDisplay(DisplayName = "Textbox", Glyphicon = Glyphicon.Pencil)]
        Textbox = 1,
        [FormBuilderControlMetadata(BsControlType = BsControlType.TextArea)]
        [FormBuilderControlDisplay(DisplayName = "Textarea", Glyphicon = Glyphicon.Font)]
        Textarea = 2,
        [FormBuilderControlMetadata(BsControlType = BsControlType.NumberInline)]
        [FormBuilderControlDisplay(DisplayName = "Number picker", Glyphicon = Glyphicon.PlusSign)]
        NumberPicker = 3,
        [FormBuilderControlMetadata(BsControlType = BsControlType.NumberRange)]
        [FormBuilderControlDisplay(DisplayName = "Number picker range", Glyphicon = Glyphicon.Plus)]
        NumberPickerRange = 4,
        [FormBuilderControlDisplay(DisplayName = "Decimal number picker", Glyphicon = Glyphicon.Plus)]
        DecimalNumberPicker = 5,
        [FormBuilderControlMetadata(BsControlType = BsControlType.DatePicker)]
        [FormBuilderControlDisplay(DisplayName = "Date picker", Glyphicon = Glyphicon.Calendar)]
        DatePicker = 6,
        [FormBuilderControlMetadata(BsControlType = BsControlType.DatePickerRange)]
        [FormBuilderControlDisplay(DisplayName = "Date picker range", Glyphicon = Glyphicon.Calendar)]
        DatePickerRange = 7,
        [FormBuilderControlMetadata(BsControlType = BsControlType.TimePicker)]
        [FormBuilderControlDisplay(DisplayName = "Time picker", Glyphicon = Glyphicon.Time)]
        TimePicker = 8,
        [FormBuilderControlMetadata(BsControlType = BsControlType.DropDownList)]
        [FormBuilderControlDisplay(DisplayName = "Single select", Glyphicon = Glyphicon.List)]
        SingleSelect = 9,
        [FormBuilderControlMetadata(BsControlType = BsControlType.TagList)]
        [FormBuilderControlDisplay(DisplayName = "Taglist", Glyphicon = Glyphicon.Tags)]
        TagList = 10,
        [FormBuilderControlMetadata(BsControlType = BsControlType.ListBox)]
        [FormBuilderControlDisplay(DisplayName = "Listbox", Glyphicon = Glyphicon.Tag)]
        ListBox = 11,
        [FormBuilderControlMetadata(BsControlType = BsControlType.RadioButtonList)]
        [FormBuilderControlDisplay(DisplayName = "Radio button list", Glyphicon = Glyphicon.ListAlt)]
        RadioButtonList = 12,
        [FormBuilderControlMetadata(BsControlType = BsControlType.CheckBoxList)]
        [FormBuilderControlDisplay(DisplayName = "Checkbox list", Glyphicon = Glyphicon.Check)]
        CheckboxList = 13,
        [FormBuilderControlMetadata(BsControlType = BsControlType.CheckBox)]
        [FormBuilderControlDisplay(DisplayName = "Checkbox", Glyphicon = Glyphicon.Check)]
        Checkbox = 14,
        [FormBuilderControlDisplay(DisplayName = "Pagebreak", Glyphicon = Glyphicon.LogIn)]
        Pagebreak = 15,
        [FormBuilderControlDisplay(DisplayName = "Title", Glyphicon = Glyphicon.TextWidth)]
        Title = 16,
        [FormBuilderControlDisplay(DisplayName = "Custom control", Glyphicon = Glyphicon.Asterisk)]
        CustomControl = 17,
        [FormBuilderControlDisplay(DisplayName = "File", Glyphicon = Glyphicon.File)]
        File = 18
    }

    public enum ColumnWidth
    {
        [Description("col-lg-4 col-md-4 col-sm-4")]
        [Display(Name = "small")]
        Small = 4,
        [Description("col-lg-6 col-md-6 col-sm-6")]
        [Display(Name = "medium")]
        Medium = 6,
        [Description("col-lg-12 col-md-12 col-sm-12")]
        [Display(Name = "large")]
        Large = 12
    }

    public enum FormBuilderInputType
    {
        [Description("text")]
        Text = 1,
        [Description("email")]
        Email,
        [Description("url")]
        Url,
        [Description("password")]
        Password,
        [Description("search")]
        Search
    }

    public enum TextEditorControlType
    {
        Bold = 1,
        Italic,
        LineThrough,
        Hyperlink,
        Image,
        OrderedList,
        UnorderedList,
        FontColor,
        BackgroundColor
    }

    public enum FormBuilderControlActionType
    {
        [FormBuilderControlDisplay(Name = "all")]
        All = 0,
        [FormBuilderControlDisplay(Name = "grab", Glyphicon = Glyphicon.Sort)]
        Grab,
        [FormBuilderControlDisplay(Name = "up", Glyphicon = Glyphicon.Sort)]
        Up,
        [FormBuilderControlDisplay(Name = "down", Glyphicon = Glyphicon.Sort)]
        Down,
        [FormBuilderControlDisplay(Name = "remove", Glyphicon = Glyphicon.Sort)]
        Remove,
        [FormBuilderControlDisplay(Name = "settings", Glyphicon = Glyphicon.Sort)]
        Settings,

        CustomAction
    }

    public enum YesNoValues
    {
        [Display(Name = "Yes")]
        Yes = 1,
        [Display(Name = "No")]
        No = 2
    }

    public enum BsComponentStatus
    {
        [Description("")]
        None = 0,
        [Description("default")]
        Default = 1,
        [Description("success")]
        Success = 2,
        [Description("info")]
        Info = 3,
        [Description("warning")]
        Warning = 4,
        [Description("danger")]
        Danger = 5,
        [Description("add")]
        Add = 6,
        [Description("primary")]
        Primary = 7
    }

    public enum BsComponentType
    {
        [Description("btn")]
        Button = 1,
        [Description("alert")]
        Alert = 2,
        [Description("label")]
        Label = 3,
        [Description("progress-bar")]
        ProgressBar = 4,
        [Description("list-group-item")]
        ListGroupItem = 5
    }

    #region Enum extensions

    public static class FormBuilderEnumExtensions
    {
        public static FormBuilderControlDisplay GetDisplayAttribute(this FormBuilderControlType type)
        {
            var fieldInfo = typeof(FormBuilderControlType).GetField(type.ToString());

            if (fieldInfo != null)
            {
                var displayAttribute = Attribute.GetCustomAttribute(fieldInfo, typeof(FormBuilderControlDisplay)) as FormBuilderControlDisplay;

                return displayAttribute;
            }

            return null;
        }

        public static FormBuilderControlDisplay GetDisplayAttribute(this FormBuilderControlActionType type)
        {
            var fieldInfo = typeof(FormBuilderControlActionType).GetField(type.ToString());

            if (fieldInfo != null)
            {
                var displayAttribute = Attribute.GetCustomAttribute(fieldInfo, typeof(FormBuilderControlDisplay)) as FormBuilderControlDisplay;

                return displayAttribute;
            }

            return null;
        }

        public static FormBuilderControlMetadata GetMetadataAttribute(this FormBuilderControlType type)
        {
            var fieldInfo = typeof(FormBuilderControlType).GetField(type.ToString());

            if (fieldInfo != null)
            {
                var metadataAttribute = Attribute.GetCustomAttribute(fieldInfo, typeof(FormBuilderControlMetadata)) as FormBuilderControlMetadata;

                return metadataAttribute;
            }

            return null;
        }
    }

    #endregion
}
