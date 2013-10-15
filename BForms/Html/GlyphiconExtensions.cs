using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using BForms.Models;
using BForms.Utilities;

namespace BForms.Html
{
    /// <summary>
    /// Represents bootstrap support for glyphicon fonts
    /// </summary>
    public static class GlyphiconExtensions
    {
        /// <summary>
        /// Returns a span element with glyphicon css
        /// </summary>
        public static MvcHtmlString BsGlyphicon<TModel>(this HtmlHelper<TModel> helper, Glyphicon icon,
            object htmlAttributes)
        {
            return helper.BsGlyphicon(icon, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Returns a span element with glyphicon css
        /// </summary>
        public static MvcHtmlString BsGlyphicon<TModel>(this HtmlHelper<TModel> helper, Glyphicon icon)
        {
            return helper.BsGlyphicon(icon, htmlAttributes: (object)null);
        }

        /// <summary>
        /// Returns a span element with glyphicon css
        /// </summary>
        public static MvcHtmlString BsGlyphicon<TModel>(this HtmlHelper<TModel> helper, Glyphicon icon,
            IDictionary<string, object> htmlAttributes)
        {
            var spanTag = new TagBuilder("span");
            spanTag.MergeAttributes(htmlAttributes);
            spanTag.AddCssClass(icon.GetDescription());
            spanTag.AddCssClass("glyphicon");
            return MvcHtmlString.Create(spanTag.ToString(TagRenderMode.Normal));
        }

        /// <summary>
        /// Returns a span element with glyphicon css and input-group-addon, to be used inside a input-group div
        /// </summary>
        public static MvcHtmlString BsGlyphiconAddon<TModel>(this HtmlHelper<TModel> helper, Glyphicon icon,
            object htmlAttributes)
        {
            return helper.BsGlyphiconAddon(icon, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Returns a span element with glyphicon css and input-group-addon, to be used inside a input-group div
        /// </summary>
        public static MvcHtmlString BsGlyphiconAddon<TModel>(this HtmlHelper<TModel> helper, Glyphicon icon)
        {
            return helper.BsGlyphiconAddon(icon, htmlAttributes: (object)null);
        }

        /// <summary>
        /// Returns a span element with glyphicon css and input-group-addon, to be used inside a input-group div
        /// </summary>
        public static MvcHtmlString BsGlyphiconAddon<TModel>(this HtmlHelper<TModel> helper, Glyphicon icon,
            IDictionary<string, object> htmlAttributes)
        {
            htmlAttributes.MergeAttribute("class", "input-group-addon");

            return helper.BsGlyphicon(icon, htmlAttributes);
        }
    }
}