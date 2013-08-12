using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using BootstrapForms.Utilities;

namespace BootstrapForms.HtmlHelpers
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
            //determine if the prop is decorated with Required
            var model = typeof(TModel);
            PropertyInfo property = null;
            string fieldName = ExpressionHelper.GetExpressionText(expression);

            foreach (var prop in fieldName.Split('.'))
            {
                property = model.GetProperty(prop);
                model = property.PropertyType;
            }
            var isRequired = Attribute.IsDefined(property, typeof(System.ComponentModel.DataAnnotations.RequiredAttribute));

            //merge custom css classes with bootstrap
            htmlAttributes.MergeAttribute("class", "control-label");
            if (isRequired)
            {
                htmlAttributes.MergeAttribute("class", "required");
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