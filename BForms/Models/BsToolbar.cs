using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BForms.Models
{
    /// <summary>
    /// Helper class used when there is no need for custom object on toolbar render
    /// </summary>
    public class BsToolbarModel
    {

    }

    /// <summary>
    /// Helper class used when toolbar has advanced search form
    /// </summary>
    /// <typeparam name="TSearch"></typeparam>
    public class BsToolbarModel<TSearch>
    {
        public TSearch Search { get; set; }
    }

    /// <summary>
    /// Helper class used when toolbar has advanced search and add new entity forms
    /// </summary>
    /// <typeparam name="TSearch"></typeparam>
    /// <typeparam name="TNew"></typeparam>
    public class BsToolbarModel<TSearch, TNew>
    {
        public TSearch Search { get; set; }
        public TNew New { get; set; }
    }
}
