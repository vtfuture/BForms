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
    /// Represents a radio button input element that is used to present mutually exclusive options
    /// </summary>
    public static class RadioButtonExtensions
    {
        #region RadioButton
        /// <summary>
        /// Returns a checkbox input element wrapped in a label element
        /// </summary>
        public static MvcHtmlString BsRadioButton(this HtmlHelper htmlHelper, string name)
        {
            return BsRadioButton(htmlHelper, name, htmlAttributes: (object)null);
        }

        /// <summary>
        /// Returns a checkbox input element wrapped in a label element
        /// </summary>
        public static MvcHtmlString BsRadioButton(this HtmlHelper htmlHelper, string name, bool isChecked)
        {
            return BsRadioButton(htmlHelper, name, isChecked, htmlAttributes: (object)null);
        }

        /// <summary>
        /// Returns a checkbox input element wrapped in a label element
        /// </summary>
        public static MvcHtmlString BsRadioButton(this HtmlHelper htmlHelper, string name, bool isChecked,
            object htmlAttributes)
        {
            return BsRadioButton(htmlHelper, name, isChecked, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Returns a checkbox input element wrapped in a label element
        /// </summary>
        public static MvcHtmlString BsRadioButton(this HtmlHelper htmlHelper, string name, object htmlAttributes)
        {
            return BsRadioButton(htmlHelper, name, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Returns a checkbox input element wrapped in a label element
        /// </summary>
        public static MvcHtmlString BsRadioButton(this HtmlHelper htmlHelper, string name,
            IDictionary<string, object> htmlAttributes)
        {
            return BsRadioButton(htmlHelper, name: name, isChecked: false, htmlAttributes: htmlAttributes);
        }

        /// <summary>
        /// Returns a checkbox input element wrapped in a label element
        /// </summary>
        public static MvcHtmlString BsRadioButton(this HtmlHelper htmlHelper, string name, bool isChecked,
            IDictionary<string, object> htmlAttributes)
        {
            //add bootstrap attributes
            htmlAttributes.ApplyRadioButtonAttributes();

            return htmlHelper.RadioButtonInternal(name, isChecked, htmlAttributes);
        }

        #endregion

        #region RadioButtonFor
        /// <summary>
        /// Returns a checkbox input element wrapped in a label element
        /// </summary>
        public static MvcHtmlString BsRadioButtonFor<TModel>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, bool>> expression)
        {
            return BsRadioButtonFor(htmlHelper, expression, (object)null);
        }

        /// <summary>
        /// Returns a checkbox input element wrapped in a label element
        /// </summary>
        public static MvcHtmlString BsRadioButtonFor<TModel>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, bool>> expression, object htmlAttributes)
        {
            return BsRadioButtonFor(htmlHelper, expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Returns a checkbox input element wrapped in a label element
        /// </summary>
        public static MvcHtmlString BsRadioButtonFor<TModel>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, bool>> expression, IDictionary<string, object> htmlAttributes)
        {
            //add bootstrap attributes
            htmlAttributes.ApplyRadioButtonAttributes();

            return htmlHelper.RadioButtonForInternal(expression, htmlAttributes);
        }
        #endregion

        #region Helpers
        internal static MvcHtmlString RadioButtonInternal(this HtmlHelper htmlHelper, string name, bool isChecked,
            IDictionary<string, object> htmlAttributes)
        {
            var metadata = ModelMetadata.FromStringExpression(name, htmlHelper.ViewData);
            var fieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            var labelText = metadata.DisplayName ?? metadata.PropertyName ?? fieldName.Split('.').Last();

            var labelTag = new TagBuilder("label");
            var labelHtml = new StringBuilder(labelTag.ToString(TagRenderMode.StartTag));

            labelHtml.Append(htmlHelper.RadioButton(name, isChecked, htmlAttributes));

            labelHtml.AppendLine(labelText);
            labelHtml.AppendLine(labelTag.ToString(TagRenderMode.EndTag));

            return MvcHtmlString.Create(labelHtml.ToString());
        }

        internal static MvcHtmlString RadioButtonForInternal<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, bool>> expression,
            IDictionary<string, object> htmlAttributes)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var labelText = metadata.DisplayName ?? metadata.PropertyName ?? fieldName.Split('.').Last();

            var labelTag = new TagBuilder("label");
            var labelHtml = new StringBuilder(labelTag.ToString(TagRenderMode.StartTag));

            labelHtml.Append(htmlHelper.RadioButtonFor(expression, htmlAttributes));

            labelHtml.AppendLine(labelText);
            labelHtml.AppendLine(labelTag.ToString(TagRenderMode.EndTag));

            return MvcHtmlString.Create(labelHtml.ToString());
        }

        internal static void ApplyRadioButtonAttributes(this IDictionary<string, object> htmlAttributes)
        {
            //add bootstrap css
            //htmlAttributes.MergeAttribute("class", "form-control");
            htmlAttributes.MergeAttribute("class", BsControlType.RadioButton.GetDescription());
        }
        #endregion
    }
}
