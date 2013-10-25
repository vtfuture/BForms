using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BForms.Utilities;

namespace BForms.Grid
{
    public class BsGridExcelColumnFactory<TRow> where TRow : class
    {
        internal List<BsGridExcelCell<TRow>> Cells = new List<BsGridExcelCell<TRow>>();

        public BsGridExcelColumnFactory()
        {
        }

        public BsGridExcelCell<TRow> For<TValue>(Expression<Func<TRow, TValue>> expression)
        {
            var cell = new BsGridExcelCell<TRow>()
            {
                PropName = expression.GetPropertyInfo<TRow, TValue>().Name
            };
            Cells.Add(cell);

            return cell;
        }

        private BsGridExcelCell<TRow> GetCell(string propName)
        {
            return Cells.FirstOrDefault(x => string.Compare(x.PropName, propName) == 0) ?? new BsGridExcelCell<TRow>
            {
                PropName = propName
            };
        }
    }
}
