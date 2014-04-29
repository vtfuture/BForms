using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BForms.Models;

namespace BForms.FormBuilder
{
    public class FormBuilderControlViewModel
    {
        public FormBuilderControlType Type { get; set; }
        public string Text { get; set; }
        public Glyphicon Glyphicon { get; set; }
        public int Order { get; set; }
    }
}
