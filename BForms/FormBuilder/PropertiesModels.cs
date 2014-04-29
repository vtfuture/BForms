using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BForms.Models;
using System.Text.RegularExpressions;

namespace BForms.FormBuilder
{

    #region Base control properties
    public class FormBuilderControlProperties
    {
        public string Label { get; set; }
        public BsControlType Type { get; set; }
        public Glyphicon Glyphicon { get; set; }
        public string Description { get; set; }
        public int Width { get; set; }
    }

    public class FormBuilderControlDefaultProperties<T> : FormBuilderControlProperties
    {
        public T DefaultValue { get; set; }
        public T SelectedValue { get; set; }
    }

    public class FormBuilderInputControlProperties<T> : FormBuilderControlDefaultProperties<T>
    {
        public string Placeholder { get; set; }
    }

    public class FormBuilderNumberControlProperties<T> : FormBuilderControlDefaultProperties<T>
    {
        public string DefaultTextValue { get; set; }
        public string TextValue { get; set; }
    }

    public class FormBuilderSelectControlProperties<T> : FormBuilderControlDefaultProperties<T>
    {
        public IEnumerable<BsSelectListItem> Items { get; set; }
        public bool AllowChoiceCreation { get; set; }
    }

    #endregion

    #region Specialized control properties

    public class FormBuilderDatepickerControlProperties : FormBuilderControlDefaultProperties<DateTime>
    {

    }

    public class FormBuilderFileControlProperties : FormBuilderControlProperties
    {

    }

    public class FormBuilderTextEditorControlProperties : FormBuilderControlDefaultProperties<string>
    {
        public IEnumerable<TextEditorControlType> Controls { get; set; }
    }

    #endregion
}
