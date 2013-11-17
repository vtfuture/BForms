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
    public class BsEditorGroupRenderer<TModel, TRow> : BsBaseRenderer<BsEditorGroupBuilder<TModel>> where TModel : IBsEditorGroupModel
    {
        public BsEditorGroupRenderer(){}

        public BsEditorGroupRenderer(BsEditorGroupBuilder<TModel> builder)
            : base(builder){}

        public override string Render()
        {
            var container = new TagBuilder("div");

            container.AddCssClass("grid_rows");

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

            anchor.InnerHtml += this.Builder.name;

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

            span.InnerHtml += this.Builder.text;

            headerContent.InnerHtml += span;

            header.InnerHtml += headerContent;

            div.InnerHtml += header;

            wrapper.InnerHtml += div;

            container.InnerHtml += wrapper;

            return container.ToString();
        }
    }
}
