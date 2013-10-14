using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BForms.Mvc;

namespace BForms.Grid
{
    public class BsToolbarActionsFactory<TToolbar>
    {
        private List<BaseComponent> actions = new List<BaseComponent>();
        private readonly ViewContext viewContext;

        internal List<BaseComponent> Actions
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

        public TCustomAction Add<TCustomAction>() where TCustomAction:new()
        {
            var toolbarAction = new TCustomAction();
            actions.Add(toolbarAction as BaseComponent);
            return toolbarAction;
        }
    }
}