using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using BootstrapForms.Utilities;
using BootstrapForms.Models;

namespace BootstrapForms.Html
{
    /// <summary>
    /// Represents helpers for bootstrap forms validation
    /// </summary>
    public static class ValidationExtensions
    {
        /// <summary>
        /// The name of the CSS class that is used to style the input group validation errors
        /// </summary>
        public static MvcHtmlString BsValidationCssFor<TModel, TProperty>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TProperty>> expression)
        {
            var cssClass = string.Empty;

            if (helper.HasModelStateErros(expression))
            {
                cssClass = "has-error";
            }

            return MvcHtmlString.Create(cssClass);
        }

        /// <summary>
        /// Returns an alert div containing the BsFormError message
        /// </summary>
        public static MvcHtmlString BsValidationFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            var propertyName = ExpressionHelper.GetExpressionText(expression);
            var name = htmlHelper.AttributeEncode(htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(propertyName));
            var fullName = htmlHelper.AttributeEncode(htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(propertyName));

             if (typeof(TProperty).FullName.Contains("BsSelectList"))
             {
                 name += ".SelectedValues";
             }

             if (typeof(TProperty).FullName.Contains("BsRange"))
             {
                 name += ".TextValue";
             }

            //create span element
            var tag = new TagBuilder("span");
            tag.MergeAttributes(htmlAttributes, false);

            var isInvalid = htmlHelper.HasModelStateErros(expression);

            //add jquery validatior html attributes & css
            tag.AddCssClass(isInvalid
                ? HtmlHelper.ValidationMessageCssClassName
                : HtmlHelper.ValidationMessageValidCssClassName);
            tag.Attributes.Add("data-valmsg-for", name);
            tag.Attributes.Add("data-valmsg-replace", "true");

            //add bootstrap glyphicon & tooltip
            tag.AddCssClass("input-group-addon glyphicon glyphicon-warning-sign");
            tag.Attributes.Add("data-toggle", "tooltip");

            if (isInvalid)
            {
                foreach (var error in htmlHelper.ViewData.ModelState[fullName].Errors)
                {
                    //add error message as title
                    tag.Attributes.Add("title", error.ErrorMessage);
                    break;
                }
            }

            //build span html element
            var returnTag = new StringBuilder(tag.ToString(TagRenderMode.StartTag));
            returnTag.Append(tag.ToString(TagRenderMode.EndTag));
            return MvcHtmlString.Create(returnTag.ToString());
        }

        /// <summary>
        /// Returns an alert div containing the BsFormError message
        /// </summary>
        public static MvcHtmlString BsValidationFor<TModel, TProperty>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TProperty>> expression)
        {
            return helper.BsValidationFor(expression, htmlAttributes: (object) null);
        }

        /// <summary>
        /// Returns an alert div containing the BsFormError message
        /// </summary>
        public static MvcHtmlString BsValidationFor<TModel, TProperty>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return helper.BsValidationFor(expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Returns an alert div containing the BsFormError message
        /// </summary>
        public static MvcHtmlString BsValidationSummary(this HtmlHelper helper, IDictionary<string, object> htmlAttributes)
        {
            var prefix = helper.NameForModel().ToHtmlString();
            string name = prefix + ".BsFormError";

            var isInvalid = helper.ViewData.ModelState[name] != null &&
                            helper.ViewData.ModelState[name].Errors != null &&
                            helper.ViewData.ModelState[name].Errors.Count > 0;
            if (isInvalid)
            {
                //create div element
                var divTag = new TagBuilder("div");
                divTag.MergeAttributes(htmlAttributes);
                
                //add bootstrap css
                divTag.AddCssClass("alert alert-danger");
                divTag.AddCssClass("bs-form-error");

                //open div html element
                var divHtml = new StringBuilder(divTag.ToString(TagRenderMode.StartTag));

                //create close button
                var btnTag = new TagBuilder("button");
                btnTag.Attributes.Add("type", "button");
                btnTag.Attributes.Add("data-dismiss", "alert");
                btnTag.AddCssClass("close");

                var btnHtml = new StringBuilder(btnTag.ToString(TagRenderMode.StartTag));
                btnHtml.Append("&times;");
                btnHtml.Append(btnTag.ToString(TagRenderMode.EndTag));

                //add close button in div
                divHtml.Append(btnHtml);

                //append error message for BsFormError
                divHtml.Append(helper.ViewData.ModelState[name].Errors[0].ErrorMessage);
                
                //close div
                divHtml.Append(divTag.ToString(TagRenderMode.EndTag));

                //inline alert html
                return MvcHtmlString.Create(divHtml.ToString());
            }

            return MvcHtmlString.Empty;
        }

        /// <summary>
        /// Returns an alert div containing the BsFormError message
        /// </summary>
        public static MvcHtmlString BsValidationSummary(this HtmlHelper helper, object htmlAttributes)
        {
            return helper.BsValidationSummary(new RouteValueDictionary(htmlAttributes));
        }

        /// <summary>
        /// Returns an alert div containing the BsFormError message
        /// </summary>
        public static MvcHtmlString BsValidationSummary(this HtmlHelper helper)
        {
            return helper.BsValidationSummary(new RouteValueDictionary((object)null));
        }
    }
}