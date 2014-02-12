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
    public class BsToolbarItemGroupActionRenderer<TToolbar> : BsBaseRenderer<BsToolbarItemGroupActionLink<TToolbar>>
    {
        public BsToolbarItemGroupActionRenderer()
        {

        }

        public BsToolbarItemGroupActionRenderer(BsToolbarItemGroupActionLink<TToolbar> builder)
            : base(builder)
        {

        }

        /// <summary>
        /// Renders component
        /// </summary>
        public override string Render()
        {
            var actionBuilder = new TagBuilder("a");

            if (this.Builder.htmlAttributes != null)
            {
                actionBuilder.MergeAttributes(this.Builder.htmlAttributes);
            }

            actionBuilder.AddCssClass(this.Builder.descriptorClass);
            actionBuilder.AddCssClass(this.Builder.styleClasses);
            actionBuilder.MergeAttribute("href", this.Builder.href ?? "#");

            if (!string.IsNullOrEmpty(this.Builder.title))
            {
                actionBuilder.MergeAttribute("title", this.Builder.title);
            }

            actionBuilder.InnerHtml += (this.Builder.glyphIcon.HasValue
                ? GetGlyphicon(this.Builder.glyphIcon.Value) + " "
                : "") + this.Builder.text;

            return actionBuilder.ToString();
        }
    }

    /// <summary>
    /// For toolbar without forms
    /// </summary>
    public class BsToolbarItemGroupActionRenderer : BsBaseRenderer<BsToolbarItemGroupActionLink>
    {
        public BsToolbarItemGroupActionRenderer()
        {

        }

        public BsToolbarItemGroupActionRenderer(BsToolbarItemGroupActionLink builder)
            : base(builder)
        {

        }

        /// <summary>
        /// Renders component
        /// </summary>
        public override string Render()
        {
            var actionBuilder = new TagBuilder("a");
            actionBuilder.AddCssClass(this.Builder.descriptorClass);
            actionBuilder.AddCssClass(this.Builder.styleClasses);
            actionBuilder.MergeAttribute("href", this.Builder.href ?? "#");

            if (!string.IsNullOrEmpty(this.Builder.title))
            {
                actionBuilder.MergeAttribute("title", this.Builder.title);
            }

            actionBuilder.InnerHtml += (this.Builder.glyphIcon.HasValue
                ? GetGlyphicon(this.Builder.glyphIcon.Value) + " "
                : "") + this.Builder.text;

            return actionBuilder.ToString();
        }
    }
}
