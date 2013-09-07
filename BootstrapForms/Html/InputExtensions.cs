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
            return BsInputFor(htmlHelper, expression, null, null, null);
        }

        /// <summary>
        /// Returns an input element based on BsControlType with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsInputFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return BsInputFor(htmlHelper, expression, null, 
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), 
                null);
        }

        /// <summary>
        /// Returns an input element based on BsControlType with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsInputFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, object htmlAttributes, object dataOptions)
        {
            return BsInputFor(htmlHelper, expression, null,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes),
                HtmlHelper.AnonymousObjectToHtmlAttributes(dataOptions));
        }

        /// <summary>
        /// Returns an input element based on BsControlType with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsInputFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            return BsInputFor(htmlHelper, expression, null, htmlAttributes, null);
        }

        /// <summary>
        /// Returns an input element based on BsControlType with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsInputFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, 
            Expression<Func<TModel, TProperty>> expression, string format)
        {
            return BsInputFor(htmlHelper, expression, format, null, null);
        }

        /// <summary>
        /// Returns an input element based on BsControlType with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsInputFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, 
            Expression<Func<TModel, TProperty>> expression, string format, object htmlAttributes)
        {
            return BsInputFor(htmlHelper, expression, null, 
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), 
                null);
        }

        /// <summary>
        /// Returns an input element based on BsControlType with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsInputFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, string format, object htmlAttributes, object dataOptions)
        {
            return BsInputFor(htmlHelper, expression, null, 
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), 
                HtmlHelper.AnonymousObjectToHtmlAttributes(dataOptions));
        }

        /// <summary>
        /// Returns an input element based on BsControlType with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsInputFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, string format, 
            IDictionary<string, object> htmlAttributes, 
            IDictionary<string, object> dataOptions)
        {
            var inputHtml = new MvcHtmlString("");
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

            //add html attributes
            htmlAttributes.ApplyBFormsAttributes(metadata, dataOptions);

            //set html5 input type based on BsControlType attribute
            BsControlAttribute bsControl = null;
            if (ReflectionHelpers.TryGetControlAttribute(ExpressionHelper.GetExpressionText(expression), typeof(TModel), out bsControl))
            {
                htmlAttributes.MergeAttribute("type", bsControl.ControlType.GetHtml5Type(), true);
                htmlAttributes.MergeAttribute("class", bsControl.ControlType.GetDescription());

                if(bsControl.IsReadonly)
                {
                    htmlAttributes.MergeAttribute("readonly", "readonly");
                }

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
                        var dateExpression = (Expression<Func<TModel, BsDateTime>>)(object)expression;
                        inputHtml = htmlHelper.DateTimeForInternal(dateExpression, htmlAttributes, dataOptions);
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
                        throw new ArgumentException(bsControl.ControlType.GetDescription() + " does not match an input element");
                }
            }
            else
            {
                throw new InvalidOperationException("The " + name + " property is not decorated with a BsControlAttribute");
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