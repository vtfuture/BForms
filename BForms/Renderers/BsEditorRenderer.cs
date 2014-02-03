using BForms.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BForms.Models;
using BForms.Utilities;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.EMMA;

namespace BForms.Renderers
{
    public class BsEditorRenderer<TModel> : BsBaseRenderer<BsEditorHtmlBuilder<TModel>>
    {
        public BsEditorRenderer() { }

        public BsEditorRenderer(BsEditorHtmlBuilder<TModel> builder)
            : base(builder)
        {

        }

        public override string Render()
        {
            var result = this.Builder.IsAjaxRequest() ?
                this.RenderAjax() :
                this.RenderIndex();
            return result;
        }

        public string RenderAjax()
        {
            var result = "";

            foreach (var tab in this.Builder.TabConfigurator.Tabs)
            {
                if (tab.Value.HasModel)
                {
                    tab.Value.Theme = this.Builder.Theme;

                    result += tab.Value.renderer.RenderAjax();
                }
            }

            return result;
        }

        public string RenderTabs()
        {
            var result = string.Empty;

            if (!string.IsNullOrEmpty(this.Builder.TabConfigurator.Title))
            {
                result += RenderTitle(this.Builder.TabConfigurator.Title);
            }

            if (this.Builder.TabConfigurator.Tabs.Count() > 1)
            {
                result += this.Builder.TabConfigurator.NavigationBuilder.ToString();
            }

            foreach (var tab in this.Builder.TabConfigurator.Tabs)
            {
                tab.Value.Theme = this.Builder.Theme;

                var bulkMoveHtml = this.RenderMoveToGroups();

                tab.Value.bulkMoveHtml = bulkMoveHtml;

                result += tab.Value.ToString();
            }

            return result;
        }

        public string RenderGroups()
        {
            var result = string.Empty;

            if (!string.IsNullOrEmpty(this.Builder.GroupConfigurator.Title))
            {
                result += RenderTitle(this.Builder.GroupConfigurator.Title);
            }

            var div = new TagBuilder("div");

            div.AddCssClass("grid_view");

            if (!String.IsNullOrEmpty(this.Builder.GroupConfigurator.FormHtml))
            {
                var formWrapper = new TagBuilder("div");
                formWrapper.AddCssClass("inline");
                formWrapper.AddCssClass("search");
                formWrapper.AddCssClass("bs-groupForm");

                formWrapper.InnerHtml += this.Builder.GroupConfigurator.FormHtml;

                div.InnerHtml += formWrapper.ToString();
            }

            foreach (var group in this.Builder.GroupConfigurator.Groups)
            {
                div.InnerHtml += group.Value.ToString();
            }

            result += div.ToString();

            return result;
        }

        public string RenderGroupsFooter()
        {

            var cssClass = "col-lg-6 col-md-6 col-sm-6";

            var counter = new TagBuilder("div");
            counter.AddCssClass("row counter");
            var total = new TagBuilder("div");
            total.AddCssClass(cssClass);
            var span = new TagBuilder("span");
            span.AddCssClass("bs-counter");
            total.InnerHtml += "Total: " + span;
            counter.InnerHtml += total;

            var reset = new TagBuilder("div");
            reset.AddCssClass(cssClass);

            var anchor = new TagBuilder("a");
            anchor.MergeAttribute("href", "#");
            anchor.AddCssClass("btn btn-white pull-right bs-resetGroupEditor");
            anchor.InnerHtml += GetGlyphicon(Models.Glyphicon.Refresh);
            anchor.InnerHtml += " " + BsResourceManager.Resource("Reset");

            if (!String.IsNullOrEmpty(this.Builder.saveUrl))
            {
                var saveAnchor = new TagBuilder("a");
                saveAnchor.MergeAttribute("href", this.Builder.saveUrl);
                saveAnchor.MergeAttribute("style", "margin-left:10px");
                saveAnchor.AddCssClass("btn btn-white pull-right bs-saveGroupEditor");
                saveAnchor.InnerHtml += GetGlyphicon(Models.Glyphicon.Save);
                saveAnchor.InnerHtml += " " + BsResourceManager.Resource("Save");

                reset.InnerHtml += saveAnchor;
            }


            reset.InnerHtml += anchor;

            counter.InnerHtml += reset;

            return counter.ToString();
        }

