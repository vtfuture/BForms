using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BForms.Mvc;
using BForms.Renderers;

namespace BForms.Grid
{
    public class BsBulkSelector : BsBaseComponent
    {
        internal string text { get; set; }
        internal string styleClass { get; set; }
        public int Order { get; set; }
        public BsBulkSelectorType? Type { get; set; }

        public BsBulkSelector()
        {
            this.renderer = new BsBulkSelectorRenderer(this);
        }

        public BsBulkSelector(ViewContext viewContext)
            : base(viewContext) 
        {
            this.renderer = new BsBulkSelectorRenderer(this);
        }

        public BsBulkSelector(BsBulkSelectorType type, ViewContext viewContext)
            : base(viewContext)
        {
            this.renderer = new BsBulkSelectorRenderer(this);

            switch (type)
            {
                case BsBulkSelectorType.All:
                    this.Type = BsBulkSelectorType.All;
                    this.text = "All";
                    this.styleClass = "js-all";
                    break;

                case BsBulkSelectorType.None:
                    this.Type = BsBulkSelectorType.None;
                    this.text = "None";
                    this.styleClass = "js-none";
                    break;
            }
        }

        public BsBulkSelector Text(string text)
        {
            this.text = text;
            return this;
        }

        public BsBulkSelector StyleClass(string styleClass)
        {
            this.styleClass = styleClass;
            return this;
        }
    }
}
