using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BForms.Grid
{
    public class BsGridRepositorySettings<TSearch>
    {
        public TSearch Search { get; set; }

        public string QuickSearch { get; set; }

        public List<BsColumnOrder> OrderableColumns { get; set; } // order grid by column

        public Dictionary<string, int> OrderColumns { get; set; } // swap columns order

        public int Page { get; set; }

        public int PageSize { get; set; }
    }
}