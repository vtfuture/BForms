using System;
using System.Linq.Expressions;
using BForms.Models;

namespace BForms.FormBuilder
{
    public class FormBuilderControlDisplay : Attribute
    {
        public string DisplayName { get; set; }
        public Glyphicon Glyphicon { get; set; }
        public string Name { get; set; }
    }

    public class FormBuilderControlMetadata : Attribute
    {
        public BsControlType BsControlType { get; set; }
    }

    public class FormBuilderPropertiesTab : Attribute
    {
        public Glyphicon Glyphicon { get; set; }
    }

    public class FormBuilderControlProperty : Attribute
    {
        public FormBuilderControlType ControlType { get; set; }
    }

    public class FormGroupWidth
    {
        public ColumnWidth Width { get; set; }
    }
}
