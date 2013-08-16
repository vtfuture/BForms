using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BootstrapForms.Models
{
    /// <summary>
    /// BForms input elements
    /// </summary>
    public enum BsControlType
    {
        /// <summary>
        /// Text input element
        /// </summary>
        [Description("bs-text")]
        TextBox,
        /// <summary>
        /// Textarea input element
        /// </summary>
        [Description("bs-textarea")]
        TextArea,
        /// <summary>
        /// Password input element
        /// </summary>
        [Description("bs-password")]
        Password,
        /// <summary>
        /// Typeahead input element
        /// </summary>
        [Description("bs-autocomplete")]
        Autocomplete,
        /// <summary>
        /// Url input element
        /// </summary>
        [Description("bs-url")]
        Url,
        /// <summary>
        /// Email input element
        /// </summary>
        [Description("bs-email")]
        Email,
        /// <summary>
        /// Number input element
        /// </summary>
        [Description("bs-number")]
        Number,
        /// <summary>
        /// Number range input element
        /// </summary>
        [Description("bs-number-range")]
        NumberRange,
        /// <summary>
        /// Datepicker input element
        /// </summary>
        [Description("bs-date")]
        DatePicker,
        /// <summary>
        /// Datepicker range input element
        /// </summary>
        [Description("bs-date-range")]
        DatePickerRange,
        /// <summary>
        /// Time input element
        /// </summary>
        [Description("bs-time")]
        TimePicker,
        /// <summary>
        /// Checkbox input element
        /// </summary>
        [Description("bs-checkbox")]
        CheckBox,
        /// <summary>
        /// Checkbox list input element
        /// </summary>
        [Description("bs-checkbox-list")]
        CheckBoxList,
        /// <summary>
        /// Radio button input element
        /// </summary>
        [Description("bs-radio")]
        RadioButton,
        /// <summary>
        /// Radio button list input element
        /// </summary>
        [Description("bs-radio-list")]
        RadioButtonList,
        /// <summary>
        /// Tag list input element
        /// </summary>
        [Description("bs-tag-list")]
        TagList,
        /// <summary>
        /// Drop-down list input element
        /// </summary>
        [Description("bs-dropdown")]
        DropDownList,
        /// <summary>
        /// Grouped drop-down list input element
        /// </summary>
        [Description("bs-dropdown-grouped")]
        DropDownListGrouped,
        /// <summary>
        /// Listbox input element
        /// </summary>
        [Description("bs-listbox")]
        ListBox,
        /// <summary>
        /// Grouped listbox input element
        /// </summary>
        [Description("bs-listbox-grouped")]
        ListBoxGrouped,
        /// <summary>
        /// File upload input element
        /// </summary>
        [Description("bs-file")]
        Upload,
        /// <summary>
        /// Multi-file upload input element
        /// </summary>
        [Description("bs-file-list")]
        UploadList,
        /// <summary>
        /// File upload input element
        /// </summary>
        [Description("bs-color")]
        ColorPicker
    }

}
