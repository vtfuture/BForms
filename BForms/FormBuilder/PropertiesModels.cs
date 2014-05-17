using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BForms.Grid;
using BForms.Models;
using System.Text.RegularExpressions;
using BForms.Mvc;

namespace BForms.FormBuilder
{

    public class DefaultcontrolProperties
    {
        [BsControl(BsControlType.DropDownList)]
        [Display(Name = "Width")]
        public BsSelectList<ColumnWidth> Width { get; set; }

        [BsControl(BsControlType.TextBox)]
        [Display(Name = "Label")]
        public string Label { get; set; }

        [BsControl(BsControlType.TextBox)]
        [Display(Name = "Name")]
        public string Name { get; set; }
    }

    public class InputControlProperties
    {
        [BsControl(BsControlType.TextBox)]
        [Display(Name = "Placeholder")]
        public string Placeholder { get; set; }

        [BsControl(BsControlType.TextBox)]
        [Display(Name = "Initial value")]
        public string InitialValue { get; set; }

        [BsControl(BsControlType.DropDownList)]
        [Display(Name = "Type")]
        public BsSelectList<FormBuilderInputType> Type { get; set; }

        [BsControl(BsControlType.TextBox)]
        [Display(Name = "Regex validation")]
        public string RegexString { get; set; }
    }

    public class TextAreaControlProperties
    {
        [BsControl(BsControlType.TextBox)]
        [Display(Name = "Initial value")]
        public string InitialValue { get; set; }
    }
    
    public class SelectControlBaseProperties
    {
        [BsControl(BsControlType.TagList)]
        [Display(Name = "Items")]
        [Required]
        public BsSelectList<List<string>> Items { get; set; } 
    }

    public class SingleSelectControlProperties : SelectControlBaseProperties
    {
        [BsControl(BsControlType.DropDownList)]
        [Display(Name = "Initial value")]
        public BsSelectList<string> InitialValue { get; set; }
    }

    public class MultipleSelectControlProperties : SelectControlBaseProperties
    {
        [BsControl(BsControlType.ListBox)]
        [Display(Name = "Initial values")]
        public BsSelectList<List<string>> InitialValues { get; set; }
    }

    public class RadioButtonListControlProperties : SelectControlBaseProperties
    {
        [BsControl(BsControlType.RadioButtonList)]
        [Display(Name = "Initial value")]
        public BsSelectList<List<int>> InitialValue { get; set; }
    }

    public class CheckBoxListControlProperties : SelectControlBaseProperties
    {
        [BsControl(BsControlType.CheckBoxList)]
        [Display(Name = "Initial values")]
        public BsSelectList<List<int>> InitialValues { get; set; }
    }

    public class DatePickerControlProperties
    {
        [BsControl(BsControlType.DatePicker)]
        [Display(Name = "Initial value")]
        public BsDateTime InitialValue { get; set; }

        [BsControl(BsControlType.DatePicker)]
        [Display(Name = "Max value")]
        public BsDateTime MinValue { get; set; }

        [BsControl(BsControlType.DatePicker)]
        [Display(Name = "Min value")]
        public BsDateTime MaxValue { get; set; }

        [BsControl(BsControlType.TextBox)]
        [Display(Name = "Null value display")]
        public string NullValueDisplay { get; set; }
    }

    public class NumberPickerControlProperties
    {
        [BsControl(BsControlType.NumberInline)]
        [Display(Name = "Initial value")]
        public BsRangeItem<int> InitialValue { get; set; }

        [BsControl(BsControlType.NumberInline)]
        [Display(Name = "Min value")]
        public BsRangeItem<int> MinValue { get; set; }
        
        [BsControl(BsControlType.NumberInline)]
        [Display(Name = "Max value")]
        public BsRangeItem<int> MaxValue { get; set; }

        [BsControl(BsControlType.TextBox)]
        [Display(Name = "Zero value display")]
        public string ZeroValueDisplay { get; set; }
    }

    public class FileControlProperties
    {
        [BsControl(BsControlType.NumberInline)]
        [Display(Name = "Max file size")]
        public int MaxSize { get; set; }
        
        [BsControl(BsControlType.TagList)]
        [Display(Name = "Accepted extensions")]
        [Required]
        public BsSelectList<List<string>> AcceptedExtensions { get; set; }

        [BsControl(BsControlType.RadioButtonList)]
        [Display(Name = "Allow multiple files")]
        public BsSelectList<YesNoValues> AllowMultiple { get; set; }

        [BsControl(BsControlType.NumberInline)]
        [Display(Name = "Max allowed files")]
        public int MaxAllowedFiles { get; set; } 
    }

    public class TitleControlProperties
    {
        [BsControl(BsControlType.TextBox)]
        [Display(Name = "Text")]
        public string Text { get; set; }
    }

}
