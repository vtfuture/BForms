using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using BForms.Models;

namespace BForms.FormBuilder
{
    #region Base control types

    public abstract class FormBuilderControl
    {
        public FormBuilderControlType ControlType { get; set; }
    }

    public abstract class FormBuilderControl<TProperties, TConstraints> : FormBuilderControl where TProperties : FormBuilderControlProperties
                                                                                    where TConstraints : FormBuilderControlConstraints
    {
        public TProperties Properties { get; set; }
        public TConstraints Constraints { get; set; }
    }

    public class FormBuilderInputControl<TValue> : FormBuilderControl<FormBuilderInputControlProperties<TValue>, FormBuilderInputControlConstraints<TValue>>
    {
        public IComparer<TValue> ValueComparer { get; set; }
        public Func<TValue, int> SizeDelegate { get; set; }
    }

    public class FormBuilderNumberControl<TValue> : FormBuilderControl<FormBuilderNumberControlProperties<TValue>, FormBuilderNumberControlConstraints<TValue>>
    {

    }

    public class FormBuilderSelectControl<TValue> : FormBuilderControl<FormBuilderSelectControlProperties<TValue>, FormBuilderSelectControlConstraints<TValue>>
    {
        public IEnumerable<BsSelectListItem> Items { get; set; }

        public TValue DefaultValue { get; set; }
        public TValue SelectedValue { get; set; }
    }

    #endregion

    #region Specialized controls

    public class FormBuilderTextboxControl : FormBuilderInputControl<string>
    {
        public TextboxType TextboxType { get; set; }

        public FormBuilderTextboxControl()
        {
            ControlType = FormBuilderControlType.Textbox;

            Properties = new FormBuilderInputControlProperties<string>
            {
                Type = BsControlType.TextBox,
                Width = 12
            };

            Constraints = new FormBuilderInputControlConstraints<string>();
        }
    }

    public class FormBuilderTextareaControl : FormBuilderInputControl<string>
    {
        public FormBuilderTextareaControl()
        {
            ControlType = FormBuilderControlType.Textarea;

            Properties = new FormBuilderInputControlProperties<string>
            {
                Type = BsControlType.TextArea,
                Width = 12
            };

            Constraints = new FormBuilderInputControlConstraints<string>();
        }
    }

    public class FormBuilderNumberPickerControl : FormBuilderNumberControl<long>
    {
        public FormBuilderNumberPickerControl()
        {
            ControlType = FormBuilderControlType.NumberPicker;

            Properties = new FormBuilderNumberControlProperties<long>
            {
                Type = BsControlType.Number,
                TextValue = String.Empty,
                DefaultTextValue = String.Empty
            };

            Constraints = new FormBuilderNumberControlConstraints<long>();
        }
    }

    public class FormBuilderDecimalNumberPickerControl : FormBuilderNumberControl<double>
    {
        public FormBuilderDecimalNumberPickerControl()
        {
            ControlType = FormBuilderControlType.DecimalNumberPicker;

            Properties = new FormBuilderNumberControlProperties<double>
            {
                Type = BsControlType.Number,
                TextValue = String.Empty,
                DefaultTextValue = String.Empty
            };

            Constraints = new FormBuilderNumberControlConstraints<double>();
        }
    }

    public class FormBuilderDatepickerControl : FormBuilderControl<FormBuilderDatepickerControlProperties, FormBuilderDatepickerControlConstraints>
    {

    }

    public class FormBuilderFileControl : FormBuilderControl<FormBuilderFileControlProperties, FormBuilderFileControlConstraints>
    {

    }

    public class FormBuilderTextEditorControl : FormBuilderControl<FormBuilderTextEditorControlProperties, FormBuilderTextEditorControlConstraints>
    {

    }

    #region Void controls

    public class FormBuilderWizardPagebreak : FormBuilderControl
    {

    }

    public class FormBuilderTitle : FormBuilderControl
    {
        public string Title { get; set; }
        public Glyphicon Glyphicon { get; set; }
    }

    #endregion

    #endregion


}
