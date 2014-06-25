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
using BForms.Utilities;

namespace BForms.FormBuilder
{
    public class BaseControlProperties
    {
        public BaseControlProperties()
        {
            Buttons = new List<BsButtonModel>
            {
                new BsButtonModel("Save", BsComponentStatus.Default),
                new BsButtonModel("Reset", BsComponentStatus.Warning)
            };
        }

        [FormButtons]
        public IEnumerable<BsButtonModel> Buttons { get; set; } 
    }

    public class DefaultcontrolProperties : BaseControlProperties
    {
        [BsControl(BsControlType.DropDownList)]
        [FormGroup(ColumnWidth.Large, Glyphicon.ResizeHorizontal)]
        [Display(Name = "Width")]
        public BsSelectList<ColumnWidth> Width { get; set; }

        [BsControl(BsControlType.TextBox)]
        [FormGroup(ColumnWidth.Large, Glyphicon.Tag)]
        [Display(Name = "Label")]
        [Required]
        public string Label { get; set; }

        [BsControl(BsControlType.TextBox)]
        [FormGroup(ColumnWidth.Large, Glyphicon.Pencil)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [BsControl(BsControlType.DropDownList)]
        [FormGroup(ColumnWidth.Large, Glyphicon.User)]
        [Display(Name = "Glyphicon")]
        public BsSelectList<Glyphicon?> GlyphiconAddon { get; set; }

        [BsControl(BsControlType.RadioButtonList)]
        [FormGroup(ColumnWidth.Large, Glyphicon.Asterisk)]
        [Display(Name = "Required")]
        [Required]
        public BsSelectList<YesNoValues> Required { get; set; }

        public DefaultcontrolProperties()
        {
            Width = BsSelectList<ColumnWidth>.FromEnum(typeof(ColumnWidth));

            GlyphiconAddon = new BsSelectList<Glyphicon?>();

            var glyphiconList = new BsSelectList<Glyphicon>();

            glyphiconList.Items = Enum.GetValues(typeof(Glyphicon)).Cast<Glyphicon>()
                .Where(x=> x != Glyphicon.Custom)
                .Select(x => new BsSelectListItem
                {
                    Text = x.ToString(),
                    Value = x.GetDescription()
                }).ToList();

            GlyphiconAddon.Items = glyphiconList.Items;

            GlyphiconAddon.Items.Insert(0, new BsSelectListItem
            {
                Text = "None",
                Value = String.Empty
            });

            Required = BsSelectList<YesNoValues>.FromEnum(typeof(YesNoValues));

            Width.SelectedValues = ColumnWidth.Large;
            Required.SelectedValues = YesNoValues.No;
        }
    }

    public class InputControlProperties : BaseControlProperties
    {
        [BsControl(BsControlType.TextBox)]
        [Display(Name = "Placeholder")]
        [FormGroup(ColumnWidth.Large, Glyphicon.Pushpin)]
        public string Placeholder { get; set; }

        [BsControl(BsControlType.TextBox)]
        [Display(Name = "Initial value")]
        [FormGroup(ColumnWidth.Large, Glyphicon.LogIn)]
        public string InitialValue { get; set; }

        [BsControl(BsControlType.DropDownList)]
        [Display(Name = "Type")]
        [FormGroup(ColumnWidth.Large, Glyphicon.QuestionSign)]
        public BsSelectList<FormBuilderInputType> Type { get; set; }

        [BsControl(BsControlType.TextBox)]
        [Display(Name = "Regex validation")]
        [FormGroup(ColumnWidth.Large, Glyphicon.Asterisk)]
        public string RegexString { get; set; }

        public InputControlProperties()
        {
            Type = BsSelectList<FormBuilderInputType>.FromEnum(typeof (FormBuilderInputType));
        }
    }

    public class TextAreaControlProperties : BaseControlProperties
    {
        [BsControl(BsControlType.TextBox)]
        [Display(Name = "Initial value")]
        [FormGroup(ColumnWidth.Large, Glyphicon.LogIn)]
        public string InitialValue { get; set; }
    }

    public class SelectControlBaseProperties : BaseControlProperties
    {
        [BsControl(BsControlType.TagList)]
        [Display(Name = "Items")]
        [Required]
        public BsSelectList<List<string>> Items { get; set; } 

        public SelectControlBaseProperties()
        {
            Items = new BsSelectList<List<string>>();

            //Items.Items.Add(new BsSelectListItem
            //{
            //    Text = "A",
            //    Value = "A"
            //});

            //Items.Items.Add(new BsSelectListItem
            //{
            //    Text = "B",
            //    Value = "B"
            //});

            //Items.SelectedValues = Items.Items.Select(x => x.Value).ToList();
        }
    }

    public class SingleSelectControlProperties : SelectControlBaseProperties
    {
        //[BsControl(BsControlType.DropDownList)]
        [Display(Name = "Initial value")]
        public BsSelectList<string> InitialValue { get; set; }

        public SingleSelectControlProperties()
        {
            InitialValue = new BsSelectList<string>();

            InitialValue.Items = new List<BsSelectListItem>(Items.Items);
        }
    }

    public class MultipleSelectControlProperties : SelectControlBaseProperties
    {
       // [BsControl(BsControlType.ListBox)]
        [Display(Name = "Initial values")]
        public BsSelectList<List<string>> InitialValues { get; set; }

        public MultipleSelectControlProperties()
        {
            InitialValues = new BsSelectList<List<string>>();

            InitialValues.Items = new List<BsSelectListItem>(Items.Items);
        }
    }

    public class RadioButtonListControlProperties : SelectControlBaseProperties
    {
       // [BsControl(BsControlType.RadioButtonList)]
        [Display(Name = "Initial value")]
        public BsSelectList<int> InitialValue { get; set; }

        public RadioButtonListControlProperties()
        {
            InitialValue = new BsSelectList<int>();

            InitialValue.Items = new List<BsSelectListItem>(Items.Items);
        }
    }

    public class CheckBoxListControlProperties : SelectControlBaseProperties
    {
        [BsControl(BsControlType.CheckBoxList)]
        [Display(Name = "Initial values")]
        public BsSelectList<List<int>> InitialValues { get; set; }

        public CheckBoxListControlProperties()
        {
            InitialValues = new BsSelectList<List<int>>();

            InitialValues.Items = new List<BsSelectListItem>(Items.Items);
        }
    }

    public class DatePickerControlProperties
    {
        [BsControl(BsControlType.DatePicker)]
        [Display(Name = "Initial value")]
        public BsDateTime InitialValue { get; set; }

        [BsControl(BsControlType.DatePicker)]
        [Display(Name = "Min value")]
        public BsDateTime MinValue { get; set; }

        [BsControl(BsControlType.DatePicker)]
        [Display(Name = "Max value")]
        public BsDateTime MaxValue { get; set; }

        [BsControl(BsControlType.TextBox)]
        [Display(Name = "Null value display")]
        public string NullValueDisplay { get; set; }

        public DatePickerControlProperties()
        {
            InitialValue = new BsDateTime();
            MinValue = new BsDateTime();
            MaxValue = new BsDateTime();
        }
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

        public NumberPickerControlProperties()
        {
            InitialValue = new BsRangeItem<int>();
            MinValue = new BsRangeItem<int>();
            MaxValue = new BsRangeItem<int>();
        }
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
