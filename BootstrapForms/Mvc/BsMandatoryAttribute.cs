using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BootstrapForms.Mvc
{
    /// <summary>
    /// The Mandatory attribute should be used for checkbox fields when the checked state is required
    /// </summary>
    public class BsMandatoryAttribute : ValidationAttribute, IClientValidatable
    {
        /// <summary>
        /// Returns true if the object is of type bool and it's set to true
        /// </summary>
        public override bool IsValid(object value)
        {
            //handle only bool? and bool

            if (value.GetType() == typeof(Nullable<bool>))
            {
                return ((bool?)value).Value;
            }

            if (value.GetType() == typeof(bool))
            {
                return (bool)value;
            }

            throw new ArgumentException("The object must be of type bool or nullable bool", "value");
        }

        public override string FormatErrorMessage(string name)
        {
            return "The " + name + " field is mandatory.";
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessage,
                ValidationType = "mandatory"
            };
        }
    }
}
