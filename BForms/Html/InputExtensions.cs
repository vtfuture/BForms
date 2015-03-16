using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using BForms.Models;
using BForms.Mvc;
using BForms.Utilities;

namespace BForms.Html
{
    /// <summary>
    /// Represents bootstrap support for HTML5 input controls
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
        /// Returns an input element with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsInputFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, BsControlType controlType)
        {
            return BsInputFor(htmlHelper, expression, controlType, null, null, null);
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
            Expression<Func<TModel, TProperty>> expression, BsControlType controlType, object htmlAttributes)
        {
            return BsInputFor(htmlHelper, expression, controlType, null,
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
            Expression<Func<TModel, TProperty>> expression, BsControlType controlType, object htmlAttributes, object dataOptions)
        {
            return BsInputFor(htmlHelper, expression, controlType, null,
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
            BsControlAttribute bsControl = null;
            if (ReflectionHelpers.TryGetAttribute(ExpressionHelper.GetExpressionText(expression), typeof(TModel),
                out bsControl))
            {
                if (bsControl.IsReadonly)
                {
                    htmlAttributes.MergeAttribute("readonly", "readonly");
                }

                return htmlHelper.BsInputFor(expression, bsControl.ControlType, format, htmlAttributes, dataOptions);
            }
            else
            {
                var name = ExpressionHelper.GetExpressionText(expression);

                throw new InvalidOperationException("The " + name + " property is not decorated with a BsControlAttribute");
            }

        }

        public static MvcHtmlString BsInputFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
           Expression<Func<TModel, TProperty>> expression, BsControlType controlType, string format,
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

            htmlAttributes.MergeAttribute("type", controlType.GetHtml5Type(), true);
            htmlAttributes.MergeAttribute("class", controlType.GetDescription());

            switch (controlType)
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
                case BsControlType.NumberInline:
                    var genericArguments = typeof(TProperty).GetGenericArguments();

                    if (genericArguments.Any() && (genericArguments[0] == typeof(int) || genericArguments[0] == typeof(int?)))
                    {
                        htmlAttributes.MergeAttribute("class",
                            controlType == BsControlType.NumberInline
                                ? "bs-number-single_range_inline"
                                : "bs-number-single_range");
                        htmlAttributes.MergeAttribute("type", "text");

                        if (genericArguments[0] == typeof(int))
                        {
                            var numberRange = (Expression<Func<TModel, BsRangeItem<int>>>)(object)expression;
                            inputHtml = htmlHelper.NumberRangeForInternal(numberRange, htmlAttributes, dataOptions);
                        }
                        else
                        {
                            var numberRange = (Expression<Func<TModel, BsRangeItem<int?>>>)(object)expression;
                            inputHtml = htmlHelper.NumberRangeForInternal(numberRange, htmlAttributes, dataOptions);
                        }

                    }
                    else
                    {
                        inputHtml = htmlHelper.TextBoxForInternal(expression, format, htmlAttributes);
                    }

                    break;
                case BsControlType.DatePicker:
                case BsControlType.DateTimePicker:
                case BsControlType.TimePicker:
                    if (typeof(TProperty) != typeof(BsDateTime))
                    {
                        throw new ArgumentException("The " + name + " property must be of type BsDateTime");
                    }
                    var dateExpression = (Expression<Func<TModel, BsDateTime>>)(object)expression;
                    inputHtml = htmlHelper.DateTimeForInternal(dateExpression, htmlAttributes, dataOptions);
                    break;
                case BsControlType.CheckBox:
                    if (typeof(TProperty) != typeof(bool))
                    {
                        throw new ArgumentException("The " + name + " property must be of type bool");
                    }
                    var checkExpression = (Expression<Func<TModel, bool>>)(object)expression;
                    inputHtml = htmlHelper.CheckBoxForInternal(checkExpression, htmlAttributes);
                    break;
                case BsControlType.RadioButton:
                    if (typeof(TProperty) != typeof(bool))
                    {
                        throw new ArgumentException("The " + name + " property must be of type bool");
                    }
                    var radioExpression = (Expression<Func<TModel, bool>>)(object)expression;
                    inputHtml = htmlHelper.RadioButtonForInternal(radioExpression, htmlAttributes);
                    break;
                case BsControlType.ColorPicker:
                    inputHtml = htmlHelper.TextBoxForInternal(expression, format, htmlAttributes);
                    break;
                case BsControlType.Upload:
                    inputHtml = htmlHelper.UploadForInternal(expression, format, htmlAttributes);
                    break;
                default:
                    throw new ArgumentException("The " + name + " property of type " + controlType.GetDescription() + " does not match an input element");
            }

            //add info tooltip
            var description = new MvcHtmlString("");
            if (!string.IsNullOrEmpty(metadata.Description))
            {
                description = htmlHelper.BsDescriptionFor(expression);
            }

            return MvcHtmlString.Create(inputHtml.ToHtmlString() + description.ToHtmlString());
        }

