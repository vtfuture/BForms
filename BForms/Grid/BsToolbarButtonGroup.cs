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
    public class BsToolbarButtonGroup<TToolbar> : BsToolbarActionsBaseFactory<TToolbar>
    {
        protected string name { get; set; }

        internal string Name
        {
            get
            {
                return this.name;
            }
        }

        protected BsToolbarGroupButtonDirection direction { get; set; }

        internal BsToolbarGroupButtonDirection Direction
        {
            get
            {
                return this.direction;
            }
        }

        public BsToolbarButtonGroup(ViewContext viewContext) : base(viewContext)
        {
            this.renderer = new BsToolbarButtonGroupRenderer<TToolbar>(this);
        } 

        public BsToolbarButtonGroup<TToolbar> DisplayName(string name)
        {
            this.name = name;

            return this;
        }

        public BsToolbarButtonGroup<TToolbar> SetDirection(BsToolbarGroupButtonDirection direction)
        {
            this.direction = direction;

            return this;
        }

        public BsToolbarItemGroupActionLink<TToolbar> AddActionLink()
        {
            var toolbarAction = new BsToolbarItemGroupActionLink<TToolbar>(this.viewContext);
            actions.Add(toolbarAction);

            return toolbarAction;
        }

        public BsToolbarItemGroupSeparator AddSeperator()
        {
            var toolbarAction = new BsToolbarItemGroupSeparator();
            actions.Add(toolbarAction);

            return toolbarAction;
        }

    }


    public enum BsToolbarGroupButtonDirection
    {
        Down =1,
        Up =2
    }
}
