using BForms.Models;
using BForms.Mvc;
using BForms.Renderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BForms.GroupEditor
{
    #region BsEditorToolbarPart
    public class BsEditorToolbarPart
    {
        #region Properties and Constructor
        internal BsEditorToolbarButtonBuilder button { get; set; }
        internal string uid { get; set; }
        internal string template { get; set; }
        internal BsBaseComponent form { get; set; }

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
            this.button = new BsEditorToolbarButtonBuilder(this.uid).DisplayName(name).Glyph(glyph);

            return this;
        }
        #endregion
    }
    #endregion

    #region BsEditorToolbarHtmlBuilder
    public class BsEditorToolbarHtmlBuilder : BsBaseComponent
    {
        #region Properties and Constructor
        private BsGroupEditor tabBuilder { get; set; }
        internal List<BsEditorToolbarPart> parts { get; set; }
        internal List<BsEditorToolbarButtonBuilder> buttons { get; set; }
        internal List<BsBaseComponent> forms { get; set; }
        internal bool inlineSearch { get; set; }

        public bool InlineSearch { get { return this.inlineSearch; } set { this.inlineSearch = value; } }

        public BsEditorToolbarHtmlBuilder(BsEditorTabBuilder tabBuilder, ViewContext viewContext)
        {
            this.renderer = new BsEditorToolbarBaseRenderer(this);
            this.parts = new List<BsEditorToolbarPart>();
            this.tabBuilder = tabBuilder.GetModel();
            this.viewContext = viewContext;
        }
        #endregion

        #region Public Methods
        public BsEditorToolbarHtmlBuilder HtmlAttributes(Dictionary<string, object> htmlAttributes)
        {
            base.HtmlAttributes(htmlAttributes);

            return this;
        }

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
                form = new BsEditorToolbarFormBuilder<TValue>((TValue)value, name, this.viewContext).Hide()
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
    }
    #endregion
}
