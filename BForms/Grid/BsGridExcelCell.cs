using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BForms.Grid
{
    public class BsGridExcelCell<TRow> where TRow : class
    {
        public string Name { get; set; }
        public string PropName { get; set; }
        public Func<TRow, string> NameFunc { get; set; }
        public Action<TRow, BsGridExcelStyle> StyleFunc { get; set; }
        public BsGridExcelStyle CellStyle { get; set; }

        public BsGridExcelCell<TRow> Text(string text)
        {
            Name = text;
            return this;
        }

        public BsGridExcelCell<TRow> Text(Func<TRow, string> func)
        {
            this.NameFunc = func;
            return this;
        }

        public BsGridExcelCell<TRow> Style(Action<TRow, BsGridExcelStyle> func)
        {
            this.StyleFunc = func;
            return this;
        }

        public BsGridExcelCell<TRow> Style(Action<BsGridExcelStyle> style)
        {
            this.CellStyle = new BsGridExcelStyle();
            style(this.CellStyle);
            return this;
        }
    }
}
