using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BForms.Models;

namespace BForms.FormBuilder
{
    public class FormBuilderControl
    {
        public FormBuilderControlType Type { get; set; }
        public string Name { get; set; }

        public FormBuilderControl(FormBuilderControlType type)
        {
            Type = type;
        }
    }

    public class InputControlModel : FormBuilderControl
    {
        public InputControlModel() :
            base(FormBuilderControlType.Textbox)
        {
            Properties = new InputControlProperties
            {
                Type = BsSelectList<FormBuilderInputType>.FromEnum(typeof (FormBuilderInputType))
            };
        }

        [FormBuilderPropertiesTab]
        [Display(Name = "Properties")]
        public InputControlProperties Properties { get; set; }
    }

    public class SingleSelectControlModel : FormBuilderControl
    {
        public SingleSelectControlModel() :
            base(FormBuilderControlType.SingleSelect)
        {

        }

        [FormBuilderPropertiesTab]
        [Display(Name = "Properties")]
        public SingleSelectControlProperties Properties { get; set; }
    }

    public class ListBoxControlModel : FormBuilderControl
    {
        public ListBoxControlModel() :
            base(FormBuilderControlType.ListBox)
        {

        }

        [FormBuilderPropertiesTab]
        [Display(Name = "Properties")]
        public MultipleSelectControlProperties Properties { get; set; }
    }

    public class TagListControlModel : FormBuilderControl
    {
        public TagListControlModel() :
            base(FormBuilderControlType.TagList)
        {

        }

        [FormBuilderPropertiesTab]
        [Display(Name = "Properties")]
        public MultipleSelectControlProperties Properties { get; set; }
    }

    public class NumberPickerControlModel : FormBuilderControl
    {
        public NumberPickerControlModel()
            : base(FormBuilderControlType.NumberPicker)
        {

        }

        [FormBuilderPropertiesTab]
        [Display(Name = "Properties")]
        public NumberPickerControlProperties Properties { get; set; }
    }

    public class NumberPickerRangeControlModel : FormBuilderControl
    {
        public NumberPickerRangeControlModel()
            : base(FormBuilderControlType.NumberPickerRange)
        {

        }

        [FormBuilderPropertiesTab]
        [Display(Name = "Left bound properties")]
        public NumberPickerControlProperties LeftBoundProperties { get; set; }

        [FormBuilderPropertiesTab]
        [Display(Name = "Right bound properties")]
        public NumberPickerControlProperties RightBoundProperties { get; set; }
    }

    public class DatePickerControlModel : FormBuilderControl
    {
        public DatePickerControlModel()
            : base(FormBuilderControlType.DatePicker)
        {

        }

        [FormBuilderPropertiesTab]
        [Display(Name = "Properties")]
        public DatePickerControlProperties Properties { get; set; }
    }

    public class DatePickerRangeControlModel : FormBuilderControl
    {
        public DatePickerRangeControlModel()
            : base(FormBuilderControlType.DatePickerRange)
        {

        }

        [FormBuilderPropertiesTab]
        [Display(Name = "Left bound properties")]
        public DatePickerControlProperties LeftBoundProperties { get; set; }

        [FormBuilderPropertiesTab]
        [Display(Name = "Right bound properties")]
        public DatePickerControlProperties RightBoundProperties { get; set; }
    }

    public class RadioButtonListControlModel : FormBuilderControl
    {
        public RadioButtonListControlModel()
            : base(FormBuilderControlType.RadioButtonList)
        {

        }

        public RadioButtonListControlProperties Properties { get; set; }
    }

}
