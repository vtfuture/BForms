using System;
using BForms.Models;

namespace BForms.FormBuilder
{
    public class FormBuilderControlDisplay : Attribute
    {
        public string DisplayName { get; set; }
        public Glyphicon Glyphicon { get; set; }
        public string Name { get; set; }
    }
}
