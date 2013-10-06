using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using BootstrapForms.Utilities;

namespace BootstrapForms.Grid
{
    public class BsGridColumnFactory<TRow> where TRow : new()
    {
        private List<BsGridColumn<TRow>> columns = new List<BsGridColumn<TRow>>();
        public List<BsGridColumn<TRow>> Columns
        {
            get
            {
                return this.columns;
            }
        }

        public BsGridColumn<TRow> Add<TValue>(Expression<Func<TRow, TValue>> expression)
        {
            BsGridColumn<TRow> column = new BsGridColumn<TRow>(expression.GetPropertyInfo<TRow, TValue>());
            this.columns.Add(column);
            return column;
        }
    }
}