        public static MvcHtmlString BsInput<TProperty>(this HtmlHelper htmlHelper, BsControlType controlType,
            string name, string format, string value,
            IDictionary<string, object> htmlAttributes,
            IDictionary<string, object> dataOptions,
            ModelMetadata metadata = null)
        {
            var inputHtml = new MvcHtmlString("");

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

            htmlAttributes.MergeAttribute("type", controlType.GetHtml5Type(), true);
            htmlAttributes.MergeAttribute("class", controlType.GetDescription());


            switch (controlType)
            {
                case BsControlType.TextBox:
                    inputHtml = htmlHelper.TextBoxInternal(name, value, format, htmlAttributes);
                    break;
                case BsControlType.TextArea:
                    inputHtml = htmlHelper.TextAreaInternal(name, value, 2, 20, htmlAttributes);
                    break;
                case BsControlType.Password:
                    inputHtml = htmlHelper.PasswordInternal(name, value, htmlAttributes);
                    break;
                case BsControlType.Url:
                case BsControlType.Email:
                case BsControlType.Number:
                case BsControlType.NumberInline:
                    var genericArguments = typeof(TProperty).GetGenericArguments();

                    if (genericArguments.Any() && (genericArguments[0] == typeof(int) || genericArguments[0] == typeof(int?)))
                    {
                        htmlAttributes.MergeAttribute("class",
                            controlType == BsControlType.NumberInline
                                ? "bs-number-single_range_inline"
                                : "bs-number-single_range");
                        htmlAttributes.MergeAttribute("type", "text");

                        if (genericArguments[0] == typeof(int))
                        {
                            // var numberRange = (Expression<Func<TModel, BsRangeItem<int>>>)(object)expression;
                            // inputHtml = htmlHelper.NumberRangeForInternal(numberRange, htmlAttributes, dataOptions);
                        }
                        else
                        {
                            //  var numberRange = (Expression<Func<TModel, BsRangeItem<int?>>>)(object)expression;
                            //  inputHtml = htmlHelper.NumberRangeForInternal(numberRange, htmlAttributes, dataOptions);
                        }

                    }
                    else
                    {
                        inputHtml = htmlHelper.TextBoxInternal(name, value, format, htmlAttributes);
                    }

                    break;
                case BsControlType.DatePicker:
                case BsControlType.DateTimePicker:
                case BsControlType.TimePicker:
                    if (typeof(TProperty) != typeof(BsDateTime))
                    {
                        throw new ArgumentException("The " + name + " property must be of type BsDateTime");
                    }
                    //var dateExpression = (Expression<Func<TModel, BsDateTime>>)(object)expression;
                    //inputHtml = htmlHelper.DateTimeForInternal(dateExpression, htmlAttributes, dataOptions);
                    break;
                default:
                    throw new ArgumentException("The " + name + " of type " + controlType.GetDescription() + " does not match an input element");
            }


            //add info tooltip
            var description = new MvcHtmlString("");
            if (!string.IsNullOrEmpty(metadata.Description))
            {
                // description = htmlHelper.BsDescriptionFor(expression);
            }

            return MvcHtmlString.Create(inputHtml.ToHtmlString() + description.ToHtmlString());
        }
    }
}