using BForms.Editor;
using BForms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BForms.Renderers
{
    public class BsEditorGroupRenderer<TModel, TRow> : BsBaseRenderer<BsEditorGroupBuilder<TModel>>
        where TModel : IBsEditorGroupModel
        where TRow : BsEditorGroupItemModel
    {
        public BsEditorGroupRenderer(){}

        public BsEditorGroupRenderer(BsEditorGroupBuilder<TModel> builder)
            : base(builder){}

        public override string Render()
        {
            var container = new TagBuilder("div");

            container.AddCssClass("grid_rows");

            if (this.Builder.Uid != null)
            {
                container.MergeAttribute("data-groupid", ((int)this.Builder.Uid).ToString());
            } 

            var title = new TagBuilder("div");

            title.AddCssClass("row grid_row title");

            var div = new TagBuilder("div");

            div.AddCssClass("col-lg-12 col-md-12 col-sm-12");

            var anchor = new TagBuilder("a");

            anchor.AddCssClass("expand bs-toggleExpand");

            anchor.MergeAttribute("href", "#");

            div.InnerHtml += anchor;

            anchor = new TagBuilder("a");

            anchor.MergeAttribute("href", "#");

            anchor.InnerHtml += this.Builder.Name;

            div.InnerHtml += " " + anchor;

            title.InnerHtml += div;

            container.InnerHtml += title;

            var wrapper = new TagBuilder("div");

            wrapper.AddCssClass("grid_rows_wrapper");

            div = new TagBuilder("div");

            div.AddCssClass("row grid_row");

            var header = new TagBuilder("header");

            var headerContent = new TagBuilder("div");

            headerContent.AddCssClass("col-lg-12 col-md-12 col-sm-12");

            var span = new TagBuilder("span");

            span.InnerHtml += this.Builder.Text;

            headerContent.InnerHtml += span;

            header.InnerHtml += headerContent;

            div.InnerHtml += header;

            wrapper.InnerHtml += div;

            if (this.Builder.Model != null)
            {
                foreach (var item in (this.Builder.Model as BsEditorGroupModel<TRow>).Items) 
                {
                    wrapper.InnerHtml += RenderItem(item);
                }
            }

            container.InnerHtml += wrapper;

            return container.ToString();
        }

        protected string RenderItem(TRow item)
        {
            var container = new TagBuilder("div");

            if (item.TabId == null)
            {
                throw new Exception("Group item model must be inherited from BsGroupItemModel and must have the TabId property set");
            }

            container.MergeAttribute("data-tabid", ((int)item.TabId).ToString());

            container.AddCssClass("row grid_row");

            var header = new TagBuilder("header");

            var leftSide = new TagBuilder("div");

            leftSide.AddCssClass("col-lg-6 col-md-6");

            leftSide.InnerHtml += this.Builder.RenderModel<TRow>(item);

            header.InnerHtml += leftSide;

            var rightSide = new TagBuilder("div");

            rightSide.AddCssClass("col-lg-6 col-md-6");

            rightSide.InnerHtml += RenderControls();

            header.InnerHtml += rightSide;

            container.InnerHtml += header;

            // if edit{

            var detailsContainer = new TagBuilder("div");

            detailsContainer.AddCssClass("row grid_row_details");

            detailsContainer.MergeAttribute("style", "display: none;");

            container.InnerHtml += detailsContainer;

            // }

            if (this.Builder.RowForms.Any())
            {
                foreach (var form in this.Builder.RowForms)
                {
                    container.InnerHtml += new TagBuilder("hr"); // form delimiter

                    container.InnerHtml += form.Value.ToString();
                }
            }

            return container.ToString();
        }

        protected string RenderControls()
        {
            var container = new TagBuilder("div");

            container.AddCssClass("controls btn-group btn-group-sm pull-right");

            // if edit {

            container.InnerHtml += RenderControl(Glyphicon.Pencil);

            // }

            container.InnerHtml += RenderControl(Glyphicon.ChevronUp);

            container.InnerHtml += RenderControl(Glyphicon.ChevronDown);

            container.InnerHtml += RenderControl(Glyphicon.Remove);

            return container.ToString();
        }

        protected string RenderControl(Glyphicon glyphicon)
        {
            var anchor = new TagBuilder("a");

            anchor.MergeAttribute("href", "#");

            anchor.AddCssClass("btn btn-default");

            anchor.InnerHtml += GetGlyphcon(glyphicon);

            return anchor.ToString();
        }
    }
}
