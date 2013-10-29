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
        private Func<TRow, bool> checkbox;
        private Func<TRow, string> _detailsTemplate;

        internal Func<TRow, Dictionary<string, object>> HtmlAttr
        {
            get
            {
                return this.htmlAttributes;
            }
        }

        internal Func<TRow, bool> Details
        {
            get
            {
                return this.details;
            }
        }

        internal Func<TRow, bool> Checkbox
        {
            get
            {
                return this.checkbox;
            }
        }

        internal Func<TRow, string> Highlight
        {
            get
            {
                return this.highlighter;
            }
        }

        internal Func<TRow, string> DetailsTmpl
        {
            get
            {
                return this._detailsTemplate;
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

        public BsGridRowConfigurator<TRow> HasDetails(Func<TRow, bool> details)
        {
            this.details = details;

            return this;
        }

        public BsGridRowConfigurator<TRow> HasCheckbox(Func<TRow, bool> checkbox)
        {
            this.checkbox = checkbox;

            return this;
        }

        public BsGridRowConfigurator<TRow> DetailsTemplate(Func<TRow,string> detailsTemplate)
        {
            this._detailsTemplate = detailsTemplate;

            return this;
        }
    }
}
