using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using BootstrapForms.Utilities;
using System.Web.Mvc;
using System.Reflection;

namespace BootstrapForms.Grid
{
    public class BsGridColumnFactory<TRow> where TRow : new()
    {
        private ViewContext viewContext;

        private List<BsGridColumn<TRow>> columns;

        public List<BsGridColumn<TRow>> Columns
        {
            get
            {
                return this.columns;
            }
        }

        public BsGridColumnFactory(ViewContext viewContext)
            : this(viewContext, new List<BsGridColumn<TRow>>())
        { }

        public BsGridColumnFactory(ViewContext viewContext, List<BsGridColumn<TRow>> columns)
        {
            this.viewContext = viewContext;
            this.columns = columns;
        }

        public BsGridColumn<TRow> Add<TValue>(Expression<Func<TRow, TValue>> expression)
        {
            this.Remove(expression);

            BsGridColumn<TRow> column = new BsGridColumn<TRow>(expression.GetPropertyInfo<TRow, TValue>(), this.viewContext);
            this.columns.Add(column);

            return column;
        }

        public BsGridColumn<TRow> For<TValue>(Expression<Func<TRow, TValue>> expression)
        {
            BsGridColumn<TRow> column = this.GetColumn(expression.GetPropertyInfo<TRow, TValue>());
            return column;
        }

        public void Remove<TValue>(Expression<Func<TRow, TValue>> expression)
        {
            BsGridColumn<TRow> column = this.GetColumn(expression.GetPropertyInfo<TRow, TValue>());
            if (column != null)
            {
                this.columns.Remove(column);
            }
        }
        
        private BsGridColumn<TRow> GetColumn(PropertyInfo property)
        {
            var column = this.Columns.Where(x => x.Property.Name == property.Name).FirstOrDefault();

            if (column == null)
            {
                throw new ArgumentException("The property you selected is not a column in your grid. Use cf.Add(x=>x.PropertyName) or decorate your property with a BsGridColumn attribute");
            }

            return column;
        }
    }
}