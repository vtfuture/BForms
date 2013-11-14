using BForms.Models;
using BForms.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BForms.GroupEditor
{
    #region BsEditorToolbarButton
    public class BsEditorToolbarButton : BaseComponent
    {
        #region Constructor and Properties
        private Glyphicon glyph { get; set; }
        private string name { get; set; }

        public BsEditorToolbarButton()
        {
        }
        #endregion

        #region Public Methods
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
        #endregion

        #region Render
        public override string Render()
        {
            var btn = new TagBuilder("button");

            btn.MergeAttribute("type", "button");

            btn.AddCssClass("btn btn-default");

            btn.InnerHtml += GetGlyphcon(this.glyph);

            btn.InnerHtml += " " + this.name;

            return btn.ToString();
        }
        #endregion
    }
    #endregion

    #region BsEditorToolbarPart
    public class BsEditorToolbarPart
    {
        #region Properties and Constructor
        internal BsEditorToolbarButton button { get; set; }
        internal string name { get; set; }
        internal string template { get; set; }
        internal object value { get; set; }

        public BsEditorToolbarPart()
        {
        }
        #endregion

        #region Public Methods
        public BsEditorToolbarPart Template(string template)
        {
            this.template = template;

            return this;
        }

        public BsEditorToolbarPart Button(string name, Glyphicon glyph)
        {
            this.button = new BsEditorToolbarButton().DisplayName(name).Glyph(glyph);

            return this;
        }
        #endregion
    }
    #endregion

    #region BsEditorToolbarHtmlBuilder
    public class BsEditorToolbarHtmlBuilder : BaseComponent
    {
        #region Properties and Constructor
        private BsGroupEditor tabBuilder { get; set; }
        private List<BsEditorToolbarPart> parts { get; set; }
        private List<BsEditorToolbarButton> buttons { get; set; }
        private bool inlineSearch { get; set; }

        public bool InlineSearch { get { return this.inlineSearch; } set { this.inlineSearch = value; } }

        public BsEditorToolbarHtmlBuilder(BsEditorTabBuilder tabBuilder)
        {
            this.parts = new List<BsEditorToolbarPart>();
            this.tabBuilder = tabBuilder.GetModel();
        }
        #endregion

        #region Public Methods
        public BsEditorToolbarPart Add<TModel, TValue>(Expression<Func<TModel, TValue>> expression) where TModel : BsGroupEditor
        {
            return Add<TModel, TValue>(expression, "");
        }

        public BsEditorToolbarPart Add<TModel, TValue>(Expression<Func<TModel, TValue>> expression, string template) where TModel : BsGroupEditor
        {
            var name = ((TModel)this.tabBuilder).GetPropertyName(expression);
            var type = typeof(TModel);

            var property = type.GetProperty(name);

            //TValue value = (TValue)property.GetValue((TModel)this.tabBuilder);
            var value = this.tabBuilder != null ? property.GetValue((TModel)this.tabBuilder) : null;

            var part = new BsEditorToolbarPart()
            {
                name = name,
                template = template,
                value = value
            };

            this.parts.Add(part);

            return part;
        }
        #endregion

        #region Helpers
        internal void Add(BsEditorToolbarPart part)
        {
            this.parts.Add(part);
        }
        #endregion

        #region Render
        public override string Render()
        {
            this.buttons = this.parts.Where(x => x.button != null).Select(x => x.button).ToList();

            var result = string.Empty;

            var container = new TagBuilder("div");

            container.AddCssClass("search");

            if (this.inlineSearch || this.buttons.Any())
            {
                container.AddCssClass("inline");

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
            }

            result += container;

            return result;
        }
        #endregion
    }
    #endregion
}
