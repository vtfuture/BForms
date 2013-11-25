using System;

namespace BForms.Models
{
    /// <summary>
    /// Model type class from BsDateTimeFor and BsDateTime
    /// </summary>
    [Serializable]
    public class BsDateTime
    {
        /// <summary>
        /// Display value
        /// </summary>
        public string TextValue { get; set; }

        /// <summary>
        /// Datetime value
        /// </summary>
        public DateTime? DateValue { get; set; }
    }
}
