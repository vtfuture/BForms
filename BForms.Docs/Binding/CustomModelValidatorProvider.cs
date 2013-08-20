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
                var a = 5;
            }
            if (metadata.ContainerType.IsSubclassOfRawGeneric(typeof(BsSelectList<>)) 
                && metadata.PropertyName == "SelectedValues")
            {
                var parentAttributes =
                    metadata.ContainerType.GetCustomAttributes(true)
                        .Where(r => r.GetType().IsAssignableFrom(typeof(ValidationAttribute)))
                        .ToList();
                var parentMetadata =
                    System.Web.Mvc.ModelMetadataProviders.Current.GetMetadataForProperties(context.Controller.ViewData.Model,
                        metadata.ContainerType);
                foreach (var parentAttribute in parentAttributes)
                {
                    res.Add(new DataAnnotationsModelValidator(metadata, context, parentAttribute as ValidationAttribute));
                }
            }

            return res;
        }
    }
}