using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BForms.Mvc
{
    [AttributeUsage(AttributeTargets.Property)]
    public class BsPanelAttribute :Attribute
    {
        public bool IsLarge { get; set; }

        public object Id { get; set; }
    }
}
