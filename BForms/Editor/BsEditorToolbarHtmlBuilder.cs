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

namespace BForms.Editor
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
    public class BsEditorToolbarHtmlBuilder<TModel> : BsBaseComponent where TModel : IBsEditorTabModel
    {
        #region Properties and Constructor
        private TModel model { get; set; }
        internal List<BsEditorToolbarPart> parts { get; set; }
        internal List<BsEditorToolbarButtonBuilder> buttons { get; set; }
        internal List<BsBaseComponent> forms { get; set; }
        internal bool quickSearch { get; set; }

        public bool QuickSearch { get { return this.quickSearch; } set { this.quickSearch = value; } }

        public BsEditorToolbarHtmlBuilder(BsEditorTabBuilder tabBuilder, ViewContext viewContext)
        {
            this.renderer = new BsEditorToolbarRenderer<TModel>(this);
            this.parts = new List<BsEditorToolbarPart>();
            this.model = (TModel)tabBuilder.GetModel();
            this.viewContext = viewContext;
        }
        #endregion

        #region Public Methods
        public BsEditorToolbarHtmlBuilder<TModel> HtmlAttributes(Dictionary<string, object> htmlAttributes)
        {
            base.HtmlAttributes(htmlAttributes);

            return this;
        }

        public BsEditorToolbarPart Add<TValue>(Expression<Func<TModel, TValue>> expression) where TValue : class
        {
            return Add<TValue>(expression, "");
        }

        public BsEditorToolbarPart Add<TValue>(Expression<Func<TModel, TValue>> expression, string template) where TValue : class
        {
            var part = new BsEditorToolbarPart();

            part.Template(template);

            this.FillDetails<TValue>((TModel)this.model, expression, part);

            Add(part);

            return part;
        }

        public BsEditorToolbarPart For<TValue>(Expression<Func<TModel, TValue>> expression)
        {
            var key = this.model.GetPropertyName(expression);

            if (this.parts.Any(x => x.uid == key))
            {
                return this.parts.FirstOrDefault(x => x.uid == key);
            }

            throw new Exception("Couldn't find " + key + " toolbar part in the tab builder");
        }
        #endregion

        #region Helpers
        internal BsEditorToolbarPart FillDetails<TValue>(TModel model, Expression<Func<TModel, TValue>> expression, BsEditorToolbarPart part) where TValue : class
        {
            var name = model.GetPropertyName(expression);

            var type = typeof(TModel);

            var property = type.GetProperty(name);

            var value = model != null ? (TValue)property.GetValue(model) : null;

            part.uid = name;

            part.form = new BsEditorFormBuilder<TValue>((TValue)value, name, this.viewContext).Hide();

            return part;
        }

        internal void Add(BsEditorToolbarPart part)
        {
            this.parts.Add(part);
        }
        #endregion
    }
    #endregion
}
