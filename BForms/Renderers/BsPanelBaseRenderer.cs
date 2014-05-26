using System;
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
    public class BsPanelBaseRenderer : BsBaseRenderer<BsPanelHtmlBuilder>
    {
        public BsPanelBaseRenderer() { }

        public BsPanelBaseRenderer(BsPanelHtmlBuilder builder) : base(builder) { }

        private string GetThemeClass()
        {
            switch (this.Builder._theme)
            {
                case BsPanelTheme.Blue:
                    return "panel-primary";
                case BsPanelTheme.Default:
                    return "panel-default";
                case BsPanelTheme.LightGreen:
                    return "panel-success";
                case BsPanelTheme.LightBlue:
                    return "panel-info";
                case BsPanelTheme.LightYellow:
                    return "panel-warning";
                case BsPanelTheme.Red:
                    return "panel-danger";
            }

            return "panel-default";
        }

        public virtual string RenderHeader()
        {
            if (this.Builder.noHeader == false)
            {
                var headerTag = new TagBuilder("div");
                headerTag.AddCssClass("panel-heading bs-panelHeader");

                var headerTitleTag = new TagBuilder("h4");
                headerTitleTag.AddCssClass("panel-title");

                var nameTag = new TagBuilder("a");
                nameTag.AddCssClass("bs-togglePanel");
                nameTag.MergeAttribute("href", "#");

                if (this.Builder.glyphicon.HasValue)
                {
                    nameTag.MergeAttribute("style", "margin-left: 5px;");
                    headerTitleTag.InnerHtml += this.GetGlyphicon(this.Builder.glyphicon.Value);
                }

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

                if (this.Builder.isEditable && (!this.Builder.mode.HasValue || this.Builder.mode.Value == BsPanelMode.Both))
                {
                    var editableTag = new TagBuilder("a");
                    editableTag.MergeAttribute("href", this.Builder.editableUrl);
                    editableTag.AddCssClass("pull-right bs-editPanel");

                    var glyphTag = this.GetGlyphicon(Glyphicon.Pencil);

                    editableTag.InnerHtml += glyphTag;

                    var cancelEditableTag = new TagBuilder("a");
                    cancelEditableTag.MergeAttribute("href", "#");
                    cancelEditableTag.AddCssClass("pull-right bs-cancelEdit");

                    if (!this.Builder.isLoaded)
                    {
                        editableTag.MergeAttribute("style", "display:none");
                        cancelEditableTag.MergeAttribute("style", "display:none");
                    }
                    else
                    {
                        if (this.Builder.initialReadonly)
                        {
                            cancelEditableTag.MergeAttribute("style", "display:none");
                        }
                        else
                        {
                            editableTag.MergeAttribute("style", "display:none");
                        }
                    }

                    var cancelGlyphTag = this.GetGlyphicon(Glyphicon.Remove);

                    cancelEditableTag.InnerHtml += cancelGlyphTag;

                    headerTitleTag.InnerHtml += editableTag.ToString();
                    headerTitleTag.InnerHtml += cancelEditableTag.ToString();
                }

                headerTag.InnerHtml += headerTitleTag.ToString();
                return headerTag.ToString();
            }

            return string.Empty;
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

            container.MergeAttribute("data-settings",
                HtmlHelper.AnonymousObjectToHtmlAttributes(new
                {
                    loaded = this.Builder.isLoaded,
                    initialReadonly = this.Builder.initialReadonly,
                    editable = this.Builder.isEditable
                }).ToJsonString());

            if (this.Builder.id != null)
            {
                container.MergeAttribute("data-component", MvcHelpers.Serialize(this.Builder.id));
            }

            container.AddCssClass("panel");
            container.AddCssClass(this.GetThemeClass());

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

