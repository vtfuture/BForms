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
        private string uid { get; set; } 

        public BsEditorToolbarButton(string uid)
        {
            this.uid = uid;
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

            btn.MergeAttribute("data-uid", this.uid);

            btn.AddCssClass("btn btn-default bs-toolbarBtn");

            btn.InnerHtml += GetGlyphcon(this.glyph);

            btn.InnerHtml += " " + this.name;

            return btn.ToString();
        }
        #endregion
    }
    #endregion

    #region BsEditorToolbarForm
    public class BsEditorToolbarForm<TModel> : BaseComponent
    {
        #region Properties and Constructor
        private TModel model { get; set; }
        private string uid { get; set; }
        private bool hide { get; set; }

        public BsEditorToolbarForm(TModel model, string uid, ViewContext viewContext)
        {
            this.model = model;
            this.uid = uid;
            this.viewContext = viewContext;
        }
        #endregion

        #region Public Methods
        public BsEditorToolbarForm<TModel> Template(string template)
        {
            this.template = template;

            return this;
        }

        public BsEditorToolbarForm<TModel> Hide()
        {
            this.hide = true;

            return this;
        }
        #endregion

        #region Render
        public override string Render()
        {
            var container = new TagBuilder("div");

            if (this.hide)
            {
                container.MergeAttribute("style", "display: none;");
            }

            container.AddCssClass("bs-editorForm");

            container.MergeAttribute("data-uid", this.uid);

            var partialView = viewContext.Controller.BsRenderPartialView(this.template, model);

            container.InnerHtml += partialView;

            return container.ToString();
        }
        #endregion
    }
    #endregion

    #region BsEditorToolbarPart
    public class BsEditorToolbarPart
    {
        #region Properties and Constructor
        internal BsEditorToolbarButton button { get; set; }
        internal string uid { get; set; }
        internal string template { get; set; }
        internal BaseComponent form { get; set; }

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
            this.button = new BsEditorToolbarButton(this.uid).DisplayName(name).Glyph(glyph);

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
        private List<BaseComponent> forms { get; set; }
        private bool inlineSearch { get; set; }

        public bool InlineSearch { get { return this.inlineSearch; } set { this.inlineSearch = value; } }

        public BsEditorToolbarHtmlBuilder(BsEditorTabBuilder tabBuilder, ViewContext viewContext)
        {
            this.parts = new List<BsEditorToolbarPart>();
            this.tabBuilder = tabBuilder.GetModel();
            this.viewContext = viewContext;
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

            var value = this.tabBuilder != null ? property.GetValue((TModel)this.tabBuilder) : null;

            var part = new BsEditorToolbarPart()
            {
                uid = name,
                template = template,
                form = new BsEditorToolbarForm<TValue>((TValue)value, name, this.viewContext).Hide()
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
            var buttons = this.parts.Where(x => x.button != null);

            var forms = this.parts.Where(x => x.form != null);

            var result = string.Empty;

            var container = new TagBuilder("div");

            container.AddCssClass("search");

            if (this.inlineSearch || buttons.Any())
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
                if (buttons.Any())
                {
                    var wrapper = new TagBuilder("div");

                    wrapper.AddCssClass("input-group-btn");

                    foreach (var btn in buttons.Select(x => x.button))
                    {
                        wrapper.InnerHtml += btn.Render();
                    }

                    group.InnerHtml += wrapper;
                }
                #endregion

                container.InnerHtml += group;
            }

            forms.ToList().ForEach(x => x.form.template = x.template);

            foreach (var form in forms.Select(x => x.form))
            {
                container.InnerHtml += form.Render();
            }

            result += container;

            return result;
        }
        #endregion
    }
    #endregion
}
