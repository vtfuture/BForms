using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BForms.Models;
using BForms.Mvc;

namespace BForms.Grid
{
    /// <summary>
    /// BulkActions component
    /// </summary>
    public class BsBulkAction : BaseComponent
    {
        private string buttonClass;
        private string title;
        private Glyphicon? glyphIcon;
        public BsBulkActionType? Type;
        private string text;
        public int? BulkActionOrder;

        public BsBulkAction()
        {
        }

        public BsBulkAction(ViewContext viewContext)
            : base(viewContext) { }

        public BsBulkAction(BsBulkActionType type, ViewContext viewContext)
            : base(viewContext)
        {
            switch (type)
            {
                case BsBulkActionType.Excel:
                    this.Type = BsBulkActionType.Excel;
                    this.buttonClass = "btn-primary js-btn-enable_selected";
                    this.title = "Export selected to excel";
                    this.glyphIcon = Glyphicon.DownloadAlt;
                    break;

                case BsBulkActionType.Delete:
                    this.Type = BsBulkActionType.Delete;
                    this.buttonClass = "btn-danger js-btn-delete_selected";
                    this.title = "Delete selected";
                    this.glyphIcon = Glyphicon.Trash;
                    break;
            }
        }

        public BsBulkAction StyleClass(string buttonClass)
        {
            this.buttonClass = buttonClass;
            return this;
        }

        public BsBulkAction Title(string title)
        {
            this.title = title;
            return this;
        }

        public BsBulkAction GlyphIcon(Glyphicon icon)
        {
            this.glyphIcon = icon;
            return this;
        }

        public BsBulkAction Text(string text)
        {
            this.text = text;
            return this;
        }

        public BsBulkAction Order(int? order)
        {
            this.BulkActionOrder = order;
            return this;
        }

        public override string Render()
        {
            var bulkButton = new TagBuilder("button");

            if (!String.IsNullOrEmpty(this.buttonClass))
            {
                bulkButton.AddCssClass(this.buttonClass);
            }

            bulkButton.AddCssClass("btn");

            bulkButton.MergeAttribute("style","display:none");

            if (!String.IsNullOrEmpty(this.title))
            {
                bulkButton.MergeAttribute("title", this.title);
            }

            bulkButton.InnerHtml += (this.glyphIcon.HasValue ? GetGlyphcon(this.glyphIcon.Value) + " " : "") + this.text;

            return bulkButton.ToString();
        }
    }
}
