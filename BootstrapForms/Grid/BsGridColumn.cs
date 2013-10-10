using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace BootstrapForms.Grid
{
    public class BsGridColumn<TRow> : BaseComponent where TRow : new()
    {
        public PropertyInfo Property { get; set; }

        public bool IsSortable { get; set; }

        public bool IsEditable { get; set; }

        public string EditableContent { get; set; }

        public string DisplayName { get; set; }

        public int Width { get; set; }

        public Func<TRow, object> CellText { get; set; }

        public BsGridColumn(ViewContext viewContext) : base(viewContext) { }

        public BsGridColumn(PropertyInfo property, ViewContext viewContext)
            : base(viewContext)
        {
            this.Property = property;
        }

        public BsGridColumn<TRow> Name(string name)
        {
            this.DisplayName = name;
            return this;
        }

        public BsGridColumn<TRow> Editable(Func<TRow, string> configurator)
        {
            this.EditableContent = configurator(new TRow());
            this.IsEditable = true;
            return this;
        }

        public BsGridColumn<TRow> Sortable()
        {
            this.IsSortable = true;
            return this;
        }

        public BsGridColumn<TRow> SetWidth(int width)
        {
            this.Width = width;
            return this;
        }

        public BsGridColumn<TRow> Text(Func<TRow, object> cellText)
        {
            this.CellText = cellText;
            return this;
        }

        public override string Render()
        {
            var columnBuilder = new TagBuilder("div");
            if (this.IsSortable)
            {
                var linkBuilder = new TagBuilder("a");
                linkBuilder.MergeAttribute("data-name", this.Property.Name);
                linkBuilder.MergeAttribute("href", "#");
                linkBuilder.InnerHtml = this.DisplayName;

                columnBuilder.InnerHtml += linkBuilder.ToString();
            }
            else
            {
                columnBuilder.InnerHtml += this.DisplayName;
            }

            if (this.IsEditable)
            {
                columnBuilder.InnerHtml += this.EditableContent;
            }

            columnBuilder.MergeAttribute("class", "col-lg-" + this.Width);

            return columnBuilder.ToString();
        }
    }
}