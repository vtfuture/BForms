using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using BootstrapForms.Mvc;
using BootstrapForms.Utilities;
using BootstrapForms.Models;

namespace BootstrapForms.Html
{
    /// <summary>
    /// Represents support for making range selections
    /// </summary>
    public static class RangeExtensions
    {
        /// <summary>
        /// Returns a BForms range element based on BsControlAttribute
        /// </summary>
        public static MvcHtmlString BsRangeFor<TModel, TKey>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, BsRange<TKey>>> expression)
        {
            return htmlHelper.BsRangeFor(expression, null, null);
        }

        /// <summary>
        /// Returns a BForms range element based on BsControlAttribute
        /// </summary>
        public static MvcHtmlString BsRangeFor<TModel, TKey>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, BsRange<TKey>>> expression, object htmlAttributes)
        {
            return htmlHelper.BsRangeFor(expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), null);
        }

        /// <summary>
        /// Returns a BForms range element based on BsControlAttribute
        /// </summary>
        public static MvcHtmlString BsRangeFor<TModel, TKey>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, BsRange<TKey>>> expression, IDictionary<string, object> htmlAttributes, IDictionary<string, object> dataOptions)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var name = ExpressionHelper.GetExpressionText(expression);
            
            if (htmlAttributes == null)
            {
                htmlAttributes = new Dictionary<string, object>();
            }
            if (dataOptions == null)
            {
                dataOptions = new Dictionary<string, object>();
            }

            //add BForms custom html attributes
            htmlAttributes.ApplyBFormsAttributes(metadata, dataOptions);

            BsControlAttribute bsControl = null;
            if (ReflectionHelpers.TryGetControlAttribute(ExpressionHelper.GetExpressionText(expression), typeof(TModel), out bsControl))
            {
                htmlAttributes.MergeAttribute("type", bsControl.ControlType.GetHtml5Type(), true);
                htmlAttributes.MergeAttribute("class", bsControl.ControlType.GetDescription());

                if (bsControl.IsReadonly)
                {
                    htmlAttributes.MergeAttribute("readonly", "readonly");
                }
            }

            //add info tooltip
            var description = new MvcHtmlString("");
            if (!string.IsNullOrEmpty(metadata.Description))
            {
                description = htmlHelper.BsDescription(name);
            }

            return new MvcHtmlString(
                htmlHelper.BsRangeForInternal(expression, htmlAttributes, dataOptions).ToHtmlString() +
                description.ToHtmlString());
        }

        /// <summary>
        /// Returns a BForms range element based on BsControlAttribute
        /// </summary>
        internal static MvcHtmlString BsRangeForInternal<TModel, TKey>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, BsRange<TKey>>> expression, IDictionary<string, object> htmlAttributes, IDictionary<string, object> dataOptions)
        {
            var inputHtml = new StringBuilder();
            var name = ExpressionHelper.GetExpressionText(expression);
            var fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            var range = expression.Compile().Invoke(htmlHelper.ViewData.Model);
           
            name += ".TextValue";
            // If there are any errors for a named field, we add the css attribute.
            ModelState modelState;
            if (htmlHelper.ViewData.ModelState.TryGetValue(name, out modelState))
            {
                if (modelState.Errors.Count > 0)
                {
                    htmlAttributes.MergeAttribute("class", HtmlHelper.ValidationInputCssClassName);
                }
            }

            //Validation Hack
            name = name.Replace(".TextValue", "");

            var attributes = htmlHelper.GetUnobtrusiveValidationAttributes(name);

            if (name.Contains(".") && !attributes.Any())
            {
                attributes = htmlHelper.GetUnobtrusiveValidationAttributes(name.Split('.').LastOrDefault());
            }

            foreach (var item in attributes)
            {
                htmlAttributes.MergeAttribute(item.Key, item.Value.ToString(), true);
            }

            //Set display value
            string displayValue = null;
            if (range != null && !string.IsNullOrEmpty(range.TextValue))
            {
                displayValue = range.TextValue;
            }

            //render input type text
            inputHtml.Append(htmlHelper.TextBox(name + ".TextValue", displayValue, htmlAttributes).ToHtmlString());

            //render hidden for range
            htmlAttributes = new Dictionary<string, object>();
            htmlAttributes.MergeAttribute("data-for", fullName + ".TextValue");

            //From
            var fromName = fullName + ".From";
            htmlAttributes.MergeAttribute("class", "bs-range-from", true);

            var valFormated = range != null ? FormatValue(range.From) : string.Empty;

            var hiddenTag = new TagBuilder("input");
            hiddenTag.GenerateId(fromName);
            hiddenTag.MergeAttribute("name", fromName);
            hiddenTag.MergeAttribute("value", valFormated);
            hiddenTag.MergeAttribute("type", "hidden");
            hiddenTag.MergeAttributes(htmlAttributes);
            inputHtml.Append(hiddenTag.ToString(TagRenderMode.Normal));

            //To
            valFormated = range != null ? FormatValue(range.To) : string.Empty;
            var toName = fullName + ".To";
            htmlAttributes.MergeAttribute("class", "bs-range-to", true);
            hiddenTag = new TagBuilder("input");
            hiddenTag.GenerateId(toName);
            hiddenTag.MergeAttribute("name", toName);
            hiddenTag.MergeAttribute("value", valFormated);
            hiddenTag.MergeAttribute("type", "hidden");
            hiddenTag.MergeAttributes(htmlAttributes);
            inputHtml.Append(hiddenTag.ToString(TagRenderMode.Normal));


            return new MvcHtmlString(inputHtml.ToString());
        }

        internal static string FormatValue(object valRange)
        {

            var valFormated = string.Empty;
            if (valRange != null)
            {
                //apply Datetime ISO format
                if (valRange.GetType() == typeof (Nullable<DateTime>))
                {
                    var date = (DateTime?) valRange;
                    valFormated = date.Value.ToString("O");

                }
                else if (valRange.GetType() == typeof(DateTime))
                {
                    var date = (DateTime)valRange;
                    valFormated = date.ToString("O");
                }
                else
                {
                    valFormated = valRange.ToString();
                }
            }
            return valFormated;
        }
    }
}
