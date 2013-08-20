using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using BootstrapForms.Models;
using BootstrapForms.Utilities;

namespace BootstrapForms.HtmlHelpers
{
    /// <summary>
    /// Represents bootstrap v3 support for text input control
    /// </summary>
    public static class TextBoxExtensions
    {
        #region TextBox
        /// <summary>
        /// Returns an input type=text element
        /// </summary>
        public static MvcHtmlString BsTextBox(this HtmlHelper htmlHelper, string name)
        {
            return BsTextBox(htmlHelper, name, value: null);
        }

        /// <summary>
        /// Returns an input type=text element
        /// </summary>
        public static MvcHtmlString BsTextBox(this HtmlHelper htmlHelper, string name, object value)
        {
            return BsTextBox(htmlHelper, name, value, format: null);
        }

        /// <summary>
        /// Returns an input type=text element
        /// </summary>
        public static MvcHtmlString BsTextBox(this HtmlHelper htmlHelper, string name, object value, string format)
        {
            return BsTextBox(htmlHelper, name, value, format, htmlAttributes: (object)null);
        }

        /// <summary>
        /// Returns an input type=text element
        /// </summary>
        public static MvcHtmlString BsTextBox(this HtmlHelper htmlHelper, string name, object value, object htmlAttributes)
        {
            return BsTextBox(htmlHelper, name, value, format: null, htmlAttributes: htmlAttributes);
        }

        /// <summary>
        /// Returns an input type=text element
        /// </summary>
        public static MvcHtmlString BsTextBox(this HtmlHelper htmlHelper, string name, object value, string format,
            object htmlAttributes)
        {
            return BsTextBox(htmlHelper, name, value, format, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Returns an input type=text element
        /// </summary>
        public static MvcHtmlString BsTextBox(this HtmlHelper htmlHelper, string name, object value,
            IDictionary<string, object> htmlAttributes)
        {
            return BsTextBox(htmlHelper, name, value, format: null, htmlAttributes: htmlAttributes);
        }

        /// <summary>
        /// Returns an input type=text element
        /// </summary>
        public static MvcHtmlString BsTextBox(this HtmlHelper htmlHelper, string name, object value, string format,
            IDictionary<string, object> htmlAttributes)
        {
            //add bootstrap attributes
            htmlAttributes.ApplyTextBoxAttributes();

            return htmlHelper.TextBoxInternal(name, value, format, htmlAttributes);
        }

        #endregion

        #region TextBoxFor
        /// <summary>
        /// Returns a textbox input element with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression)
        {
            return BsTextBoxFor(htmlHelper, expression, (object)null);
        }

        /// <summary>
        /// Returns a textbox input element with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return BsTextBoxFor(htmlHelper, expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Returns a textbox input element with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            return BsTextBoxFor(htmlHelper, expression, null, htmlAttributes);
        }

        /// <summary>
        /// Returns a textbox input element with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string format)
        {
            return BsTextBoxFor(htmlHelper, expression, null, null);
        }

        /// <summary>
        /// Returns a textbox input element with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string format, object htmlAttributes)
        {
            return BsTextBoxFor(htmlHelper, expression, null, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Returns a textbox input element with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string format, IDictionary<string, object> htmlAttributes)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

            //add placeholder           
            if (!string.IsNullOrEmpty(metadata.Watermark) && !htmlAttributes.ContainsKey("placeholder"))
            {
                htmlAttributes.MergeAttribute("placeholder", metadata.Watermark);
            }

            //add bootstrap attributes
            htmlAttributes.ApplyTextBoxAttributes();

            //add info tooltip
            var description = new MvcHtmlString("");
            if (!string.IsNullOrEmpty(metadata.Description))
            {
                description = htmlHelper.BsDescriptionFor(expression);
            }

            return
                MvcHtmlString.Create(htmlHelper.TextBoxForInternal(expression, format, htmlAttributes).ToHtmlString() +
                                     description.ToHtmlString());
        }
        #endregion

        #region Helpers
        internal static MvcHtmlString TextBoxInternal(this HtmlHelper htmlHelper, string name, object value,
            string format, IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.TextBox(name, value, format, htmlAttributes);
        }

        internal static MvcHtmlString TextBoxForInternal<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression,
            string format, IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.TextBoxFor(expression, format, htmlAttributes);
        }

        internal static void ApplyTextBoxAttributes(this IDictionary<string, object> htmlAttributes)
        {
            //add bootstrap css
            htmlAttributes.MergeAttribute("class", "form-control");
            htmlAttributes.MergeAttribute("type", BsControlType.TextBox.GetHtml5Type());
            htmlAttributes.MergeAttribute("class", BsControlType.TextBox.GetDescription());
        }
        #endregion
    }
}
