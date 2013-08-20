using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;
using System.Web.Mvc;

using BForms.Docs.Utils;

using BootstrapForms.Models;

using DataAnnotationsModelValidator = System.Web.Mvc.DataAnnotationsModelValidator;
using ModelMetadata = System.Web.Mvc.ModelMetadata;
using ModelValidator = System.Web.Mvc.ModelValidator;
using ModelValidatorProvider = System.Web.Mvc.ModelValidatorProvider;

namespace BForms.Docs.Binding
{
    public class CustomModelValidatorProvider : ModelValidatorProvider
    {
        public override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context)
        {
            var res = new List<ModelValidator>();
            if (metadata == null || metadata.ContainerType == null)
            {
                return res;
            }
            if(metadata.ModelType.IsSubclassOfRawGeneric(typeof(BsSelectList<>)))
            {
                var selectedValuesMetadata = metadata.Properties.Where(r => r.PropertyName == "SelectedValues").FirstOrDefault();
                var myPropInfo = metadata.ContainerType.GetProperties().Where(r => r.Name == metadata.PropertyName).FirstOrDefault();
                if (myPropInfo == null)
                {
                    return res;
                }
                var attributes =
                    myPropInfo.GetCustomAttributes(true)
                        .Where(r => r is ValidationAttribute)
                        .ToList();
                foreach (var attribute in attributes)
                {
                    res.Add(new DataAnnotationsModelValidator(selectedValuesMetadata, context, attribute as ValidationAttribute));
                }
            }
            return res;
        }
    }
}