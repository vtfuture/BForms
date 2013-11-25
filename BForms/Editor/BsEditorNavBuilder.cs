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
    public class BsEditorNavBuilder : BsBaseComponent<BsEditorNavBuilder>
    {
        #region Properties and Constructor
        internal List<BsEditorTabAttribute> TabsProperties { get; set; }

        public BsEditorNavBuilder(ViewContext viewContext) : base(viewContext)
        {
            this.renderer = new BsEditorNavRenderer(this);
            this.TabsProperties = new List<BsEditorTabAttribute>();
        }
        #endregion

        #region Public Methods
        public BsEditorNavBuilder AddTab(BsEditorTabAttribute attr)
        {
            this.TabsProperties.Add(attr);

            return this;
        }
        #endregion

        #region Helpers
        internal BsEditorNavBuilder Selected(object uid)
        {
            this.TabsProperties.ForEach(x =>
            {
                if (x.Id == uid)
                {
                    x.Selected = true;
                }
                else
                {
                    x.Selected = false;
                }
            });

            return this;
        }
        #endregion
    }
}
