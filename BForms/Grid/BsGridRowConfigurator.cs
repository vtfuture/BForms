using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BForms.Grid
{
    public class BsGridRowConfigurator<TRow> where TRow : new()
    {
        private Func<TRow, Dictionary<string, object>> htmlAttributes;
        private Func<TRow, string> highlighter;
        private Func<TRow, bool> details;

        public Func<TRow, Dictionary<string, object>> HtmlAttr
        {
            get
            {
                return this.htmlAttributes;
            }
        }

        public Func<TRow, bool> HasDetails
        {
            get
            {
                return this.details;
            }
        }

        public Func<TRow, string> Highlight
        {
            get
            {
                return this.highlighter;
            }
        }

        public BsGridRowConfigurator()
        {

        }

        public BsGridRowConfigurator<TRow> HtmlAttributes(Func<TRow, Dictionary<string, object>> htmlAttributes)
        {
            this.htmlAttributes = htmlAttributes;

            return this;
        }

        public BsGridRowConfigurator<TRow> Highlighter(Func<TRow, string> highlighter)
        {
            this.highlighter = highlighter;

            return this;
        }

        public BsGridRowConfigurator<TRow> Details(Func<TRow, bool> details)
        {
            this.details = details;

            return this;
        }
    }
}
