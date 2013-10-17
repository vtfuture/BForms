using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BForms.Mvc;

namespace BForms.Grid
{
    public class BsBulkSelector : BaseComponent
    {
        private string text { get; set; }
        private string styleClass { get; set; }
        public int Order { get; set; }
        public BsBulkSelectorType? Type { get; set; }

          public BsBulkSelector()
        {
        }

        public BsBulkSelector(ViewContext viewContext)
            : base(viewContext) { }

        public BsBulkSelector(BsBulkSelectorType type, ViewContext viewContext)
            : base(viewContext)
        {
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

        public override string Render()
        {
            var bulkSelector = new TagBuilder("li");
            var bulkSelectorAnchor = new TagBuilder("a");
            bulkSelectorAnchor.MergeAttribute("href","#");
            bulkSelectorAnchor.MergeAttribute("class",this.styleClass);
            bulkSelectorAnchor.InnerHtml += this.text;

            bulkSelector.InnerHtml += bulkSelectorAnchor.ToString();

            return bulkSelector.ToString();
        }
    }
}
