using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BForms.Models
{
    public class BsGroupEditor<TRow>
    {
        private bool inlineSearch = true;

        public BsGridModel<TRow> Grid { get; set; }

        public bool InlineSearch
        {
            get
            {
                return this.inlineSearch;
            }
            set
            {
                this.inlineSearch = value;
            }
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
