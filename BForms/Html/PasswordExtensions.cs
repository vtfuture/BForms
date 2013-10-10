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
    /// Represents bootstrap v3 support for password input control
    /// </summary>
    public static class PasswordExtensions
    {
        #region Password
        /// <summary>
        /// Returns an input type=password element
        /// </summary>
        public static MvcHtmlString BsPassword(this HtmlHelper htmlHelper, string name)
        {
            return BsPassword(htmlHelper, name, value: null);
        }

        /// <summary>
        /// Returns an input type=password element
        /// </summary>
        public static MvcHtmlString BsPassword(this HtmlHelper htmlHelper, string name, object value)
        {
            return BsPassword(htmlHelper, name, value);
        }

        /// <summary>
        /// Returns an input type=password element
        /// </summary>
        public static MvcHtmlString BsPassword(this HtmlHelper htmlHelper, string name, object value, object htmlAttributes)
        {
            return BsPassword(htmlHelper, name, value, htmlAttributes);
        }

        /// <summary>
        /// Returns an input type=password element
        /// </summary>
        public static MvcHtmlString BsPassword(this HtmlHelper htmlHelper, string name, object value,
            IDictionary<string, object> htmlAttributes)
        {
            //add bootstrap attributes
            htmlAttributes.ApplyPasswordAttributes();

            return htmlHelper.PasswordInternal(name, value, htmlAttributes);
        }

        #endregion

        #region PasswordFor
        /// <summary>
        /// Returns a textbox password element with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsPasswordFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression)
        {
            return BsPasswordFor(htmlHelper, expression, (object)null);
        }

        /// <summary>
        /// Returns a textbox password element with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsPasswordFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return BsPasswordFor(htmlHelper, expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Returns a textbox password element with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsPasswordFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

            //add placeholder           
            if (!string.IsNullOrEmpty(metadata.Watermark) && !htmlAttributes.ContainsKey("placeholder"))
            {
                htmlAttributes.MergeAttribute("placeholder", metadata.Watermark);
            }

            //add bootstrap attributes
            htmlAttributes.ApplyPasswordAttributes();

            //add info tooltip
            var description = new MvcHtmlString("");
            if (!string.IsNullOrEmpty(metadata.Description))
            {
                description = htmlHelper.BsDescriptionFor(expression);
            }

            return
                MvcHtmlString.Create(htmlHelper.PasswordForInternal(expression, htmlAttributes).ToHtmlString() +
                                     description.ToHtmlString());
        }
        #endregion

        #region Helpers
        internal static MvcHtmlString PasswordInternal(this HtmlHelper htmlHelper, string name, object value,
            IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.Password(name, value, htmlAttributes);
        }

        internal static MvcHtmlString PasswordForInternal<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression,
            IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.PasswordFor(expression, htmlAttributes);
        }

        internal static void ApplyPasswordAttributes(this IDictionary<string, object> htmlAttributes)
        {
            //add bootstrap css
            htmlAttributes.MergeAttribute("class", "form-control");
            htmlAttributes.MergeAttribute("type", BsControlType.Password.GetHtml5Type());
            htmlAttributes.MergeAttribute("class", BsControlType.Password.GetDescription());
        }
        #endregion
    }
}
