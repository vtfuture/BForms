using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BForms.Models
{
    /// <summary>
    /// Used for logging client side errors
    /// </summary>
    public class BsJavascriptErrorModel
    {
        public string ErrorMessage { get; set; }

        public string Url { get; set; }

        public int LineNumber { get; set; }

        public int ColumnNumber { get; set; }

        public DateTime Timestamp { get; set; }

        public string ErrorType { get; set; }

        public string Stack { get; set; }
    }
}
