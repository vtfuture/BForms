using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BForms.FormBuilder
{
    public enum FormBuilderControlType
    {
        Textbox = 1,
        Textarea,
        NumberPicker,
        DecimalNumberPicker,
        DatePicker,
        TimePicker,
        SingleSelect,
        MultipleSelect,
        RadioButtonList,
        Checkbox,
        Pagebreak,
        Title
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
}
