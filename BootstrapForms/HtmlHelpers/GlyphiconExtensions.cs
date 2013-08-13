using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using BootstrapForms.Models;
using BootstrapForms.Utilities;

namespace BootstrapForms.HtmlHelpers
{
    /// <summary>
    /// Represents bootstrap v3 support for glyphicon fonts
    /// </summary>
    public static class GlyphiconExtensions
    {
        /// <summary>
        /// Returns a span element with glyphicon css
        /// </summary>
        public static MvcHtmlString BsGlyphicon<TModel>(this HtmlHelper<TModel> helper, Glyphicon icon,
            object htmlAttributes)
        {
            return BsGlyphicon(helper, icon, new RouteValueDictionary(htmlAttributes));
        }

        /// <summary>
        /// Returns a span element with glyphicon css
        /// </summary>
        public static MvcHtmlString BsGlyphicon<TModel>(this HtmlHelper<TModel> helper, Glyphicon icon)
        {
            return BsGlyphicon(helper, icon, (object) null);
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
            return BsGlyphiconAddon(helper, icon, new RouteValueDictionary(htmlAttributes));
        }

        /// <summary>
        /// Returns a span element with glyphicon css and input-group-addon, to be used inside a input-group div
        /// </summary>
        public static MvcHtmlString BsGlyphiconAddon<TModel>(this HtmlHelper<TModel> helper, Glyphicon icon)
        {
            return BsGlyphiconAddon(helper, icon, (object) null);
        }

        /// <summary>
        /// Returns a span element with glyphicon css and input-group-addon, to be used inside a input-group div
        /// </summary>
        public static MvcHtmlString BsGlyphiconAddon<TModel>(this HtmlHelper<TModel> helper, Glyphicon icon,
            IDictionary<string, object> htmlAttributes)
        {
            htmlAttributes.MergeAttribute("class", "input-group-addon");
            return BsGlyphicon(helper, icon, htmlAttributes);
        }
    }
}