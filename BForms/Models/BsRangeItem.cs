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
        public T MinValue
        {
            get
            {
                return _minValue;
            }
            set
            {
                _minValue = value;
                MinValueSet = true;
            }
        }

        /// <summary>
        /// Top limit for ItemValue
        /// </summary>
        public T MaxValue
        {
            get
            {
                return _maxValue;
            }
            set
            {
                _maxValue = value;
                MaxValueSet = true;
            }
        }

        /// <summary>
        /// Display text
        /// </summary>
        public string Display { get; set; }

        private T _minValue;
        private T _maxValue;

        internal bool MinValueSet
        {
            get;
            private set;
        }

        internal bool MaxValueSet
        {
            get;
            private set;
        }
    }
}
