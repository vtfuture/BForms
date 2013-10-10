using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BForms.Grid
{
    public class BsGridRepositorySettings<TSearch>
    {
        public TSearch Search { get; set; }

        public List<BsColumnOrder> OrderColumns { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }
    }
}