using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BootstrapForms.Grid
{
    public class BsToolbarActionsFactory
    {
        private List<BsToolbarAction> actions = new List<BsToolbarAction>();

        internal List<BsToolbarAction> Actions
        {
            get
            {
                return this.actions;
            }
        }

        public BsToolbarAction Add(BsToolbarActionType actionType)
        {
            var toolbarAction = new BsToolbarAction(actionType);
            actions.Add(toolbarAction);

            return toolbarAction;
        }

        public BsToolbarAction Add(string descriptorClass)
        {
            var toolbarAction = new BsToolbarAction(descriptorClass);
            actions.Add(toolbarAction);

            return toolbarAction;
        }
    }
}