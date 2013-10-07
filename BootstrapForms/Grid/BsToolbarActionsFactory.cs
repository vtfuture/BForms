using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BootstrapForms.Grid
{
    public class BsToolbarActionsFactory<TToolbar>
    {
        private List<BsToolbarAction<TToolbar>> actions = new List<BsToolbarAction<TToolbar>>();
        private readonly ViewContext viewContext;

        internal List<BsToolbarAction<TToolbar>> Actions
        {
            get
            {
                return this.actions;
            }
        }

        public BsToolbarActionsFactory(ViewContext viewContext)
        {
            this.viewContext = viewContext;
        }

        public BsToolbarAction<TToolbar> Add(BsToolbarActionType actionType)
        {
            var toolbarAction = new BsToolbarAction<TToolbar>(actionType, this.viewContext);
            actions.Add(toolbarAction);

            return toolbarAction;
        }

        public BsToolbarAction<TToolbar> Add(string descriptorClass)
        {
            var toolbarAction = new BsToolbarAction<TToolbar>(descriptorClass, this.viewContext);
            actions.Add(toolbarAction);

            return toolbarAction;
        }
    }
}