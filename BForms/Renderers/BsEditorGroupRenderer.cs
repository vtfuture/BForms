using BForms.Editor;
using BForms.Models;
using BForms.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BForms.Renderers
{
    public class BsEditorGroupRenderer<TModel, TRow, TForm> : BsEditorGroupRenderer<TModel, TRow> 
        where TModel : IBsEditorGroupModel
        where TRow : BsEditorGroupItemModel<TForm>, new()
    {
        public BsEditorGroupRenderer() { }

        public BsEditorGroupRenderer(BsEditorGroupBuilder<TModel> builder)
            : base(builder) { }

        protected override void RenderForm(TagBuilder container, TRow item)
        {
            if (this.Builder.RowForms.Any())
            {
                if (item.Form != null)
                {
                    foreach (var form in this.Builder.RowForms)
                    {
                        // create prefix for inline forms (unique prefix => based on groupId/tabId/Id)
                        var uid = item.GetUniqueID().ToString() + "_" + item.TabId.ToString() + "_" + this.Builder.Uid;

                        var formBuilder = new BsEditorFormBuilder<TForm>(item.Form, uid, this.Builder.viewContext).Template(form.Value.template);

                        container.InnerHtml += new TagBuilder("hr"); // form delimiter

                        container.InnerHtml += formBuilder.ToString();
                    }
                }
                else
                {
                    var formBuilder = this.Builder.RowForms.FirstOrDefault();

                    formBuilder.Value.uid = "{{objid}}_{{tabid}}_" + this.Builder.Uid;

                    container.InnerHtml += new TagBuilder("hr"); // form delimiter

                    container.InnerHtml += formBuilder.Value.ToString();
                }
            }
            
            
        }
    }

    public class BsEditorGroupRenderer<TModel, TRow> : BsBaseRenderer<BsEditorGroupBuilder<TModel>>
        where TModel : IBsEditorGroupModel
        where TRow : BsEditorGroupItemModel, new()
    {
        public BsEditorGroupRenderer(){}

        public BsEditorGroupRenderer(BsEditorGroupBuilder<TModel> builder)
            : base(builder){}

        public override string Render()
        {
            var container = new TagBuilder("div");

            var templateRow = new TagBuilder("div");

            templateRow.AddCssClass("bs-itemTemplate");

            templateRow.MergeAttribute("style", "display: none;");

            templateRow.InnerHtml += RenderItem(new TRow()
            {
                TabId = "{{tabid}}"
            } as TRow);

            container.InnerHtml += templateRow;

            container.AddCssClass("grid_rows");

            if (this.Builder.Uid != null)
            {
                container.MergeAttribute("data-groupid", MvcHelpers.Serialize(this.Builder.Uid));
            } 

            var title = new TagBuilder("div");

            title.AddCssClass("row grid_row title");

            var div = new TagBuilder("div");

            div.AddCssClass("col-lg-12 col-md-12 col-sm-12");

            var anchor = new TagBuilder("a");

            anchor.AddCssClass("expand open bs-toggleExpand");

            anchor.MergeAttribute("href", "#");

            div.InnerHtml += anchor;

            anchor = new TagBuilder("a");

            anchor.MergeAttribute("href", "#");

            anchor.AddCssClass("bs-toggleExpand");

            anchor.InnerHtml += this.Builder.Name;

            div.InnerHtml += " " + anchor;

            title.InnerHtml += div;

            container.InnerHtml += title;

            var wrapper = new TagBuilder("div");

            wrapper.AddCssClass("grid_rows_wrapper bs-itemsWrapper");

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

        protected virtual string RenderItem(TRow item)
        {
            var container = new TagBuilder("div");

            if (item.TabId == null)
            {
                throw new Exception("Group item model must be inherited from BsGroupItemModel and must have the TabId property set");
            }

            container.MergeAttribute("data-objid", MvcHelpers.Serialize(item.GetUniqueID()));

            container.MergeAttribute("data-tabid", MvcHelpers.Serialize(item.TabId));

            container.AddCssClass("row grid_row bs-groupItem");

            var header = new TagBuilder("header");

            var leftSide = new TagBuilder("div");

            leftSide.AddCssClass("col-lg-6 col-md-6 bs-itemContent");

            leftSide.InnerHtml += this.Builder.RenderModel<TRow>(item, "");

            header.InnerHtml += leftSide;

            var rightSide = new TagBuilder("div");

            rightSide.AddCssClass("col-lg-6 col-md-6");

            rightSide.InnerHtml += RenderControls(this.Builder.EditableTabIds.Any(x => x.Equals(item.TabId)) ? false : true);

            header.InnerHtml += rightSide;

            container.InnerHtml += header;

            // if edit{

            var detailsContainer = new TagBuilder("div");

            detailsContainer.AddCssClass("row grid_row_details");

            detailsContainer.MergeAttribute("style", "display: none;");

            container.InnerHtml += detailsContainer;

            this.RenderForm(container, item);

            return container.ToString();
        }

        protected virtual void RenderForm(TagBuilder container, TRow item)
        {

        }

        protected string RenderControls(bool hidden = true)
        {
            var container = new TagBuilder("div");

            container.AddCssClass("controls btn-group btn-group-sm pull-right");

            // if edit {

            container.InnerHtml += RenderControl(Glyphicon.Pencil, "bs-editBtn", hidden);

            // }

            container.InnerHtml += RenderControl(Glyphicon.ChevronUp, "bs-upBtn");

            container.InnerHtml += RenderControl(Glyphicon.ChevronDown, "bs-downBtn");

            container.InnerHtml += RenderControl(Glyphicon.Remove, "bs-removeBtn");

            return container.ToString();
        }

        protected string RenderControl(Glyphicon glyphicon, string cssClass = null, bool hidden = false)
        {
            var anchor = new TagBuilder("a");

            if (hidden)
            {
                anchor.MergeAttribute("style", "display: none;");
            }

            anchor.MergeAttribute("href", "#");

            anchor.AddCssClass("btn btn-white");

            if (!string.IsNullOrEmpty(cssClass))
            {
                anchor.AddCssClass(cssClass);
            }

            anchor.InnerHtml += GetGlyphicon(glyphicon);

            return anchor.ToString();
        }
    }
}
