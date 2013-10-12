using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using BForms.Utilities;

namespace BForms.Html
{
    /// <summary>
    /// Represents HTML helpers for bootstrap forms
    /// </summary>
    public static class BaseExtensions
    {
        /// <summary>
        /// Replacer for Html.Partial when dealing with nested models
        /// </summary>
        /// <example>
        /// @Html.PartialPrefixed(m => m.Client.Address, "Client/_Address", Model)
        /// </example>
        public static MvcHtmlString BsPartialPrefixed<TModel, TNewModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TNewModel, TProperty>> expression, string partialViewName, TNewModel model,
            string additionalPrefix = "")
        {
            var propertyName = ExpressionHelper.GetExpressionText(expression);
            var backupFieldPrefix = htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix;
            var backupModel = htmlHelper.ViewData["Model"];

            var prefix = htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix != string.Empty ? "." : string.Empty;
            htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix += prefix + propertyName + additionalPrefix;

            htmlHelper.ViewData["Model"] = model;

            var property = expression.Compile()(model);
            var result = htmlHelper.Partial(partialViewName, property);

            htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix = backupFieldPrefix;
            htmlHelper.ViewData["Model"] = backupModel;

            return result;
        }

        /// <summary>
        /// Renders the name of an enum based on DisplayAttribute
        /// </summary>
        public static MvcHtmlString BsEnumDisplayName<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, TEnum val)
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("val is not of type enum", "val");
            }

            return new MvcHtmlString(ReflectionHelpers.EnumDisplayName(typeof(TEnum), val as Enum));
        }

        /// <summary>
        /// Appends BForms custom html attribute to an existing collection
        /// </summary>
        internal static void ApplyBFormsAttributes(this IDictionary<string, object> htmlAttributes, ModelMetadata metadata, IDictionary<string, object> dataOptions)
        {
            htmlAttributes = htmlAttributes ?? new Dictionary<string, object>();

            //merge custom css classes with bootstrap (if is checkbox or radio do not apply)
            if (metadata.ModelType != typeof(bool))
            {
                htmlAttributes.MergeAttribute("class", "form-control");
            }

            //add placeholder           
            if (!string.IsNullOrEmpty(metadata.Watermark) && !htmlAttributes.ContainsKey("placeholder"))
            {
                htmlAttributes.MergeAttribute("placeholder", metadata.Watermark);
            }

            //set html5 input type based on DataType attribute
            if (!string.IsNullOrEmpty(metadata.DataTypeName))
            {
                htmlAttributes.MergeAttribute("type", metadata.DataTypeName.GetHtml5Type());
            }

            //set data- options
            if (dataOptions != null && dataOptions.Any())
            {
                htmlAttributes.MergeAttribute("data-options", dataOptions.ToJsonString());
            }
        }
    }
}