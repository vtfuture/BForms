using BForms.Grid;
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
    #region BsEditorTabBuilder Base
    public abstract class BsEditorTabBuilder : BsBaseComponent
    {
        #region Properties
        internal string rowTemplate { get; set; }
        protected BsPagerSettings pagerSettings;
        protected string name { get; set; }
        internal object uid { get; set; }
        internal bool selected { get; set; }
        internal bool HasModel { get; set; }
        #endregion

        #region Methods
        internal abstract BsGroupEditor GetModel();

        internal abstract bool IsSelected();

        internal abstract void SetSelected(bool selected);
        #endregion
    }
    #endregion

    #region BsEditorTabBuilder Search
    public class BsEditorTabBuilder<TRow, TSearch> : BsEditorTabBuilder<TRow> 
        where TRow : new() 
        where TSearch : class
    {
        #region Constructor and Properties
        private BsGroupEditor<TRow, TSearch> model;
        protected BsEditorToolbarPart searchPart = new BsEditorToolbarPart();
        protected TSearch searchModel;

        public BsEditorTabBuilder(BsGroupEditor<TRow, TSearch> model, ViewContext viewContext)
            : base(model, viewContext)
        {
            this.model = model;
        }
        #endregion

        #region Public Methods
        

        public BsEditorTabBuilder<TRow, TSearch> Template(Expression<Func<BsGroupEditor<TRow, TSearch>, TSearch>> expression, string template)
        {
            this.searchPart.template = template;

            FillDetails(this.model, expression, this.searchPart).Button("Cauta", Glyphicon.Search);

            return this;
        }

        public BsEditorTabBuilder<TRow, TSearch> Template(Expression<Func<BsGroupEditor<TRow, TSearch>, BsGridModel<TRow>>> expression, string template)
        {
            this.rowTemplate = template;

            return this;
        }

        public BsEditorTabBuilder<TRow, TSearch> PagerSettings(BsPagerSettings pagerSettings)
        {
            this.pagerSettings = pagerSettings;

            return this;
        }
        #endregion

        #region Helpers
        public BsEditorToolbarPart FillDetails<TModel, TValue>(TModel model, Expression<Func<TModel, TValue>> expression, BsEditorToolbarPart part) where TValue : class
        {
            var name = model.GetPropertyName(expression);

            var type = typeof(TModel);

            var property = type.GetProperty(name);

            var value = this.model != null ? (TValue)property.GetValue(model) : null;

            part.uid = name;

            part.form = new BsEditorToolbarFormBuilder<TValue>((TValue)value, name, this.viewContext).Hide();

            return part;
        }
        #endregion

        #region Render
        internal override void BeforeRender()
        {
            this.toolbar.Add(searchPart);
        }
        #endregion
    }
    #endregion

    #region BsEditorTabBuilder Search and New
    public class BsEditorTabBuilder<TRow, TSearch, TNew> : BsEditorTabBuilder<TRow, TSearch>
        where TRow : new() 
        where TSearch : class
        where TNew : class
    {
        #region Constructor and Properties
        private BsGroupEditor<TRow, TSearch, TNew> model;
        private BsEditorToolbarPart newPart = new BsEditorToolbarPart();
        private TNew newModel;

        public BsEditorTabBuilder(BsGroupEditor<TRow, TSearch, TNew> model, ViewContext viewContext)
            : base(model, viewContext)
        {
            this.model = model;
        }
        #endregion

        #region Public Methods
        public BsEditorTabBuilder<TRow, TSearch, TNew> Template(Expression<Func<BsGroupEditor<TRow, TSearch, TNew>, TSearch>> expression, string template)
        {
            this.searchPart.template = template;

            FillDetails(this.model, expression, this.searchPart).Button("Cauta", Glyphicon.Search);

            return this;
        }

        public BsEditorTabBuilder<TRow, TSearch, TNew> Template(Expression<Func<BsGroupEditor<TRow, TSearch, TNew>, TNew>> expression, string template)
        {
            this.newPart.Template(template);

            FillDetails(this.model, expression, this.newPart).Button("Adauga", Glyphicon.Plus);

            return this;
        }

        public BsEditorTabBuilder<TRow, TSearch, TNew> Template(Expression<Func<BsGroupEditor<TRow, TSearch, TNew>, BsGridModel<TRow>>> expression, string template)
        {
            this.rowTemplate = template;

            return this;
        }

        public BsEditorTabBuilder<TRow, TSearch, TNew> PagerSettings(BsPagerSettings pagerSettings)
        {
            this.pagerSettings = pagerSettings;

            return this;
        }
        #endregion

        #region Render
        internal override void BeforeRender()
        {
            this.toolbar.Add(searchPart);
            this.toolbar.Add(newPart);
        }
        #endregion
    }
    #endregion

    #region BsEditorTabBuilder Basic
    public class BsEditorTabBuilder<TRow> : BsEditorTabBuilder where TRow : new()
    {
        #region Properties and Constructor
        internal BsEditorToolbarHtmlBuilder toolbar { get; set; }
        internal bool inlineSearch { get; set; }
        internal bool hasItems { get; set; }
        internal BsGridPagerBuilder pagerBuilder;
        internal BsGroupEditor<TRow> model;

        public BsEditorToolbarHtmlBuilder Toolbar { get { return this.toolbar; } }
        public bool InlineSearch
        {
            set
            {
                this.toolbar.InlineSearch = value;
                this.inlineSearch = value;
            }
        }

        public BsEditorTabBuilder(BsGroupEditor<TRow> model, ViewContext viewContext)
        {
            this.renderer = new BsEditorTabBaseRenderer<TRow>(this);
            this.viewContext = viewContext;
            this.model = model;
            this.HasModel = this.model != null && this.model.Grid != null;
            this.hasItems = this.HasModel ? this.model.Grid.Items != null && this.model.Grid.Items.Any() : false;
            this.toolbar = new BsEditorToolbarHtmlBuilder(this, viewContext);
            this.pagerSettings = new BsPagerSettings();
        }
        #endregion

        #region Public Methods
        public BsEditorTabBuilder<TRow> Template(Expression<Func<BsGroupEditor<TRow>, BsGridModel<TRow>>> expression, string template)
        {
            this.rowTemplate = template;

            return this;
        }

        public BsEditorTabBuilder<TRow> Selected(bool selected)
        {
            this.selected = selected;

            return this;
        }

        public BsEditorTabBuilder<TRow> PagerSettings(BsPagerSettings pagerSettings)
        {
            this.pagerSettings = pagerSettings;

            return this;
        }
        #endregion

        #region Helpers
        internal BsEditorTabBuilder<TRow> DisplayName(string name)
        {
            this.name = name;

            return this;
        }

        internal BsEditorTabBuilder<TRow> Id(object uid)
        {
            this.uid = uid;

            return this;
        }
        #endregion

        #region Override
        internal override BsGroupEditor GetModel()
        {
            return this.model;
        }

        internal override bool IsSelected()
        {
            return this.selected;
        }

        internal override void SetSelected(bool selected)
        {
            this.selected = selected;
        }
        #endregion

        #region Render
        internal virtual void InitPager()
        {
            if (this.model != null)
            {
                this.pagerBuilder = new BsGridPagerBuilder(this.model.Grid.Pager, this.pagerSettings, this.model.Grid.BaseSettings);
            }
        }

        internal virtual void BeforeRender()
        {
            
        }
        #endregion
    }
    #endregion
}