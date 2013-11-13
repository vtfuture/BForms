using BForms.Models;
using BForms.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BForms.GroupEditor
{
    public class BsEditorToolbarButton : BaseComponent
    {
        private Glyphicon glyph { get; set; }
        private string name { get; set; }

        public BsEditorToolbarButton()
        {
        }

        public BsEditorToolbarButton Glyph(Glyphicon glyph)
        {
            this.glyph = glyph;

            return this;
        }

        public BsEditorToolbarButton DisplayName(string name)
        {
            this.name = name;

            return this;
        }

        public override string Render()
        {
            var btn = new TagBuilder("button");

            btn.MergeAttribute("type", "button");

            btn.AddCssClass("btn btn-default");

            btn.InnerHtml += GetGlyphcon(this.glyph);

            btn.InnerHtml += " " + this.name;

            return btn.ToString();
        }
    }

    public class BsEditorToolbarHtmlBuilder : BaseComponent
    {
        #region Properties and Constructor
        private List<BsEditorToolbarButton> buttons { get; set; }
        private bool inlineSearch { get; set; }

        public BsEditorToolbarHtmlBuilder(bool inlineSearch)
        {
            this.inlineSearch = inlineSearch;
            this.buttons = new List<BsEditorToolbarButton>();
        }
        #endregion

        #region Config
        public BsEditorToolbarButton AddButton(Glyphicon glyph)
        {
            var button = new BsEditorToolbarButton().Glyph(glyph);

            buttons.Add(button);

            return button;
        }
        #endregion

        #region Render
        public override string Render()
        {
            if (this.inlineSearch || this.buttons.Any())
            {
                var container = new TagBuilder("div");

                container.AddCssClass("search inline");

                var group = new TagBuilder("div");

                group.AddCssClass("input-group");

                #region Inline Search
                if (this.inlineSearch)
                {
                    container.AddCssClass("inline");

                    var glyph = GetGlyphcon(Glyphicon.Search, true);

                    var input = new TagBuilder("input");

                    input.MergeAttribute("type", "text");

                    input.MergeAttribute("placeholder", "Cauta");

                    input.AddCssClass("form-control");

                    group.InnerHtml += glyph;

                    group.InnerHtml += input;
                }
                #endregion

                #region Buttons
                if (this.buttons.Any())
                {
                    var wrapper = new TagBuilder("div");

                    wrapper.AddCssClass("input-group-btn");

                    foreach (var btn in buttons)
                    {
                        wrapper.InnerHtml += btn.Render();
                    }

                    group.InnerHtml += wrapper;
                }
                #endregion

                container.InnerHtml += group;

                return container.ToString();
            }

            return string.Empty;
        }
        #endregion
    }
}
