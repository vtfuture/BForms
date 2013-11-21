using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BForms.Mvc;

namespace BForms.Grid
{
    public class BsToolbarActionsBaseFactory<TToolbar> : BsBaseComponent<BsToolbarActionsBaseFactory<TToolbar>>
    {
        protected List<BsBaseComponent> actions = new List<BsBaseComponent>();
        internal List<BsBaseComponent> Actions
        {
            get
            {
                return this.actions;
            }
        }

        public BsToolbarActionsBaseFactory(ViewContext viewContext)
            : base(viewContext)
        {
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
    }

    public class BsToolbarActionsFactory<TToolbar> : BsToolbarActionsBaseFactory<TToolbar>
    {
        protected List<BsToolbarButtonGroup<TToolbar>> buttonGroup;

        internal List<BsToolbarButtonGroup<TToolbar>> ButtonGroups
        {
            get
            {
                return this.buttonGroup;
            }
        }

        public BsToolbarActionsFactory(ViewContext viewContext) : base(viewContext)
        {
        }

        /// <summary>
        /// Adds custom control - ex: QuickSearch
        /// </summary>
        /// <typeparam name="TCustomAction"></typeparam>
        /// <returns></returns>
        public TCustomAction Add<TCustomAction>() where TCustomAction:new()
        {
            var toolbarAction = new TCustomAction();
            this.actions.Add(toolbarAction as BsBaseComponent);
            return toolbarAction;
        }

        public BsToolbarButtonGroup<TToolbar> AddButtonGroup()
        {
            if (this.buttonGroup == null)
            {
                this.buttonGroup = new List<BsToolbarButtonGroup<TToolbar>>();
            }

            var group = new BsToolbarButtonGroup<TToolbar>(this.viewContext);

            this.buttonGroup.Add(group);

            return group;
        }
    }
}