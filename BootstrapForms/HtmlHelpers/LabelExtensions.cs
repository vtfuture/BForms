using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Mvc.Html;
using BootstrapForms.HtmlHelpers;

namespace BootstrapForms.Razor
{
    /// <summary>
    /// Represents support for the bootstrap HTML label element
    /// </summary>
    public static class LabelExtensions
    {
        /// <summary>
        /// Adds to LabelFor bootstrap css and the required class
        /// </summary>
        public static MvcHtmlString BsLabelFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            string fieldName = ExpressionHelper.GetExpressionText(expression);
            string labelText = metadata.DisplayName ?? metadata.PropertyName ?? fieldName.Split('.').Last();

            //determine if the prop is decorated with Required
            var model = typeof(TModel);
            var property = model.GetProperty(fieldName);
            var isRequired = Attribute.IsDefined(property, typeof(System.ComponentModel.DataAnnotations.RequiredAttribute));

            //merge custom css classes with bootstrap
            htmlAttributes.BsMergeAttribute("class", "control-label");
            if (isRequired)
            {
                htmlAttributes.BsMergeAttribute("class", "required");
            }

            return helper.LabelFor(expression, htmlAttributes);
        }
        public static MvcHtmlString BsLabelFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression)
        {
            return BsLabelFor(helper, expression, (object)null);
        }
        public static MvcHtmlString BsLabelFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return BsLabelFor(helper, expression, new RouteValueDictionary(htmlAttributes));
        }
    }
}