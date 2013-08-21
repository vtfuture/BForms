using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using BootstrapForms.Utilities;

namespace BootstrapForms.Html
{
    /// <summary>
    /// Represents support for the bootstrap HTML label element
    /// </summary>
    public static class LabelExtensions
    {
        /// <summary>
        /// Returns a label element with required css class
        /// </summary>
        public static MvcHtmlString BsLabelFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression)
        {
            return BsLabelFor(htmlHelper, expression, (object) null);
        }

        /// <summary>
        /// Returns a label element with required css class
        /// </summary>
        public static MvcHtmlString BsLabelFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return BsLabelFor(htmlHelper, expression, new RouteValueDictionary(htmlAttributes));
        }

        /// <summary>
        /// Returns a label element with required css class
        /// </summary>
        public static MvcHtmlString BsLabelFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            //merge custom css classes with bootstrap
            htmlAttributes.MergeAttribute("class", "control-label");

            //determine if the prop is decorated with Required
            var model = typeof (TModel);
            PropertyInfo property = null;
            var propertyName = ExpressionHelper.GetExpressionText(expression);

            foreach (var prop in propertyName.Split('.'))
            {
                property = model.GetProperty(prop);
                model = property.PropertyType;
            }
            if (property != null)
            {
                var isRequired = Attribute.IsDefined(property,
                    typeof (System.ComponentModel.DataAnnotations.RequiredAttribute));

                if (isRequired)
                {
                    htmlAttributes.MergeAttribute("class", "required");
                }
            }

            
            var name = htmlHelper.AttributeEncode(htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(propertyName));
            if (typeof(TProperty).FullName.Contains("BsSelectList"))
            {
                name += ".SelectedValues";
            }

            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var labelText = metadata.DisplayName ?? metadata.PropertyName ?? propertyName.Split('.').Last();

            return htmlHelper.Label(name, labelText, htmlAttributes);
        }
    }
}