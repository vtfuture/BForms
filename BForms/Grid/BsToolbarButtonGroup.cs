using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
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
    }


    public enum BsToolbarGroupButtonDirection
    {
        Down =1,
        Up =2
    }

    public enum BsToolbarGroupButtonItemType
    {
        ActionLink=1,
        Separator = 2
    }
}
