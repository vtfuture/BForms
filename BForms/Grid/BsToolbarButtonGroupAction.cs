using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BForms.Grid
{
    class BsToolbarButtonGroupAction<TToolbar> : BsToolbarAction<TToolbar>
    {
        public BsToolbarButtonGroup<TToolbar> ButtonGroup { get; set; }
    }
}
