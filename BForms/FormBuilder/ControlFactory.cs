using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BForms.FormBuilder
{
    public class FormBuilderControlFactory
    {
        private Dictionary<FormBuilderControlType, Type> _controlTypes;

        public FormBuilderControlFactory()
        {
            _controlTypes = new Dictionary<FormBuilderControlType, Type>
            {
                {FormBuilderControlType.Textbox, typeof(FormBuilderTextboxControl)}
            };
        }

        public FormBuilderControl Create(FormBuilderControlType type)
        {
            if(!_controlTypes.ContainsKey(type))
            {
                throw new UnmappedFormBuilderControlException("The given type does not match any FormBuilder control");
            }

            var controlType = _controlTypes[type];

            var instance = controlType.TypeInitializer.Invoke(null);

            return instance as FormBuilderControl;
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
