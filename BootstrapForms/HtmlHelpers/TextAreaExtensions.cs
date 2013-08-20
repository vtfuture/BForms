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
    public static class TextAreaExtensions
    {
        #region TextArea
        /// <summary>
        /// Returns a textarea element
        /// </summary>
        public static MvcHtmlString BsTextArea(this HtmlHelper htmlHelper, string name)
        {
            return BsTextArea(htmlHelper, name, value: null, htmlAttributes: null);
        }

        /// <summary>
        /// Returns a textarea element
        /// </summary>
        public static MvcHtmlString BsTextArea(this HtmlHelper htmlHelper, string name, object htmlAttributes)
        {
            return BsTextArea(htmlHelper, name, null,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Returns a textarea element
        /// </summary>
        public static MvcHtmlString BsTextArea(this HtmlHelper htmlHelper, string name,
            IDictionary<string, object> htmlAttributes)
        {
            return BsTextArea(htmlHelper, name, null, htmlAttributes);
        }

        /// <summary>
        /// Returns a textarea element
        /// </summary>
        public static MvcHtmlString BsTextArea(this HtmlHelper htmlHelper, string name, string value)
        {
            return BsTextArea(htmlHelper, name, value, htmlAttributes: null);
        }

        /// <summary>
        /// Returns a textarea element
        /// </summary>
        public static MvcHtmlString BsTextArea(this HtmlHelper htmlHelper, string name, string value,
            object htmlAttributes)
        {
            return BsTextArea(htmlHelper, name, value, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Returns a textarea element
        /// </summary>
        public static MvcHtmlString BsTextArea(this HtmlHelper htmlHelper, string name, string value,
            IDictionary<string, object> htmlAttributes)
        {
            return BsTextArea(htmlHelper, name, value, rows: 2, columns: 20, 
                htmlAttributes: HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Returns a textarea element
        /// </summary>
        public static MvcHtmlString BsTextArea(this HtmlHelper htmlHelper, string name, string value, int rows,
            int columns, object htmlAttributes)
        {
            return BsTextArea(htmlHelper, name, value, rows, columns,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Returns a textarea element
        /// </summary>
        public static MvcHtmlString BsTextArea(this HtmlHelper htmlHelper, string name, string value, int rows,
            int columns, IDictionary<string, object> htmlAttributes)
        {
            htmlAttributes.ApplyTextAreaAttributes();

            return htmlHelper.TextAreaInternal(name, value, rows, columns, htmlAttributes);
        }

        #endregion

        #region TextAreaFor
        /// <summary>
        /// Returns a textarea element with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsTextAreaFor<TModel, TProperty>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TProperty>> expression)
        {
            return BsTextAreaFor(helper, expression, (object)null);
        }

        /// <summary>
        /// Returns a textarea element with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsTextAreaFor<TModel, TProperty>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return BsTextAreaFor(helper, expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Returns a textarea element with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsTextAreaFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.BsTextAreaFor(expression, rows: 2, columns: 20, htmlAttributes: htmlAttributes);
        }

        /// <summary>
        /// Returns a textarea element with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsTextAreaFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression,
            int rows, int columns, object htmlAttributes)
        {
            return BsTextAreaFor(htmlHelper, expression, rows, columns, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Returns a textarea element with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsTextAreaFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression,
            int rows, int columns, IDictionary<string, object> htmlAttributes)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

            //add placeholder           
            if (!string.IsNullOrEmpty(metadata.Watermark) && !htmlAttributes.ContainsKey("placeholder"))
            {
                htmlAttributes.MergeAttribute("placeholder", metadata.Watermark);
            }

            //add bootstrap attributes
            htmlAttributes.ApplyTextAreaAttributes();

            //add info tooltip
            var description = new MvcHtmlString("");
            if (!string.IsNullOrEmpty(metadata.Description))
            {
                description = htmlHelper.BsDescriptionFor(expression);
            }

            return
                MvcHtmlString.Create(htmlHelper.TextAreaForInternal(expression, rows, columns, htmlAttributes).ToHtmlString() +
                                     description.ToHtmlString());
        }
        #endregion

        #region Helpers
        internal static MvcHtmlString TextAreaInternal(this HtmlHelper htmlHelper, string name, string value,
            int rows, int columns, IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.TextArea(name, value, rows, columns, htmlAttributes);
        }

        internal static MvcHtmlString TextAreaForInternal<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression,
            int rows, int columns, IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.TextAreaFor(expression, rows, columns, htmlAttributes);
        }

        internal static void ApplyTextAreaAttributes(this IDictionary<string, object> htmlAttributes)
        {
            //add bootstrap css
            htmlAttributes.MergeAttribute("class", "form-control");
            htmlAttributes.MergeAttribute("class", BsControlType.TextArea.GetDescription());
        }
        #endregion
    }
}
