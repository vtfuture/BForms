using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BForms.Panels;

namespace BForms.Renderers
{
    public class BsPanelBaseRenderer : BsBaseRenderer<BsPanelHtmlBuilder>
    {
        public BsPanelBaseRenderer(){}

        public BsPanelBaseRenderer(BsPanelHtmlBuilder builder) : base(builder) { }

        public virtual string RenderHeader()
        {
            var headerTag = new TagBuilder("div");
            headerTag.AddCssClass("panel-heading");

            var headerTitleTag = new TagBuilder("h4");
            headerTitleTag.AddCssClass("panel-title");

            var nameTag = new TagBuilder("a");
            nameTag.AddCssClass("bs-togglePanel");
            nameTag.MergeAttribute("href", "#");

            var loaderImg = new TagBuilder("span");
            loaderImg.AddCssClass("bs-panelLoader loading-spinner");

            if (this.Builder.isLoaded)
            {
                loaderImg.MergeAttribute("style", "display:none");
            }

            nameTag.InnerHtml += loaderImg.ToString();

            if (this.Builder.isExpandable)
            {
                var caretTag = new TagBuilder("span");
                caretTag.AddCssClass("caret bs-panelCaret");

                if (!this.Builder.isLoaded)
                {
                    caretTag.MergeAttribute("style", "display:none");
                }

                nameTag.InnerHtml += caretTag.ToString();

                if (this.Builder.isExpanded)
                {
                    nameTag.AddCssClass("dropup");
                }

                nameTag.MergeAttribute("data-expandable", "true");
            }


            nameTag.InnerHtml += this.Builder.name;

            headerTitleTag.InnerHtml += nameTag.ToString();

            if (this.Builder.isEditable)
            {
                var editableTag = new TagBuilder("a");
                editableTag.MergeAttribute("href", "#");
                editableTag.AddCssClass("pull-right bs-editPanel");

                var glyphTag = new TagBuilder("span");
                glyphTag.AddCssClass("glyphicon glyphicon-pencil");

                editableTag.InnerHtml += glyphTag.ToString();

                if (!this.Builder.isLoaded)
                {
                    editableTag.MergeAttribute("style", "display:none");
                }

                var cancelEditableTag = new TagBuilder("a");
                cancelEditableTag.MergeAttribute("href", "#");
                cancelEditableTag.AddCssClass("pull-right bs-cancelEdit");
                cancelEditableTag.MergeAttribute("style", "display:none");


                var cancelGlyphTag = new TagBuilder("span");
                cancelGlyphTag.AddCssClass("glyphicon glyphicon-remove");

                cancelEditableTag.InnerHtml += cancelGlyphTag.ToString();

                headerTitleTag.InnerHtml += editableTag.ToString();
                headerTitleTag.InnerHtml += cancelEditableTag.ToString();
            }

            headerTag.InnerHtml += headerTitleTag.ToString();
            return headerTag.ToString();
        }

        public virtual string RenderContent()
        {
            var contentDiv = new TagBuilder("div");

            contentDiv.AddCssClass("panel-collapse bs-containerPanel");

            if (!this.Builder.isExpanded)
            {
                contentDiv.AddCssClass("collapse");
            }


            var panelBody = new TagBuilder("div");
            panelBody.AddCssClass("panel-body bs-contentPanel");
            panelBody.InnerHtml += this.Builder.content;

            contentDiv.InnerHtml += panelBody.ToString();

            return contentDiv.ToString();
        }

        public virtual TagBuilder GetContainer(out TagBuilder body)
        {
            var container = new TagBuilder("div");

            container.MergeAttributes(this.Builder.htmlAttributes, true);

            if (!String.IsNullOrEmpty(this.Builder.editableUrl))
            {
                container.MergeAttribute("data-editableurl", this.Builder.editableUrl);
            }

            if (!String.IsNullOrEmpty(this.Builder.readonlyUrl))
            {
                container.MergeAttribute("data-readonlyurl", this.Builder.readonlyUrl);
            }

            if (!String.IsNullOrEmpty(this.Builder.saveUrl))
            {
                container.MergeAttribute("data-saveurl", this.Builder.saveUrl);
            }

            container.MergeAttribute("data-component", this.Builder.id.ToString());

            container.AddCssClass("panel panel-default");

            body = container;

            return container;
        }

        public override string Render()
        {
            TagBuilder result;

            var container = this.GetContainer(out result);

            container.InnerHtml += this.RenderHeader();

            container.InnerHtml += this.RenderContent();

            return result.ToString();
        }
    }
}
