using BForms.GroupEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BForms.Renderers
{
    public class BsEditorToolbarButtonRenderer : BsBaseRenderer<BsEditorToolbarButtonBuilder>
    {
        public BsEditorToolbarButtonRenderer(){}

        public BsEditorToolbarButtonRenderer(BsEditorToolbarButtonBuilder builder)
            : base(builder)
        { 
        }

        public override string Render()
        {
            var btn = new TagBuilder("button");

            btn.MergeAttribute("type", "button");

            btn.MergeAttribute("data-uid", this.Builder.uid);

            btn.AddCssClass("btn btn-default bs-toolbarBtn");

            btn.InnerHtml += this.GetGlyphcon(this.Builder.glyph);

            btn.InnerHtml += " " + this.Builder.name;

            return btn.ToString();
        }
    }
}
