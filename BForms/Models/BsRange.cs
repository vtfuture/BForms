using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BForms.Models
{
    /// <summary>
    /// Model type class from BsRangeFor
    /// </summary>
    [Serializable]
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
        /// Display text for begin value
        /// </summary>
        public string TextFrom { get; set; }

        /// <summary>
        /// Lower limit
        /// </summary>
        public T MinFrom { get; set; }

        /// <summary>
        /// End value
        /// </summary>
        public T To { get; set; }

        /// <summary>
        /// Display text for end value
        /// </summary>
        public string TextTo { get; set; }

        /// <summary>
        /// Top limit
        /// </summary>
        public T MaxTo { get; set; }
    }
}
