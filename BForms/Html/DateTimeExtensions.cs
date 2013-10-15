using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using BForms.Models;
using BForms.Utilities;

namespace BForms.Html
{
    /// <summary>
    /// Represents bootstrap support for date and time input control
    /// </summary>
    public static class DateTimeExtensions
    {
        #region DateTime
        /// <summary>
        /// Returns an input type=datetime element
        /// </summary>
        public static MvcHtmlString BsDateTimePicker(this HtmlHelper htmlHelper, string name)
        {
            return BsDateTimePicker(htmlHelper, name, value: null);
        }

        /// <summary>
        /// Returns an input type=datetime element
        /// </summary>
        public static MvcHtmlString BsDateTimePicker(this HtmlHelper htmlHelper, string name, DateTime? value)
        {
            return BsDateTimePicker(htmlHelper, name, value, null);
        }

        /// <summary>
        /// Returns an input type=datetime element
        /// </summary>
        public static MvcHtmlString BsDateTimePicker(this HtmlHelper htmlHelper, string name, DateTime? value, object htmlAttributes)
        {
            return BsDateTimePicker(htmlHelper, name, value, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), null);
        }

        /// <summary>
        /// Returns an input type=datetime element
        /// </summary>
        public static MvcHtmlString BsDateTimePicker(this HtmlHelper htmlHelper, string name, DateTime? value,
            IDictionary<string, object> htmlAttributes, IDictionary<string, object> dataOptions)
        {
            //add bootstrap attributes
            htmlAttributes.ApplyDateTimePickerAttributes();

            return htmlHelper.DateTimeInternal(name, value, htmlAttributes, dataOptions);
        }

        #endregion

        #region DateTimeFor
        /// <summary>
        /// Returns a DateTime picker input element with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsDateTimePickerFor<TModel>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, BsDateTime>> expression)
        {
            return BsDateTimePickerFor(htmlHelper, expression, (object)null);
        }

        /// <summary>
        /// Returns a DateTime picker input element with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsDateTimePickerFor<TModel>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, BsDateTime>> expression, object htmlAttributes)
        {
            return BsDateTimePickerFor(htmlHelper, expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), null);
        }

        /// <summary>
        /// Returns a DateTime picker input element with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsDateTimePickerFor<TModel>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, BsDateTime>> expression, IDictionary<string, object> htmlAttributes, IDictionary<string, object> dataOptions)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

            if (htmlAttributes == null)
            {
                htmlAttributes = new Dictionary<string, object>();
            }
            if (dataOptions == null)
            {
                dataOptions = new Dictionary<string, object>();
            }

            //add placeholder           
            if (!string.IsNullOrEmpty(metadata.Watermark) && !htmlAttributes.ContainsKey("placeholder"))
            {
                htmlAttributes.MergeAttribute("placeholder", metadata.Watermark);
            }

            //add bootstrap attributes
            htmlAttributes.ApplyDateTimePickerAttributes();

            //add info tooltip
            var description = new MvcHtmlString("");
            if (!string.IsNullOrEmpty(metadata.Description))
            {
                description = htmlHelper.BsDescriptionFor(expression);
            }

            return
                MvcHtmlString.Create(htmlHelper.DateTimeForInternal(expression, htmlAttributes, dataOptions).ToHtmlString() +
                                     description.ToHtmlString());
        }
        #endregion

        #region Helpers
        internal static MvcHtmlString DateTimeInternal(this HtmlHelper htmlHelper, string name, DateTime? value,
             IDictionary<string, object> htmlAttributes, IDictionary<string, object> dataOptions)
        {
            var inputHtml = new StringBuilder();

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
            if (value.HasValue)
            {
                displayValue = value.Value.ToString();
            }

            //render input type text
            inputHtml.Append(htmlHelper.TextBox(name + ".TextValue", displayValue, htmlAttributes).ToHtmlString());

            //render hidden for range
            htmlAttributes = new Dictionary<string, object>();
            htmlAttributes.MergeAttribute("data-for", name + ".TextValue");

            //From
            var fromName = name + ".DateValue";
            htmlAttributes.MergeAttribute("class", "bs-date-iso", true);

            var valFormated = (value.HasValue) ? RangeExtensions.FormatValue(value) : string.Empty;

            var hiddenTag = new TagBuilder("input");
            hiddenTag.GenerateId(fromName);
            hiddenTag.MergeAttribute("name", fromName);
            hiddenTag.MergeAttribute("value", valFormated);
            hiddenTag.MergeAttribute("type", "hidden");
            hiddenTag.MergeAttributes(htmlAttributes);
            inputHtml.Append(hiddenTag.ToString(TagRenderMode.Normal));

            return new MvcHtmlString(inputHtml.ToString());
        }

        internal static MvcHtmlString DateTimeForInternal<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, BsDateTime>> expression,
             IDictionary<string, object> htmlAttributes, IDictionary<string, object> dataOptions)
        {
            var inputHtml = new StringBuilder();
            var name = ExpressionHelper.GetExpressionText(expression);
            var fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            var bsDate = expression.Compile().Invoke(htmlHelper.ViewData.Model);

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
            if (bsDate != null && !string.IsNullOrEmpty(bsDate.TextValue))
            {
                displayValue = bsDate.TextValue;
            }

            //render input type text
            inputHtml.Append(htmlHelper.TextBox(name + ".TextValue", displayValue, htmlAttributes).ToHtmlString());

            //render hidden for range
            htmlAttributes = new Dictionary<string, object>();
            htmlAttributes.MergeAttribute("data-for", fullName + ".TextValue");

            //From
            var fromName = fullName + ".DateValue";
            htmlAttributes.MergeAttribute("class", "bs-date-iso", true);

            var valFormated = (bsDate != null && bsDate.DateValue != null) ? RangeExtensions.FormatValue(bsDate.DateValue) : string.Empty;

            var hiddenTag = new TagBuilder("input");
            hiddenTag.GenerateId(fromName);
            hiddenTag.MergeAttribute("name", fromName);
            hiddenTag.MergeAttribute("value", valFormated);
            hiddenTag.MergeAttribute("type", "hidden");
            hiddenTag.MergeAttributes(htmlAttributes);
            inputHtml.Append(hiddenTag.ToString(TagRenderMode.Normal));

            return new MvcHtmlString(inputHtml.ToString());
        }

        internal static void ApplyDateTimePickerAttributes(this IDictionary<string, object> htmlAttributes)
        {
            //add bootstrap css
            htmlAttributes.MergeAttribute("class", "form-control");
            htmlAttributes.MergeAttribute("type", BsControlType.DateTimePicker.GetHtml5Type());
            htmlAttributes.MergeAttribute("class", BsControlType.DateTimePicker.GetDescription());
        }
        #endregion
    }
}
