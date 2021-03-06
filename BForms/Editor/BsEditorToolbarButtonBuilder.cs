﻿using BForms.Models;
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
    #region BsEditorToolbarButton
    public class BsEditorToolbarButtonBuilder : BsBaseComponent<BsEditorToolbarButtonBuilder>
    {
        #region Constructor and Properties
        internal Glyphicon glyph { get; set; }
        internal string name { get; set; }
        internal string uid { get; set; }

        public BsEditorToolbarButtonBuilder()
        {
            this.renderer = new BsEditorToolbarButtonRenderer(this);
        }

        public BsEditorToolbarButtonBuilder(string uid)
        {
            this.uid = uid;
            this.renderer = new BsEditorToolbarButtonRenderer(this);
        }
        #endregion

        #region Public Methods
        public BsEditorToolbarButtonBuilder Glyph(Glyphicon glyph)
        {
            this.glyph = glyph;

            return this;
        }

        public BsEditorToolbarButtonBuilder DisplayName(string name)
        {
            this.name = name;

            return this;
        }
        #endregion
    }
    #endregion
}
