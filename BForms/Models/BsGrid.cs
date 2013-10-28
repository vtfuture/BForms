using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BForms.Models
{
    public class BsGridModel<T>
    {
        public IEnumerable<T> Items { get; set; }

        public BsPagerModel Pager { get; set; }

        public Dictionary<string, int> OrderColumns { get; set; } 
    }

    public class BsGridRowData
    {
        public int Id { get; set; }
        public bool GetDetails { get; set; }
    }
}
