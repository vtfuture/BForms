using BForms.GroupEditor;
using BForms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BForms.Mvc;
using BForms.Grid;

namespace BForms.Renderers
{
    public class BsEditorTabBaseRenderer<TModel, TRow> : BsBaseRenderer<BsEditorTabBuilder<TModel>> where TModel : BsGroupEditor
    {
        public BsEditorTabBaseRenderer(){}

        public BsEditorTabBaseRenderer(BsEditorTabBuilder<TModel> builder)
            : base(builder){}

        protected void InitPager()
        {
            if (this.Builder.model != null)
            {
                this.Builder.pagerBuilder = new BsGridPagerBuilder(
                this.Builder.model.GetGrid<TRow>().Pager,
                this.Builder.pagerSettings,
                this.Builder.model.GetGrid<TRow>().BaseSettings);
            }
        }

        protected virtual string RenderItems()
        {
            var result = "";

            if (!string.IsNullOrEmpty(this.Builder.RowTemplate))
            {
                var list = new TagBuilder("ul");

                list.AddCssClass("group_profiles");

                foreach (var item in this.Builder.model.GetItems<TRow>())
                {
                    var listItem = new TagBuilder("li");

                    var listItemWrapper = new TagBuilder("div");

                    listItemWrapper.AddCssClass("media profile large");

                    var anchorLeft = new TagBuilder("a");

                    anchorLeft.MergeAttribute("href", "#");

                    anchorLeft.AddCssClass("pull-left");

                    var img = new TagBuilder("img");

                    img.AddCssClass("media-object");

                    img.MergeAttribute("src", "http://www.cloverwos.ro/1.0.0.217/Content/images/bg-user.png");

                    anchorLeft.InnerHtml += img;

                    listItemWrapper.InnerHtml += anchorLeft;

                    var anchorRight = new TagBuilder("a");

                    anchorRight.MergeAttribute("href", "#");

                    anchorRight.AddCssClass("btn btn-default select_profile");

                    anchorRight.InnerHtml += GetGlyphcon(Glyphicon.Ok);

                    listItemWrapper.InnerHtml += anchorRight;

                    var templateWrapper = new TagBuilder("div");

                    templateWrapper.AddCssClass("media-body");

                    templateWrapper.InnerHtml += this.Builder.viewContext.Controller.BsRenderPartialView(this.Builder.rowTemplate, item);

                    listItemWrapper.InnerHtml += templateWrapper;

                    listItem.InnerHtml += listItemWrapper;

                    list.InnerHtml += listItem;
                }

                result += list;
            }
            else
            {
                throw new Exception("You must set the template for tab " + this.Builder.uid.ToString());
            }

            return result;
        }

        protected virtual string RenderPager()
        {
            return this.Builder.pagerBuilder.ToString();
        }

        protected virtual string RenderContent()
        {
            var result = this.Builder.toolbar.renderer.Render();

            var wrapper = new TagBuilder("div");

            wrapper.AddCssClass("bs-tabContent");

            if (this.Builder.hasModel)
            {
                wrapper.InnerHtml += RenderAjax();
            }

            result += wrapper;

            return result;
        }

        public override string RenderAjax()
        {
            this.InitPager();

            var result = "";

            if (this.Builder.hasItems)
            {
                result += RenderItems();
            }

            result += RenderPager();

            return result;
        }

        public override string Render()
        {
            this.InitPager();

            var wrapper = new TagBuilder("div");

            wrapper.MergeAttribute("data-tabid", ((int)this.Builder.uid).ToString());

            wrapper.MergeAttribute("data-loaded", this.Builder.hasModel.ToString());

            if (!this.Builder.selected)
            {
                wrapper.MergeAttribute("style", "display: none;");
            }

            wrapper.InnerHtml += this.RenderContent();

            return wrapper.ToString();
        }
    }
}
