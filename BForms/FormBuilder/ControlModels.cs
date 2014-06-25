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

        [FormBuilderPropertiesTab(Glyphicon = Glyphicon.Wrench)]
        [Display(Name = "Default properties")]
        public DefaultcontrolProperties DefaultProperties { get; set; }

        public FormBuilderControl(FormBuilderControlType type = FormBuilderControlType.CustomControl)
        {
            Type = type;
            DefaultProperties = new DefaultcontrolProperties();
        }
    }

    public class InputControlModel : FormBuilderControl
    {
        public InputControlModel() :
            base(FormBuilderControlType.Textbox)
        {
            Properties = new InputControlProperties();

            DefaultProperties.Name = "New.textBox";
            DefaultProperties.Label = "New Textbox";
        }

        [FormBuilderPropertiesTab(Glyphicon = Glyphicon.Cog)]
        [Display(Name = "Input properties")]
        public InputControlProperties Properties { get; set; }
    }

    public class TextAreaControlModel : FormBuilderControl
    {
        public TextAreaControlModel() :
            base(FormBuilderControlType.Textarea)
        {
            DefaultProperties.Name = "New.textArea";
            DefaultProperties.Label = "New Textarea";
        }
    }

    public class SingleSelectControlModel : FormBuilderControl
    {
        public SingleSelectControlModel() :
            base(FormBuilderControlType.SingleSelect)
        {
            Properties = new SingleSelectControlProperties();

            DefaultProperties.Name = "New.singleSelect.SelectedValues";
            DefaultProperties.Label = "New Single select";
        }

       // [FormBuilderPropertiesTab(Glyphicon = Glyphicon.Cog)]
        [Display(Name = "Select properties")]
        public SingleSelectControlProperties Properties { get; set; }
    }

    public class ListBoxControlModel : FormBuilderControl
    {
        public ListBoxControlModel() :
            base(FormBuilderControlType.ListBox)
        {
            Properties = new MultipleSelectControlProperties();

            DefaultProperties.Name = "New.listBox.SelectedValues";
            DefaultProperties.Label = "New Listbox";
        }

       // [FormBuilderPropertiesTab(Glyphicon = Glyphicon.Cog)]
        [Display(Name = "Listbox properties")]
        public MultipleSelectControlProperties Properties { get; set; }
    }

    public class TagListControlModel : FormBuilderControl
    {
        public TagListControlModel() :
            base(FormBuilderControlType.TagList)
        {
            Properties = new MultipleSelectControlProperties();

            DefaultProperties.Name = "New.tagList.SelectedValues";
            DefaultProperties.Label = "New Taglist";
        }

      //  [FormBuilderPropertiesTab(Glyphicon = Glyphicon.Cog)]
        [Display(Name = "Taglist properties")]
        public MultipleSelectControlProperties Properties { get; set; }
    }

    public class NumberPickerControlModel : FormBuilderControl
    {
        public NumberPickerControlModel()
            : base(FormBuilderControlType.NumberPicker)
        {
            Properties = new NumberPickerControlProperties();
        }

        [FormBuilderPropertiesTab(Glyphicon = Glyphicon.Cog)]
        [Display(Name = "Number picker properties")]
        public NumberPickerControlProperties Properties { get; set; }
    }

    public class NumberPickerRangeControlModel : FormBuilderControl
    {
        public NumberPickerRangeControlModel()
            : base(FormBuilderControlType.NumberPickerRange)
        {
            LeftBoundProperties = new NumberPickerControlProperties();
            RightBoundProperties = new NumberPickerControlProperties();
        }

        [FormBuilderPropertiesTab(Glyphicon = Glyphicon.Cog)]
        [Display(Name = "Left bound properties")]
        public NumberPickerControlProperties LeftBoundProperties { get; set; }

        [FormBuilderPropertiesTab(Glyphicon = Glyphicon.Cog)]
        [Display(Name = "Right bound properties")]
        public NumberPickerControlProperties RightBoundProperties { get; set; }
    }

    public class DatePickerControlModel : FormBuilderControl
    {
        public DatePickerControlModel()
            : base(FormBuilderControlType.DatePicker)
        {
            Properties = new DatePickerControlProperties();
        }

        [FormBuilderPropertiesTab(Glyphicon = Glyphicon.Cog)]
        [Display(Name = "Date picker properties")]
        public DatePickerControlProperties Properties { get; set; }
    }

    public class DatePickerRangeControlModel : FormBuilderControl
    {
        public DatePickerRangeControlModel()
            : base(FormBuilderControlType.DatePickerRange)
        {
            LeftBoundProperties = new DatePickerControlProperties();
            RightBoundProperties = new DatePickerControlProperties();
        }

        [FormBuilderPropertiesTab(Glyphicon = Glyphicon.Cog)]
        [Display(Name = "Left bound properties")]
        public DatePickerControlProperties LeftBoundProperties { get; set; }

        [FormBuilderPropertiesTab(Glyphicon = Glyphicon.Cog)]
        [Display(Name = "Right bound properties")]
        public DatePickerControlProperties RightBoundProperties { get; set; }
    }

    public class RadioButtonListControlModel : FormBuilderControl
    {
        public RadioButtonListControlModel()
            : base(FormBuilderControlType.RadioButtonList)
        {
            Properties = new RadioButtonListControlProperties();

            DefaultProperties.Name = "New.radioButtonList.SelectedValues";
            DefaultProperties.Label = "New Radio button list";
        }

       // [FormBuilderPropertiesTab(Glyphicon = Glyphicon.Cog)]
        [Display(Name = "Radio list properties")]
        public RadioButtonListControlProperties Properties { get; set; }
    }

}