        public string RenderTitle(string title)
        {
            var divToolbar = new TagBuilder("div");
            divToolbar.AddCssClass("grid_toolbar");

            var theme = this.Builder.Theme.GetDescription();
            divToolbar.AddCssClass(theme);

            var divToolbarHeader = new TagBuilder("div");
            divToolbarHeader.AddCssClass("grid_toolbar_header");
            var toolbarH = new TagBuilder("h1");
            toolbarH.InnerHtml += title;

            divToolbarHeader.InnerHtml += toolbarH;
            divToolbar.InnerHtml += divToolbarHeader;

            return divToolbar.ToString();
        }

        public string RenderMoveToGroups()
        {

            if (this.Builder.GroupConfigurator.Groups.Any())
            {
                var button = new TagBuilder("button");

                var glyph = GetGlyphiconTag(Glyphicon.ShareAlt);

                if (this.Builder.GroupConfigurator.Groups.Count > 1)
                {
                    var divContainer = new TagBuilder("div");
                    divContainer.AddCssClass("btn-white btn pull-right bs-bulkGroupMove");
                    divContainer.MergeAttribute("style", "margin: 0 10px 10px 0");

                    var dropdownA = new TagBuilder("a");
                    
                    dropdownA.MergeAttribute("data-toggle", "dropdown");
                    dropdownA.MergeAttribute("href", "#");
                    dropdownA.InnerHtml += BsResourceManager.Resource("GroupEditorMoveToGroups");
                    dropdownA.InnerHtml += glyph;

                    var dropdownUl = new TagBuilder("ul");
                    dropdownUl.AddCssClass("dropdown-menu");
                    dropdownUl.MergeAttribute("style", "top:auto");

                    foreach (var group in this.Builder.GroupConfigurator.Groups)
                    {
                        var li = new TagBuilder("li");

                        var a = new TagBuilder("a");
                        a.MergeAttribute("href", "#");
                        a.MergeAttribute("class", "bs-moveToGroupBtn");
                        a.MergeAttribute("data-groupid", MvcHelpers.Serialize(group.Value.Uid));
                        a.InnerHtml += group.Value.Name;

                        li.InnerHtml += a;
                        dropdownUl.InnerHtml += li;
                    }

                    divContainer.InnerHtml += dropdownA;
                    divContainer.InnerHtml += dropdownUl;

                    return divContainer.ToString();
                }
                else
                {
                    button.MergeAttribute("style", "margin: 0 10px 10px 0");
                    button.AddCssClass("btn-white btn pull-right bs-bulkGroupMove");
                    button.InnerHtml += BsResourceManager.Resource("GroupEditorMoveToGroups");
                    button.InnerHtml += glyph;
                    button.AddCssClass("bs-moveToGroupBtn");
                    button.MergeAttribute("data-groupid", MvcHelpers.Serialize(this.Builder.GroupConfigurator.Groups.First().Value.Uid));

                    return button.ToString();
                }

            }
            return string.Empty;
        }

        public string RenderIndex()
        {
            var container = new TagBuilder("div");

            container.AddCssClass("group_editor");

            if (this.Builder.htmlAttributes != null)
            {
                container.MergeAttributes(this.Builder.htmlAttributes);
            }

            #region Left
            var left = new TagBuilder("div");

            left.AddCssClass("left bs-tabs");

            left.InnerHtml += RenderTabs();

            container.InnerHtml += left;
            #endregion

            #region Right
            var right = new TagBuilder("div");

            right.AddCssClass("right bs-groups");

            right.InnerHtml += RenderGroups();

            right.InnerHtml += RenderGroupsFooter();

            container.InnerHtml += right;
            #endregion

            return container.ToString();
        }
    }
}
