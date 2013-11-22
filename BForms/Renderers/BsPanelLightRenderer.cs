﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BForms.Models;
using BForms.Panels;

namespace BForms.Renderers
{
    public class BsPanelLightRenderer : BsPanelBaseRenderer
    {
        public BsPanelLightRenderer() { }

        public BsPanelLightRenderer(BsPanelHtmlBuilder builder) : base(builder) { }

        public virtual string RenderHeader()
        {
            var headerTag = new TagBuilder("h3");
            headerTag.AddCssClass("panel-heading");

            headerTag.InnerHtml += this.GetGlyphicon(Glyphicon.User);

            headerTag.InnerHtml += " " + this.Builder.name;

            if (this.Builder.isEditable)
            {
                headerTag.AddCssClass("editable");
                var editableGlyph = this.GetGlyphiconTag(Glyphicon.Pencil);
                editableGlyph.AddCssClass("open-editable");

                headerTag.InnerHtml += editableGlyph;
            }

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
