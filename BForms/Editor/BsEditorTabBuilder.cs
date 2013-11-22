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

namespace BForms.Editor
{
    #region BsEditorTabBuilder Base
    public abstract class BsEditorTabBuilder : BsBaseComponent<BsEditorTabBuilder>
    {
        #region Properties
        internal BsPagerSettings pagerSettings;
        protected string name { get; set; }
        protected object uid { get; set; }
        internal bool selected { get; set; }
        protected bool hasModel { get; set; }
        protected bool hasItems { get; set; }

        internal object Uid
        {
            get
            {
                return this.uid;
            }
        }

        internal bool HasModel
        {
            get
            {
                return this.hasModel;
            }
        }

        internal bool HasItems
        {
            get
            {
                return this.hasItems;
            }
        }
        #endregion

        #region Methods
        internal abstract IBsEditorTabModel GetModel();

        internal abstract bool IsSelected();

        internal abstract void SetSelected(bool selected);
        #endregion
    }
    #endregion

    #region BsEditorTabBuilder Basic
    public class BsEditorTabBuilder<TModel> : BsEditorTabBuilder 
        where TModel : IBsEditorTabModel
    {
        #region Properties and Constructor
        internal BsEditorToolbarHtmlBuilder<TModel> toolbar { get; set; }
        internal bool quickSearch { get; set; }
        private BsGridPagerBuilder pagerBuilder;
        private TModel model;
        private object[] connectsWith { get; set; }
        internal IBsEditorRowConfigurator rowConfigurator;

        internal object[] ConnectsWithIds
        {
            get
            {
                return this.connectsWith;
            }
        }

        internal TModel Model
        {
            get
            {
                return this.model;
            }
        }

        internal BsGridPagerBuilder PagerBuilder
        {
            get
            {
                return this.pagerBuilder;
            }
            set
            {
                this.pagerBuilder = value;
            }
        }

        public BsEditorToolbarHtmlBuilder<TModel> Toolbar { get { return this.toolbar; } }

        public bool QuickSearch
        {
            set
            {
                this.toolbar.QuickSearch = value;
                this.quickSearch = value;
            }
        }

        public BsEditorTabBuilder(IBsEditorTabModel model, ViewContext viewContext)
        {
            this.viewContext = viewContext;
            this.model = (TModel)model;
            
            this.toolbar = new BsEditorToolbarHtmlBuilder<TModel>(this, viewContext);
            this.pagerSettings = new BsPagerSettings();
        }

        internal void InitRenderer<TRow>() where TRow : BsItemModel
        {
            this.rowConfigurator = new BsEditorRowConfigurator<TRow>(this.viewContext);
            this.renderer = new BsEditorTabRenderer<TModel, TRow>(this);
            this.hasModel = this.model != null && this.model.GetGrid<TRow>() != null;
            this.hasItems = this.hasModel ? !this.model.GetGrid<TRow>().IsEmpty() : false;
        }
        #endregion

        #region Public Methods
        public BsEditorToolbarPart For<TValue>(Expression<Func<TModel, TValue>> expression) where TValue : class
        {
            var key = this.uid + "." + this.model.GetPropertyName(expression);

            if (this.toolbar.parts.Any(x => x.uid == key))
            {
                return this.toolbar.parts.FirstOrDefault(x => x.uid == key);
            }

            throw new Exception("Couldn't find " + key + " toolbar part in the tab builder");
        }

        public BsEditorRowConfigurator<TRow> For<TRow>(Expression<Func<TModel, BsGridModel<TRow>>> expression)
        {
            return (BsEditorRowConfigurator<TRow>)this.rowConfigurator;
        }

        public BsEditorTabBuilder<TModel> Template<TValue>(Expression<Func<TModel, TValue>> expression, string template) where TValue : class
        {
            var key = this.model.GetPropertyName(expression);

            if (key == "Grid")
            {
                this.template = template;
            }
            else
            {
                var part = this.toolbar.Add<TValue>(expression, template);

                if (key == "Search")
                {
                    part.Button("Cauta", Glyphicon.Search);
                }
                else if (key == "New")
                {
                    part.Button("New", Glyphicon.Plus);
                }
            }
          
            return this;
        }

        public BsEditorTabBuilder<TModel> Selected(bool selected)
        {
            this.selected = selected;

            return this;
        }

        public BsEditorTabBuilder<TModel> PagerSettings(BsPagerSettings pagerSettings)
        {
            this.pagerSettings = pagerSettings;

            return this;
        }

        public BsEditorTabBuilder<TModel> DisplayName(string name)
        {
            this.name = name;

            return this;
        }

        public BsEditorTabBuilder<TModel> Id(object uid)
        {
            this.uid = uid;

            this.toolbar.Id(this.uid.ToString());

            return this;
        }

        public BsEditorTabBuilder<TModel> ConnectsWith(params object[] ids)
        {
            this.connectsWith = ids;

            return this;
        }
        #endregion

        #region Override
        internal override IBsEditorTabModel GetModel()
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
    }
    #endregion
}