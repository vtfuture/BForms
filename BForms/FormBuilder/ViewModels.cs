using System.Collections;
using System.Collections.Generic;
using BForms.Models;

namespace BForms.FormBuilder
{
    public class FormBuilderControlViewModel
    {
        public FormBuilderControlViewModel()
        {
            // Actions = new List<FormBuilderControlActionType> { FormBuilderControlActionType.All };
        }

        public FormBuilderControlType Type { get; set; }
        public string Text { get; set; }
        public Glyphicon Glyphicon { get; set; }
        public int Order { get; set; }
        public int TabId { get; set; }
        public string ControlName { get; set; }
        public List<FormBuilderControlActionType> Actions { get; set; }
    }
}
