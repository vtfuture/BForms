using BForms.Mvc;
using BForms.Renderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BForms.Editor
{
    #region BsEditorToolbarForm
    public class BsEditorToolbarFormBuilder<TModel> : BsBaseComponent
    {
        #region Properties and Constructor
        protected TModel model { get; set; }
        internal string uid { get; set; }
        internal bool hide { get; set; }

        internal TModel Model
        {
            get
            {
                return this.model;
            }
        }

        public BsEditorToolbarFormBuilder()
        {
            this.renderer = new BsEditorToolbarFormRenderer<TModel>(this);
        }

        public BsEditorToolbarFormBuilder(TModel model, string uid, ViewContext viewContext)
        {
            this.model = model;
            this.uid = uid;
            this.viewContext = viewContext;
            this.renderer = new BsEditorToolbarFormRenderer<TModel>(this);
        }
        #endregion

        #region Public Methods
        public BsEditorToolbarFormBuilder<TModel> HtmlAttributes(Dictionary<string, object> htmlAttributes)
        {
            base.HtmlAttributes(htmlAttributes);

            return this;
        }

        public BsEditorToolbarFormBuilder<TModel> Template(string template)
        {
            this.template = template;

            return this;
        }

        public BsEditorToolbarFormBuilder<TModel> Hide()
        {
            this.hide = true;

            return this;
        }
        #endregion
    }
    #endregion
}
