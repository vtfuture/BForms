using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace BootstrapForms.HtmlHelpers
{
    /// <summary>
    /// Represents helpers for bootstrap forms
    /// </summary>
    public static class HtmlExtensions
    {
        /// <summary>
        /// Replacer for Html.Partial when dealing with nested models
        /// </summary>
        /// <example>
        /// @Html.PartialPrefixed(m => m.Client.Address, "Client/_Address", Model)
        /// </example>
        public static MvcHtmlString BsPartialPrefixed<TModel, TNewModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TNewModel, TProperty>> expression, string partialViewName, TNewModel model, string additionalPrefix = "")
        {
            var propertyName = ExpressionHelper.GetExpressionText(expression);
            var backup = htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix;
            var backupModel = htmlHelper.ViewData["Model"];
            htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix += (htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix != string.Empty ? "." : string.Empty) + propertyName + additionalPrefix;
            htmlHelper.ViewData["Model"] = model;
            var property = expression.Compile()(model);
            var result = htmlHelper.Partial(partialViewName, property);
            htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix = backup;
            htmlHelper.ViewData["Model"] = backupModel;
            return result;
        }

        /// <summary>
        /// Returns true if the specified model property has validation errors
        /// </summary>
        internal static bool BsPropertyIsInvalid<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression)
        {
            var propertyName = ExpressionHelper.GetExpressionText(expression);
            var name = helper.AttributeEncode(helper.ViewData.TemplateInfo.GetFullHtmlFieldName(propertyName));
            return helper.ViewData.ModelState[name] != null &&
                            helper.ViewData.ModelState[name].Errors != null &&
                            helper.ViewData.ModelState[name].Errors.Count > 0;
        }

        /// <summary>
        /// Appends the specified html attribute to an existing collection
        /// </summary>
        internal static void BsMergeAttribute(this IDictionary<string, object> htmlAttributes, string key, string val)
        {
            htmlAttributes = htmlAttributes ?? new Dictionary<string, object>();
            if (htmlAttributes.Any(x => x.Key == key))
            {
                var attr = htmlAttributes[key].ToString().ToLowerInvariant();
                if (!attr.Contains(val.ToLowerInvariant()))
                {
                    htmlAttributes[key] = htmlAttributes[key] + " " + val;
                }
            }
            else
            {
                htmlAttributes.Add(key, val);
            }
        }

        /// <summary>
        /// Returns the Description attribute value
        /// </summary>
        public static string GetDescription(this Enum enumValue)
        {
            var attribute = enumValue.GetType()
                        .GetMember(enumValue.ToString())
                        .First()
                        .GetCustomAttributes(false)
                        .OfType<DescriptionAttribute>()
                        .LastOrDefault();

            return attribute == null ? String.Empty : attribute.Description;
        }
    }
}