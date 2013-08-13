using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace BootstrapForms.HtmlHelpers
{
    /// <summary>
    /// Represents helpers for bootstrap forms info tooltips
    /// </summary>
    public static class TooltipExtensions
    {
        /// <summary>
        /// Returns a span element containing the localized value of Display description attribute
        /// </summary>
        public static MvcHtmlString BsDescription(this HtmlHelper helper, string name)
        {
            return helper.BsDescription(name, htmlAttributes: (object) null);
        }

        /// <summary>
        /// Returns a span element containing the localized value of Display description attribute
        /// </summary>
        public static MvcHtmlString BsDescription(this HtmlHelper helper, string name, object htmlAttributes)
        {
            return helper.BsDescription(name, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Returns a span element containing the localized value of Display description attribute
        /// </summary>
        public static MvcHtmlString BsDescription(this HtmlHelper helper, string name,
            IDictionary<string, object> htmlAttributes)
        {
            var metaData = ModelMetadata.FromStringExpression(name, helper.ViewData);

            if (!string.IsNullOrEmpty(metaData.Description))
            {
                //create span element
                var tag = new TagBuilder("span");
                tag.MergeAttributes(htmlAttributes, false);

                //add bootstrap glyphicon & tooltip
                tag.AddCssClass("input-group-addon glyphicon glyphicon-info-sign");
                tag.Attributes.Add("data-toggle", "tooltip");
                tag.Attributes.Add("title", metaData.Description);

                //build span html element
                var returnTag = new StringBuilder(tag.ToString(TagRenderMode.StartTag));
                returnTag.Append(tag.ToString(TagRenderMode.EndTag));
                return MvcHtmlString.Create(returnTag.ToString());
            }

            return MvcHtmlString.Create(string.Empty);
        }

        /// <summary>
        /// Returns a span element containing the localized value of Display description attribute
        /// </summary>
        public static MvcHtmlString BsDescriptionFor<TModel, TProperty>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TProperty>> expression)
        {
            return helper.BsDescriptionFor(expression, htmlAttributes: (object) null);
        }

        /// <summary>
        /// Returns a span element containing the localized value of Display description attribute
        /// </summary>
        public static MvcHtmlString BsDescriptionFor<TModel, TProperty>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return helper.BsDescriptionFor(expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Returns a span element containing the localized value of Display description attribute
        /// </summary>
        public static MvcHtmlString BsDescriptionFor<TModel, TProperty>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            var metaData = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);

            if (!string.IsNullOrEmpty(metaData.Description))
            {
                //create span element
                var tag = new TagBuilder("span");
                tag.MergeAttributes(htmlAttributes, false);

                //add bootstrap glyphicon & tooltip
                tag.AddCssClass("input-group-addon glyphicon glyphicon-info-sign");
                tag.Attributes.Add("data-toggle", "tooltip");
                tag.Attributes.Add("title", metaData.Description);

                //build span html element
                var returnTag = new StringBuilder(tag.ToString(TagRenderMode.StartTag));
                returnTag.Append(tag.ToString(TagRenderMode.EndTag));
                return MvcHtmlString.Create(returnTag.ToString());
            }

            return MvcHtmlString.Create(string.Empty);
        }
    }
}