using BForms.Editor;
using BForms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BForms.Utilities;

namespace BForms.Renderers
{
    public class BsEditorToolbarRenderer<TModel> : BsBaseRenderer<BsEditorToolbarHtmlBuilder<TModel>> where TModel : IBsEditorTabModel
    {
        public BsEditorToolbarRenderer()
        {

        }

        public BsEditorToolbarRenderer(BsEditorToolbarHtmlBuilder<TModel> builder)
            : base(builder)
        {

        }

        public override string Render()
        {
            var buttons = this.Builder.parts.Where(x => x.button != null);

            var forms = this.Builder.parts.Where(x => x.form != null);

            var result = string.Empty;

            var container = new TagBuilder("div");

            container.AddCssClass("search");

            if (this.Builder.quickSearch || buttons.Any())
            {
                container.AddCssClass("inline");

                var group = new TagBuilder("div");

                group.AddCssClass("input-group");

                #region Inline Search
                if (this.Builder.quickSearch)
                {
                    var glyph = GetGlyphicon(Glyphicon.Search, true);

                    var input = new TagBuilder("input");

                    input.MergeAttribute("type", "text");

                    input.MergeAttribute("placeholder", BsResourceManager.Resource("Search"));

                    input.AddCssClass("form-control");

                    input.AddCssClass("bs-tabInlineSearch");

                    group.InnerHtml += glyph;

                    group.InnerHtml += input;
                }
                #endregion

                #region Buttons
                if (buttons.Any())
                {
                    var wrapper = new TagBuilder("div");

                    wrapper.AddCssClass("input-group-btn");

                    foreach (var btn in buttons.Select(x => x.button))
                    {
                        wrapper.InnerHtml += btn.ToString();
                    }

                    group.InnerHtml += wrapper;
                }
                #endregion

                container.InnerHtml += group;
            }

            forms.ToList().ForEach(x => 
            {
                if (string.IsNullOrEmpty(x.form.template)) 
                {
                    x.form.template = x.template;
                }
            });

            foreach (var form in forms.Select(x => x.form))
            {
                container.InnerHtml += form.ToString();
            }

            result += container;

            return result;
        }
    }
}
