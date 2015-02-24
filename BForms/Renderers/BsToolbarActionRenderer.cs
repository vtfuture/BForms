using BForms.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BForms.Renderers
{
    /// <summary>
    /// For toolbar with forms
    /// </summary>
    public class BsToolbarActionRenderer<TToolbar> : BsBaseRenderer<BsToolbarAction<TToolbar>>
    {
        public BsToolbarActionRenderer()
        {

        }

        public BsToolbarActionRenderer(BsToolbarAction<TToolbar> builder)
            : base(builder)
        {

        }

        /// <summary>
        /// Renders component
        /// </summary>
        public override string Render()
        {
            var liContainer = new TagBuilder("li");

            var actionBuilder = new TagBuilder("a");

            if (this.Builder.htmlAttributes != null)
            {
                actionBuilder.MergeAttributes(this.Builder.htmlAttributes);
            }

            if (this.Builder.selected)
            {
                actionBuilder.MergeAttribute("class","selected");
            }

            actionBuilder.AddCssClass(this.Builder.descriptorClass);
            actionBuilder.AddCssClass(this.Builder.styleClasses);
            actionBuilder.MergeAttribute("href", this.Builder.href ?? "#");

            if (!string.IsNullOrEmpty(this.Builder.tabId))
            {
                actionBuilder.MergeAttribute("data-tabid", this.Builder.tabId);
            }

            if (!string.IsNullOrEmpty(this.Builder.title))
            {
                actionBuilder.MergeAttribute("title", this.Builder.title);
            }

            actionBuilder.InnerHtml += (this.Builder.glyphIcon.HasValue ? GetGlyphicon(this.Builder.glyphIcon.Value) + " " : "");

            var notXsSpan = new TagBuilder("span");
            notXsSpan.InnerHtml += this.Builder.text;

            if (!String.IsNullOrEmpty(this.Builder.xsText))
            {
                notXsSpan.AddCssClass("hidden-xs");

                var xsSpan = new TagBuilder("span");
                xsSpan.AddCssClass("visible-xs");
                xsSpan.InnerHtml += this.Builder.xsText;

                actionBuilder.InnerHtml += notXsSpan.ToString() + xsSpan;
            }
            else
            {
                actionBuilder.InnerHtml += notXsSpan;
            }


            liContainer.InnerHtml += actionBuilder;

            return liContainer.ToString();
        }

    }

    /// <summary>
    /// For toolbar without forms
    /// </summary>
    public class BsToolbarActionRenderer : BsBaseRenderer<BsToolbarAction>
    {
        public BsToolbarActionRenderer()
        {

        }

        public BsToolbarActionRenderer(BsToolbarAction builder)
            : base(builder)
        {

        }

        /// <summary>
        /// Renders component
        /// </summary>
        public override string Render()
        {
            var liContainer = new TagBuilder("li");

            var actionBuilder = new TagBuilder("a");
            actionBuilder.AddCssClass(this.Builder.descriptorClass);
            actionBuilder.AddCssClass("btn");
            actionBuilder.AddCssClass(this.Builder.styleClasses);
            actionBuilder.MergeAttribute("href", this.Builder.href ?? "#");
          

            if (!string.IsNullOrEmpty(this.Builder.tabId))
            {
                actionBuilder.MergeAttribute("data-tabid", this.Builder.tabId);
            }

            if (!string.IsNullOrEmpty(this.Builder.title))
            {
                actionBuilder.MergeAttribute("title", this.Builder.title);
            }

            actionBuilder.InnerHtml += (this.Builder.glyphIcon.HasValue ? GetGlyphicon(this.Builder.glyphIcon.Value) + " " : "") + this.Builder.text;

            liContainer.InnerHtml += actionBuilder;

            return liContainer.ToString();
        }

    }
}
