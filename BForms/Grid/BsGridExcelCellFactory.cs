using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BForms.Utilities;

namespace BForms.Grid
{
    public class BsGridExcelCellFactory<TRow> where TRow : class
    {
        public BsGridExcelStyle Style = new BsGridExcelStyle();
        public List<BsGridExcelCell<TRow>> Cells = new List<BsGridExcelCell<TRow>>();

        public BsGridExcelCellFactory()
        {
            Style = new BsGridExcelStyle();
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

        public BsGridExcelCell<TRow> Name<TValue>(Expression<Func<TRow, TValue>> expression, string text)
        {
            var propName = expression.GetPropertyInfo<TRow, TValue>().Name;
            var cell = GetCell(propName);
            cell.Name = text;
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
