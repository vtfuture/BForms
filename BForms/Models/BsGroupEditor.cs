using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BForms.Utilities;

namespace BForms.Models
{
    public abstract class BsGroupEditor
    {
        internal abstract BsGridModel<TRow> GetGrid<TRow>();

        internal abstract IEnumerable<TRow> GetItems<TRow>();
    }

    public class BsGroupEditor<TRow> : BsGroupEditor
    {
        public BsGridModel<TRow> Grid { get; set; }

        internal override BsGridModel<T> GetGrid<T>()
        {
            return this.Grid as BsGridModel<T>;
        }

        internal override IEnumerable<TRow> GetItems<TRow>()
        {
            if (this.Grid == null)
            {
                throw new Exception("Grid is null");
            }
            return this.Grid.Items as IEnumerable<TRow>;
        }
    }

    public class BsGroupEditor<TRow, TSearch> : BsGroupEditor<TRow>
    {
        public TSearch Search { get; set; }
    }

    public class BsGroupEditor<TRow, TSearch, TNew> : BsGroupEditor<TRow, TSearch>
    {
        public TNew New { get; set; }
    }
}
