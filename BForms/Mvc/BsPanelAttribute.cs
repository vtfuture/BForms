using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BForms.Mvc
{
    [AttributeUsage(AttributeTargets.Property)]
    public class BsPanelAttribute :Attribute
    {
        public bool IsLarge { get; set; }

        public object Id { get; set; }

        private bool _isExpandable = true;

        private bool _isEditable = true;

        public bool Expandable
        {
            get
            {
                return _isExpandable;
            }

            set
            {
                _isExpandable = value;
            }
        }

        public bool Editable
        {
            get
            {
                return _isEditable;
            }
            set
            {
                _isEditable = value;
            }
        }
    }
}
