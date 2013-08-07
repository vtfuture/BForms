using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using BootstrapForms.Razor;

namespace BootstrapForms.HtmlHelpers
{
    /// <summary>
    /// Represents bootstrap v3 support for HTML input controls
    /// </summary>
    public static class InputExtensions
    {
        /// <summary>
        /// Returns a textbox element with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var labelText = metadata.DisplayName ?? metadata.PropertyName ?? fieldName.Split('.').Last();

            //determine if the prop is decorated with Required
            var model = typeof(TModel);
            var property = model.GetProperty(fieldName);
            var isRequired = Attribute.IsDefined(property, typeof(RequiredAttribute));

            //merge custom css classes with bootstrap
            htmlAttributes.BsMergeAttribute("class", "form-control");

            //add placeholder           
            if (!string.IsNullOrEmpty(metadata.Watermark) && !htmlAttributes.ContainsKey("placeholder"))
            {
                htmlAttributes.BsMergeAttribute("placeholder", metadata.Watermark);
            }

            //add html5 input types
            if (!string.IsNullOrEmpty(metadata.DataTypeName))
            {
                string html5Type;
                var dataType = (DataTypeAttribute)Attribute.GetCustomAttribute(property, typeof(DataTypeAttribute));

                switch (dataType.DataType)
                {
                    case DataType.Date:
                        html5Type = "date";
                        break;
                    case DataType.DateTime:
                        html5Type = "datetime";
                        break;
                    case DataType.EmailAddress:
                        html5Type = "email";
                        break;
                    case DataType.ImageUrl:
                        html5Type = "url";
                        break;
                    case DataType.Password:
                        html5Type = "password";
                        break;
                    case DataType.PhoneNumber:
                        html5Type = "tel";
                        break;
                    case DataType.PostalCode:
                        html5Type = "number";
                        break;
                    case DataType.Text:
                        html5Type = "text";
                        break;
                    case DataType.Time:
                        html5Type = "time";
                        break;
                    case DataType.Upload:
                        html5Type = "file";
                        break;
                    case DataType.Url:
                        html5Type = "url";
                        break;
                    default:
                        html5Type = "text";
                        break;
                }

                htmlAttributes.BsMergeAttribute("type", html5Type);
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
            htmlAttributes.BsMergeAttribute("class", "form-control");

            //add placeholder
            if (!string.IsNullOrEmpty(metadata.Watermark) && !htmlAttributes.ContainsKey("placeholder"))
            {
                htmlAttributes.BsMergeAttribute("placeholder", metadata.Watermark);
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