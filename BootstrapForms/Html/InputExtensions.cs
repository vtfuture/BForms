using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using BootstrapForms.Models;
using BootstrapForms.Mvc;
using BootstrapForms.Utilities;

namespace BootstrapForms.Html
{
    /// <summary>
    /// Represents bootstrap v3 support for HTML5 input controls
    /// </summary>
    public static class InputExtensions
    {
        /// <summary>
        /// Returns an input element based on BsControlType with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsInputFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression)
        {
            return BsInputFor(htmlHelper, expression, (object)null);
        }

        /// <summary>
        /// Returns an input element based on BsControlType with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsInputFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return BsInputFor(htmlHelper, expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Returns an input element based on BsControlType with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsInputFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            return BsInputFor(htmlHelper, expression, null, htmlAttributes);
        }

        /// <summary>
        /// Returns an input element based on BsControlType with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsInputFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, 
            Expression<Func<TModel, TProperty>> expression, string format)
        {
            return BsInputFor(htmlHelper, expression, null, null);
        }

        /// <summary>
        /// Returns an input element based on BsControlType with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsInputFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, 
            Expression<Func<TModel, TProperty>> expression, string format, object htmlAttributes)
        {
            return BsInputFor(htmlHelper, expression, null, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), null);
        }

        /// <summary>
        /// Returns an input element based on BsControlType with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsInputFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, string format, IDictionary<string, object> htmlAttributes, IDictionary<string, object> dataOptions)
        {
            var inputHtml = new MvcHtmlString("");
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            htmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

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

            //set html5 input type based on BsControlType attribute
            BsControlAttribute bsControl = null;
            if (ReflectionHelpers.TryGetControlAttribute(ExpressionHelper.GetExpressionText(expression), typeof(TModel), out bsControl))
            {
                htmlAttributes.MergeAttribute("type", bsControl.ControlType.GetHtml5Type(), true);
                htmlAttributes.MergeAttribute("class", bsControl.ControlType.GetDescription());

                switch (bsControl.ControlType)
                {
                    case BsControlType.TextBox:
                        inputHtml = htmlHelper.TextBoxForInternal(expression, format, htmlAttributes);
                        break;
                    case BsControlType.TextArea:
                        inputHtml = htmlHelper.TextAreaForInternal(expression, 2, 20, htmlAttributes);
                        break;
                    case BsControlType.Password:
                        inputHtml = htmlHelper.PasswordFor(expression, htmlAttributes);
                        break;
                    case BsControlType.Url:
                    case BsControlType.Email:
                    case BsControlType.Number:
                        inputHtml = htmlHelper.TextBoxForInternal(expression, format, htmlAttributes);
                        break;
                    case BsControlType.DatePicker:
                    case BsControlType.DateTimePicker:
                    case BsControlType.TimePicker:
                        inputHtml = htmlHelper.TextBoxForInternal(expression, format, htmlAttributes);
                        break;
                    case BsControlType.CheckBox:
                        var checkExpression = (Expression<Func<TModel, bool>>) (object) expression;
                        inputHtml = htmlHelper.CheckBoxForInternal(checkExpression, htmlAttributes);
                        break;
                    case BsControlType.RadioButton:
                        var radioExpression = (Expression<Func<TModel, bool>>) (object) expression;
                        inputHtml = htmlHelper.RadioButtonForInternal(radioExpression, htmlAttributes);
                        break;
                    case BsControlType.ColorPicker:
                        inputHtml = htmlHelper.TextBoxForInternal(expression, format, htmlAttributes);
                        break;
                    default:
                        throw new Exception(bsControl.ControlType.GetDescription() + " does not match an input element");
                }
            }

            //add info tooltip
            var description = new MvcHtmlString("");
            if (!string.IsNullOrEmpty(metadata.Description))
            {
                description = htmlHelper.BsDescriptionFor(expression);
            }

            return MvcHtmlString.Create(inputHtml.ToHtmlString() + description.ToHtmlString());
        }

    }
}