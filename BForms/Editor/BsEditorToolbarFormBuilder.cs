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
    public class BsEditorFormBuilder : BsBaseComponent<BsEditorFormBuilder>
    {
        internal object uid { get; set; }
    }

    #region BsEditorToolbarForm
    public class BsEditorFormBuilder<TModel> : BsEditorFormBuilder
    {
        #region Properties and Constructor
        protected TModel model { get; set; }
        internal bool hide { get; set; }

        internal TModel Model
        {
            get
            {
                return this.model;
            }
        }

        public BsEditorFormBuilder()
        {
            this.renderer = new BsEditorFormRenderer<TModel>(this);
        }

        public BsEditorFormBuilder(TModel model, object uid, ViewContext viewContext)
        {
            this.model = model;
            this.uid = uid;
            this.viewContext = viewContext;
            this.renderer = new BsEditorFormRenderer<TModel>(this);
        }
        #endregion

        #region Public Methods
        public BsEditorFormBuilder<TModel> Template(string template)
        {
            this.template = template;

            return this;
        }

        public BsEditorFormBuilder<TModel> Hide()
        {
            this.hide = true;

            return this;
        }
        #endregion
    }
    #endregion
}
