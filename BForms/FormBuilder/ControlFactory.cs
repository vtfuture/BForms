using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BForms.Models;
using BForms.Utilities;

namespace BForms.FormBuilder
{
    public class FormBuilderControlFactory
    {
        public FormBuilderControlFactory()
        {

        }

        public FormBuilderControlViewModel Create(FormBuilderControlType type, int tabId = 0)
        {
            var displayAttribute = type.GetDisplayAttribute();

            var control = new FormBuilderControlViewModel
            {
                Glyphicon = displayAttribute.Glyphicon,
                Text = displayAttribute.DisplayName,
                Type = type,
                TabId = tabId
            };

            return control;
        }
    }

    public class UnmappedFormBuilderControlException : Exception 
    {
        public UnmappedFormBuilderControlException(string message)
            : base(message)
        {

        }
    }
}
