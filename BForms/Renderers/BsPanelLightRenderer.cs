﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BForms.Models;
using BForms.Panels;
using BForms.Utilities;

namespace BForms.Renderers
{
    public class BsPanelLightRenderer : BsPanelBaseRenderer
    {
        public BsPanelLightRenderer() { }

        public BsPanelLightRenderer(BsPanelHtmlBuilder builder) : base(builder) { }

        public virtual string RenderHeader()
        {
            if (this.Builder.noHeader == false)
            {

                var headerTag = new TagBuilder("h3");
                headerTag.AddCssClass("panel-heading bs-panelHeader");

                if (this.Builder.glyphicon.HasValue)
                {
                    headerTag.InnerHtml += this.GetGlyphicon(this.Builder.glyphicon.Value);
                }

                headerTag.InnerHtml += " " + this.Builder.name;

                if (this.Builder.isEditable)
                {
                    headerTag.AddCssClass("editable");

                    var editableTag = new TagBuilder("a");
                    editableTag.MergeAttribute("href", "#");

                    editableTag.AddCssClass("pull-right bs-editPanel");

                    var editableGlyph = this.GetGlyphiconTag(Glyphicon.Pencil);
                    editableGlyph.AddCssClass("open-editable");

                    editableTag.InnerHtml += editableGlyph;

                    headerTag.InnerHtml += editableTag;

                    var cancelEditableTag = new TagBuilder("a");
                    cancelEditableTag.AddCssClass("pull-right bs-cancelEdit");
                    cancelEditableTag.MergeAttribute("style", "display:none");

                    var cancelEditGlyph = this.GetGlyphiconTag(Glyphicon.Remove);
                    cancelEditableTag.InnerHtml += cancelEditGlyph;

                    headerTag.InnerHtml += cancelEditableTag;
                }

                return headerTag.ToString();
            }

            return string.Empty;
        }

        public virtual string RenderContent()
        {
            var contentDiv = new TagBuilder("div");

            contentDiv.AddCssClass("bs-containerPanel");

            if (!this.Builder.isExpanded)
            {
                contentDiv.AddCssClass("collapse");
            }

            var panelBody = new TagBuilder("div");
            panelBody.AddCssClass("bs-contentPanel");
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

            container.MergeAttribute("data-settings",
               HtmlHelper.AnonymousObjectToHtmlAttributes(new
               {
                   loaded = this.Builder.isLoaded
               }).ToJsonString());

            if (this.Builder.id != null)
            {
                container.MergeAttribute("data-component", MvcHelpers.Serialize(this.Builder.id));
            }

            if (this.Builder.objId != null)
            {
                container.MergeAttribute("data-objid", MvcHelpers.Serialize(this.Builder.objId));
            }

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
