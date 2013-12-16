using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BForms.Models;
using BForms.Mvc;
using BForms.Renderers;

namespace BForms.Grid
{
    /// <summary>
    /// BulkActions component
    /// </summary>
    public class BsBulkAction : BsBaseComponent<BsBulkAction>
    {
        internal string buttonClass;
        internal string title;
        internal Glyphicon? glyphIcon;
        public BsBulkActionType? Type;
        internal bool ignore;
        internal string text;
        public int? BulkActionOrder;

        public BsBulkAction()
        {
            this.renderer = new BsBulkActionRenderer(this);
        }

        public BsBulkAction(ViewContext viewContext)
            : base(viewContext) 
        {
            this.renderer = new BsBulkActionRenderer(this);
        }

        public BsBulkAction(BsBulkActionType type, ViewContext viewContext)
            : base(viewContext)
        {
            this.renderer = new BsBulkActionRenderer(this);

            switch (type)
            {
                case BsBulkActionType.Excel:
                    this.Type = BsBulkActionType.Excel;
                    this.buttonClass = "btn-primary js-btn-exportExcel_selected";
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

        public BsBulkAction Ignore(bool ignore = true)
        {
            this.ignore = ignore;
            return this;
        }
    }
}
