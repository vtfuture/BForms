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
        private List<BsBaseComponent> actions = new List<BsBaseComponent>();
        private readonly ViewContext viewContext;

        internal List<BsBaseComponent> Actions
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

        /// <summary>
        /// Adds a default action
        /// </summary>
        /// <param name="actionType">Action type</param>
        /// <returns>BsToolbarAction</returns>
        public BsToolbarAction<TToolbar> Add(BsToolbarActionType actionType)
        {
            var toolbarAction = new BsToolbarAction<TToolbar>(actionType, this.viewContext);
            actions.Add(toolbarAction);

            return toolbarAction;
        }

        /// <summary>
        /// Adds custom control - action or tab
        /// </summary>
        /// <param name="descriptorClass"></param>
        /// <returns>BsToolbarAction</returns>
        public BsToolbarAction<TToolbar> Add(string descriptorClass)
        {
            var toolbarAction = new BsToolbarAction<TToolbar>(descriptorClass, this.viewContext);
            actions.Add(toolbarAction);

            return toolbarAction;
        }

        /// <summary>
        /// Adds custom control - ex: QuickSearch
        /// </summary>
        /// <typeparam name="TCustomAction"></typeparam>
        /// <returns></returns>
        public TCustomAction Add<TCustomAction>() where TCustomAction:new()
        {
            var toolbarAction = new TCustomAction();
            actions.Add(toolbarAction as BsBaseComponent);
            return toolbarAction;
        }
    }
}