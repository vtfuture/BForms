using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BootstrapForms.Models
{
    /// <summary>
    /// Model type class from BsRangeFor
    /// </summary>
    public class BsRange<T>
    {
        /// <summary>
        /// Display value
        /// </summary>
        public string TextValue { get; set; }

        /// <summary>
        /// Begin value
        /// </summary>
        public T From { get; set; }

        /// <summary>
        /// End value
        /// </summary>
        public T To { get; set; }
    }
}
