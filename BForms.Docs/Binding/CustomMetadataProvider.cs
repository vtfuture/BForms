using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using BForms.Docs.Utils;

using BootstrapForms.Models;

namespace BForms.Docs.Binding
{
    public class CustomMetadataProvider : DataAnnotationsModelMetadataProvider
    {
        protected override ModelMetadata CreateMetadata(IEnumerable<Attribute> attributes, Type containerType, Func<object> modelAccessor, Type modelType, string propertyName)
        {
            return base.CreateMetadata(attributes, containerType, modelAccessor, modelType, propertyName);
        }

        protected override ModelMetadata GetMetadataForProperty(Func<object> modelAccessor, Type containerType, PropertyDescriptor propertyDescriptor)
        {
            return base.GetMetadataForProperty(modelAccessor, containerType, propertyDescriptor);
        }
    }
}