using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using BForms.Utilities;
using System.Web.Mvc;
using System.Reflection;

namespace BForms.Grid
{
    public class BsGridColumnFactory<TRow> where TRow : new()
    {
        private ViewContext viewContext;

        private readonly int totalColumnWidth = 12;

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

        public BsGridColumn<TRow> Add(string name)
        {
            BsGridColumn<TRow> column = new BsGridColumn<TRow>(name, this.viewContext);
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
            var column = this.Columns.Where(x => x.PrivateName == property.Name).FirstOrDefault();

            if (column == null)
            {
                throw new ArgumentException("The property you selected is not a column in your grid. Use cf.Add(x=>x.PropertyName) or decorate your property with a BsGridColumn attribute");
            }

            return column;
        }

        internal virtual void Validate()
        {
            if (!this.Columns.Any())
            {
                throw new NotImplementedException("You must define your grid columns either in model as data attributes or in the view");
            }

            foreach (BsScreenType item in Enum.GetValues(typeof(BsScreenType)))
            {
                var width = this.Columns.Select(x => x.WidthSizes.Where(y => y.ScreenType == item).Select(y => y.Size).FirstOrDefault()).Sum();
                if (width < this.totalColumnWidth)
                {
                    throw new Exception(string.Format("Total sum of grid columns width for {0} must be greater than {1}", item, this.totalColumnWidth));
                }
            }
        }
    }
}