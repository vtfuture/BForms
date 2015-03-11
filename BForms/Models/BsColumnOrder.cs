using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BForms.Grid
{
    public enum BsOrderType
    {
        Default = 0,
        Ascending = 1,
        Descending = 2,
    }

    public class BsColumnOrder
    {
        public string Name { get; set; }
        public BsOrderType Type { get; set; }
        public int Order { get; set; }
        public object Value { get; set; }
    }
}