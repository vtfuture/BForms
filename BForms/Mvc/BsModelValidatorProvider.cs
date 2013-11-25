using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using BForms.Models;
using BForms.Utilities;
using DataAnnotationsModelValidator = System.Web.Mvc.DataAnnotationsModelValidator;
using ModelMetadata = System.Web.Mvc.ModelMetadata;
using ModelValidator = System.Web.Mvc.ModelValidator;
using ModelValidatorProvider = System.Web.Mvc.ModelValidatorProvider;

namespace BForms.Mvc
{
    /// <summary>
    /// Custom validator provider for BsSelectList fields
    /// </summary>
    public class BsModelValidatorProvider : ModelValidatorProvider
    {
        /// <summary>
        /// Custom validation for BsSelectList&lt;T&gt;
        /// </summary>
        public override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context)
        {
            var res = new List<ModelValidator>();
            if (metadata == null || metadata.ContainerType == null)
            {
                return res;
            }

            if (metadata.ModelType.IsSubclassOfRawGeneric(typeof(BsSelectList<>)))
            {
                var selectedValuesMetadata = metadata.Properties.Where(r => r.PropertyName == "SelectedValues").FirstOrDefault();
                var propertyInfo = metadata.ContainerType.GetProperties().Where(r => r.Name == metadata.PropertyName).FirstOrDefault();

                if (propertyInfo == null)
                {
                    return res;
                }

                //get validation attributes for parent
                var attributes =
                    propertyInfo.GetCustomAttributes(true)
                        .Where(r => r is ValidationAttribute)
                        .ToList();

                //copy validation meta to parent from SelectedValues
                foreach (var attribute in attributes)
                {
                    res.Add(new DataAnnotationsModelValidator(selectedValuesMetadata, context, attribute as ValidationAttribute));
                }
            }

            if (metadata.ModelType.IsSubclassOfRawGeneric(typeof(BsRange<>)) || metadata.ModelType == typeof(BsDateTime))
            {
                var selectedValuesMetadata = metadata.Properties.Where(r => r.PropertyName == "TextValue").FirstOrDefault();
                var propertyInfo = metadata.ContainerType.GetProperties().Where(r => r.Name == metadata.PropertyName).FirstOrDefault();

                if (propertyInfo == null)
                {
                    return res;
                }

                //get validation attributes for parent
                var attributes =
                    propertyInfo.GetCustomAttributes(true)
                        .Where(r => r is ValidationAttribute)
                        .ToList();

                //copy validation meta to parent from TextValue
                foreach (var attribute in attributes)
                {
                    res.Add(new DataAnnotationsModelValidator(selectedValuesMetadata, context, attribute as ValidationAttribute));
                }
            }
            return res;
        }
    }
}
