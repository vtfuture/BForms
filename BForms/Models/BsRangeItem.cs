using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BForms.Models
{
    [Serializable]
    public class BsRangeItem<T>
    {
        /// <summary>
        /// Display value
        /// </summary>
        public string TextValue { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public T ItemValue { get; set; }

        /// <summary>
        /// Bottom limit for ItemValue
        /// </summary>
        public T MinValue { get; set; }

        /// <summary>
        /// Top limit for ItemValue
        /// </summary>
        public T MaxValue { get; set; }

        /// <summary>
        /// Display text
        /// </summary>
        public string Display { get; set; }
    }
}
