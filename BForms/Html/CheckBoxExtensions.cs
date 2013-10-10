using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using BForms.Models;
using BForms.Utilities;

namespace BForms.Html
{
    /// <summary>
    /// Represents a check box input element
    /// </summary>
    public static class CheckBoxExtensions
    {
        #region CheckBox
        /// <summary>
        /// Returns a checkbox input element wrapped in a label element
        /// </summary>
        public static MvcHtmlString BsCheckBox(this HtmlHelper htmlHelper, string name)
        {
            return BsCheckBox(htmlHelper, name, htmlAttributes: (object)null);
        }

        /// <summary>
        /// Returns a checkbox input element wrapped in a label element
        /// </summary>
        public static MvcHtmlString BsCheckBox(this HtmlHelper htmlHelper, string name, bool isChecked)
        {
            return BsCheckBox(htmlHelper, name, isChecked, htmlAttributes: (object)null);
        }

        /// <summary>
        /// Returns a checkbox input element wrapped in a label element
        /// </summary>
        public static MvcHtmlString BsCheckBox(this HtmlHelper htmlHelper, string name, bool isChecked,
            object htmlAttributes)
        {
            return BsCheckBox(htmlHelper, name, isChecked, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Returns a checkbox input element wrapped in a label element
        /// </summary>
        public static MvcHtmlString BsCheckBox(this HtmlHelper htmlHelper, string name, object htmlAttributes)
        {
            return BsCheckBox(htmlHelper, name, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Returns a checkbox input element wrapped in a label element
        /// </summary>
        public static MvcHtmlString BsCheckBox(this HtmlHelper htmlHelper, string name,
            IDictionary<string, object> htmlAttributes)
        {
            return BsCheckBox(htmlHelper, name: name, isChecked: false, htmlAttributes: htmlAttributes);
        }

        /// <summary>
        /// Returns a checkbox input element wrapped in a label element
        /// </summary>
        public static MvcHtmlString BsCheckBox(this HtmlHelper htmlHelper, string name, bool isChecked,
            IDictionary<string, object> htmlAttributes)
        {
            //add bootstrap attributes
            htmlAttributes.ApplyCheckBoxAttributes();

            return htmlHelper.CheckBoxInternal(name, isChecked, htmlAttributes);
        }

        #endregion

        #region CheckBoxFor
        /// <summary>
        /// Returns a checkbox input element wrapped in a label element
        /// </summary>
        public static MvcHtmlString BsCheckBoxFor<TModel>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, bool>> expression)
        {
            return BsCheckBoxFor(htmlHelper, expression, (object)null);
        }

        /// <summary>
        /// Returns a checkbox input element wrapped in a label element
        /// </summary>
        public static MvcHtmlString BsCheckBoxFor<TModel>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, bool>> expression, object htmlAttributes)
        {
            return BsCheckBoxFor(htmlHelper, expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Returns a checkbox input element wrapped in a label element
        /// </summary>
        public static MvcHtmlString BsCheckBoxFor<TModel>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, bool>> expression, IDictionary<string, object> htmlAttributes)
        {
            //add bootstrap attributes
            htmlAttributes.ApplyCheckBoxAttributes();

            return htmlHelper.CheckBoxForInternal(expression, htmlAttributes);
        }
        #endregion

        #region Helpers
        internal static MvcHtmlString CheckBoxInternal(this HtmlHelper htmlHelper, string name, bool isChecked,
            IDictionary<string, object> htmlAttributes)
        {
            var metadata = ModelMetadata.FromStringExpression(name, htmlHelper.ViewData);
            var fieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            var labelText = metadata.DisplayName ?? metadata.PropertyName ?? fieldName.Split('.').Last();

            var labelTag = new TagBuilder("label");
            var labelHtml = new StringBuilder(labelTag.ToString(TagRenderMode.StartTag));

            labelHtml.Append(htmlHelper.CheckBox(name, isChecked, htmlAttributes));

            labelHtml.AppendLine(labelText);
            labelHtml.AppendLine(labelTag.ToString(TagRenderMode.EndTag));

            return MvcHtmlString.Create(labelHtml.ToString());
        }

        internal static MvcHtmlString CheckBoxForInternal<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, bool>> expression,
            IDictionary<string, object> htmlAttributes)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var labelText = metadata.DisplayName ?? metadata.PropertyName ?? fieldName.Split('.').Last();

            var labelTag = new TagBuilder("label");

            //determine if the prop is decorated with Required
            var model = typeof(TModel);
            PropertyInfo property = null;
            foreach (var prop in fieldName.Split('.'))
            {
                property = model.GetProperty(prop);
                model = property.PropertyType;
            }
            if (property != null)
            {
                var isRequired = Attribute.IsDefined(property,
                    typeof(Mvc.BsMandatoryAttribute));

                if (isRequired)
                {
                    labelTag.AddCssClass("required");
                }
            }

            var labelHtml = new StringBuilder(labelTag.ToString(TagRenderMode.StartTag));

            labelHtml.Append(htmlHelper.CheckBoxFor(expression, htmlAttributes));

            labelHtml.AppendLine(labelText);
            labelHtml.AppendLine(labelTag.ToString(TagRenderMode.EndTag));

            return MvcHtmlString.Create(labelHtml.ToString());
        }

        internal static void ApplyCheckBoxAttributes(this IDictionary<string, object> htmlAttributes)
        {
            //add bootstrap css
            //htmlAttributes.MergeAttribute("class", "form-control");
            htmlAttributes.MergeAttribute("class", BsControlType.CheckBox.GetDescription());
        }
        #endregion
    }
}
