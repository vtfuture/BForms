using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.ComponentModel;
using BForms.Models;

namespace BForms.FormBuilder
{
    public enum FormBuilderControlType
    {
        [FormBuilderControlDisplay(DisplayName = "Textbox", Glyphicon = Glyphicon.Pencil)]
        Textbox = 1,
        [FormBuilderControlDisplay(DisplayName = "Textarea", Glyphicon = Glyphicon.Font)]
        Textarea,
        [FormBuilderControlDisplay(DisplayName = "Number picker", Glyphicon = Glyphicon.PlusSign)]
        NumberPicker,
        [FormBuilderControlDisplay(DisplayName = "Number picker range", Glyphicon = Glyphicon.Plus)]
        NumberPickerRange,
        [FormBuilderControlDisplay(DisplayName = "Decimal number picker", Glyphicon = Glyphicon.Plus)]
        DecimalNumberPicker,
        [FormBuilderControlDisplay(DisplayName = "Date picker", Glyphicon = Glyphicon.Calendar)]
        DatePicker,
        [FormBuilderControlDisplay(DisplayName = "Date picker range", Glyphicon = Glyphicon.Calendar)]
        DatePickerRange,
        [FormBuilderControlDisplay(DisplayName = "Time picker", Glyphicon = Glyphicon.Time)]
        TimePicker,
        [FormBuilderControlDisplay(DisplayName = "Single select", Glyphicon = Glyphicon.List)]
        SingleSelect,
        [FormBuilderControlDisplay(DisplayName = "Taglist", Glyphicon = Glyphicon.Tags)]
        TagList,
        [FormBuilderControlDisplay(DisplayName = "Listbox", Glyphicon = Glyphicon.Tag)]
        ListBox,
        [FormBuilderControlDisplay(DisplayName = "Radio button list", Glyphicon = Glyphicon.ListAlt)]
        RadioButtonList,
        [FormBuilderControlDisplay(DisplayName = "Checkbox list", Glyphicon = Glyphicon.Check)]
        CheckboxList,
        [FormBuilderControlDisplay(DisplayName = "Checkbox", Glyphicon = Glyphicon.Check)]
        Checkbox,
        [FormBuilderControlDisplay(DisplayName = "Pagebreak", Glyphicon = Glyphicon.LogIn)]
        Pagebreak,
        [FormBuilderControlDisplay(DisplayName = "Title", Glyphicon = Glyphicon.TextWidth)]
        Title,
        [FormBuilderControlDisplay(DisplayName = "Custom control", Glyphicon = Glyphicon.Asterisk)]
        CustomControl
    }
    
    public enum TextboxType
    {
        Text = 1,
        Email,
        Url
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

    public static class FormBuilderEnumExtensions
    {
        public static FormBuilderControlDisplay GetDisplayAttribute(this FormBuilderControlType type)
        {
            var fieldInfo = typeof (FormBuilderControlType).GetField(type.ToString());

            if (fieldInfo != null)
            {
                var displayAttribute = Attribute.GetCustomAttribute(fieldInfo, typeof (FormBuilderControlDisplay)) as FormBuilderControlDisplay;

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
    }
}
