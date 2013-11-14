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
        
    }

    public class BsGroupEditor<TRow> : BsGroupEditor
    {
        public BsGridModel<TRow> Grid { get; set; }
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
