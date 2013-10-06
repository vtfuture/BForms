using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace BootstrapForms.Grid
{
    public class BsGridColumn<TRow> where TRow : new()
    {
        public PropertyInfo Property { get; set; }

        public bool IsSortable { get; set; }

        public bool IsEditable { get; set; }

        public string EditableContent { get; set; }

        public string DisplayName { get; set; }

        public int Width { get; set; }

        public Func<TRow, string> CellText { get; set; }

        public BsGridColumn()
        {
        }

        public BsGridColumn(PropertyInfo property)
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

        public BsGridColumn<TRow> Text(Func<TRow, string> cellText)
        {
            this.CellText = cellText;
            return this;
        }
    }
}