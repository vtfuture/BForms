using BForms.GroupEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BForms.Renderers
{
    public class BsEditorToolbarFormRenderer<TModel> : BsBaseRenderer<BsEditorToolbarFormBuilder<TModel>>
    {
        public BsEditorToolbarFormRenderer(){}

        public BsEditorToolbarFormRenderer(BsEditorToolbarFormBuilder<TModel> builder)
            : base(builder)
        { 
        }

        public override string Render()
        {
            var container = new TagBuilder("div");

            if (this.Builder.hide)
            {
                container.MergeAttribute("style", "display: none;");
            }

            container.AddCssClass("bs-editorForm");

            container.MergeAttribute("data-uid", this.Builder.uid);

            container.InnerHtml += this.Builder.CompileModel();

            return container.ToString();
        }
    }
}
