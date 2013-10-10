using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BForms.Models
{
    public class BsToolbarModel<TSearch>
    {
        public TSearch Search { get; set; }
    }

    public class BsToolbarModel<TSearch, TNew>
    {
        public TSearch Search { get; set; }
        public TNew New { get; set; }
    }
}
