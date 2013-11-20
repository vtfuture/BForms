using BForms.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BForms.Renderers
{
    public class BsEditorRenderer<TModel> : BsBaseRenderer<BsEditorHtmlBuilder<TModel>>
    {
        public BsEditorRenderer(){}

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
                    result += tab.Value.renderer.RenderAjax();
                }
            }

            return result;
        }

        public string RenderTabs()
        {
            var result = this.Builder.TabConfigurator.NavigationBuilder.ToString();

            foreach (var tab in this.Builder.TabConfigurator.Tabs)
            {
                result += tab.Value.ToString();
            }

            return result;
        }

        public string RenderGroups()
        {
            var div = new TagBuilder("div");

            div.AddCssClass("grid_view");

            foreach (var group in this.Builder.GroupConfigurator.Groups)
            {
                div.InnerHtml += group.Value.ToString();
            }

            return div.ToString();
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

            left.AddCssClass("left");

            left.InnerHtml += RenderTabs();

            container.InnerHtml += left;
            #endregion

            #region Right
            var right = new TagBuilder("div");

            right.AddCssClass("right");

            right.InnerHtml += RenderGroups();

            container.InnerHtml += right;
            #endregion

            return container.ToString();
        }
    }
}
