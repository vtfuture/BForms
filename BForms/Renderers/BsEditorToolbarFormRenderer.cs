using BForms.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BForms.Utilities;

namespace BForms.Renderers
{
    public class BsEditorFormRenderer<TModel> : BsBaseRenderer<BsEditorFormBuilder<TModel>>
    {
        public BsEditorFormRenderer(){}

        public BsEditorFormRenderer(BsEditorFormBuilder<TModel> builder)
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

            container.MergeAttribute("data-uid", this.Builder.uid.ToString());

            container.InnerHtml += this.Builder.RenderModel<TModel>(this.Builder.Model, this.Builder.uid.ToString());

            return container.ToString();
        }
    }
}
