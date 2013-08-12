using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using BootstrapForms.Utilities;

namespace BootstrapForms.HtmlHelpers
{
    /// <summary>
    /// Represents bootstrap v3 support for HTML input controls
    /// </summary>
    public static class InputExtensions
    {
        public static MvcHtmlString BsTextBox(this HtmlHelper helper, string name)
        {
            return BsTextBox(helper, name, value: null);
        }
        public static MvcHtmlString BsTextBox(this HtmlHelper helper, string name, object value)
        {
            return BsTextBox(helper, name, value, format: null);
        }
        public static MvcHtmlString BsTextBox(this HtmlHelper helper, string name, object value, string format)
        {
            return BsTextBox(helper, name, value, format, htmlAttributes: (object)null);
        }
        public static MvcHtmlString BsTextBox(this HtmlHelper helper, string name, object value, object htmlAttributes)
        {
            return BsTextBox(helper, name, value, format: null, htmlAttributes: htmlAttributes);
        }
        public static MvcHtmlString BsTextBox(this HtmlHelper helper, string name, object value, string format, object htmlAttributes)
        {
            return BsTextBox(helper, name, value, format, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }
        public static MvcHtmlString BsTextBox(this HtmlHelper helper, string name, object value, IDictionary<string, object> htmlAttributes)
        {
            return BsTextBox(helper, name, value, format: null, htmlAttributes: htmlAttributes);
        }
        public static MvcHtmlString BsTextBox(this HtmlHelper helper, string name, object value, string format, IDictionary<string, object> htmlAttributes)
        {
            var metadata = ModelMetadata.FromStringExpression(name, helper.ViewContext.ViewData);
            if (value != null)
            {
                metadata.Model = value;
            }

            //add placeholder           
            if (!string.IsNullOrEmpty(metadata.Watermark) && !htmlAttributes.ContainsKey("placeholder"))
            {
                htmlAttributes.MergeAttribute("placeholder", metadata.Watermark);
            }

            //add html5 input type
            if (!string.IsNullOrEmpty(metadata.DataTypeName))
            {
                htmlAttributes.MergeAttribute("type", metadata.DataTypeName.GetHtml5Type());
            }

            //merge custom css classes with bootstrap
            htmlAttributes.MergeAttribute("class", "form-control");
            return helper.TextBox(name, value, format, htmlAttributes);

        }

        /// <summary>
        /// Returns a textbox element with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var labelText = metadata.DisplayName ?? metadata.PropertyName ?? fieldName.Split('.').Last();

            var model = typeof(TModel);
            var property = model.GetProperty(fieldName);

            //merge custom css classes with bootstrap
            htmlAttributes.MergeAttribute("class", "form-control");

            //add placeholder           
            if (!string.IsNullOrEmpty(metadata.Watermark) && !htmlAttributes.ContainsKey("placeholder"))
            {
                htmlAttributes.MergeAttribute("placeholder", metadata.Watermark);
            }

            //add html5 input types
            if (!string.IsNullOrEmpty(metadata.DataTypeName))
            {
                htmlAttributes.MergeAttribute("type", metadata.DataTypeName.GetHtml5Type());
            }

            //add info tooltip
            var description = new MvcHtmlString("");
            if (!string.IsNullOrEmpty(metadata.Description))
            {
                description = helper.BsDescriptionFor(expression);
            }

            return MvcHtmlString.Create(helper.TextBoxFor(expression, htmlAttributes).ToHtmlString() + description.ToHtmlString());
        }
        public static MvcHtmlString BsTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression)
        {
            return BsTextBoxFor(helper, expression, (object)null);
        }
        public static MvcHtmlString BsTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return BsTextBoxFor(helper, expression, new RouteValueDictionary(htmlAttributes));
        }

        /// <summary>
        /// Returns a textbox area element with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsTextAreaFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);

            //merge custom css classes with bootstrap
            htmlAttributes.MergeAttribute("class", "form-control");

            //add placeholder
            if (!string.IsNullOrEmpty(metadata.Watermark) && !htmlAttributes.ContainsKey("placeholder"))
            {
                htmlAttributes.MergeAttribute("placeholder", metadata.Watermark);
            }

            //add info tooltip
            var description = new MvcHtmlString("");
            if (!string.IsNullOrEmpty(metadata.Description))
            {
                description = helper.BsDescriptionFor(expression);
            }

            return MvcHtmlString.Create(helper.TextAreaFor(expression, htmlAttributes).ToHtmlString() + description.ToHtmlString());
        }
        public static MvcHtmlString BsTextAreaFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression)
        {
            return BsTextAreaFor(helper, expression, (object)null);
        }
        public static MvcHtmlString BsTextAreaFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return BsTextAreaFor(helper, expression, new RouteValueDictionary(htmlAttributes));
        }

        /// <summary>
        /// Returns a checkbox element wrapped in a label element
        /// </summary>
        public static MvcHtmlString BsCheckBoxFor<TModel>(this HtmlHelper<TModel> helper, Expression<Func<TModel, bool>> expression, IDictionary<string, object> htmlAttributes)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var labelText = metadata.DisplayName ?? metadata.PropertyName ?? fieldName.Split('.').Last();

            var labelTag = new TagBuilder("label");
            var labelHtml = new StringBuilder(labelTag.ToString(TagRenderMode.StartTag));
            labelHtml.Append(helper.CheckBoxFor(expression, htmlAttributes));
            labelHtml.AppendLine(labelText);
            labelHtml.AppendLine(labelTag.ToString(TagRenderMode.EndTag));

            return MvcHtmlString.Create(labelHtml.ToString());
        }
        public static MvcHtmlString BsCheckBoxFor<TModel>(this HtmlHelper<TModel> helper, Expression<Func<TModel, bool>> expression)
        {
            return BsCheckBoxFor(helper, expression, (object)null);
        }
        public static MvcHtmlString BsCheckBoxFor<TModel>(this HtmlHelper<TModel> helper, Expression<Func<TModel, bool>> expression, object htmlAttributes)
        {
            return BsCheckBoxFor(helper, expression, new RouteValueDictionary(htmlAttributes));
        }
    }
}