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
using BForms.Utilities;

namespace BForms.Editor
{
    #region BsEditorTabBuilder Base
    public abstract class BsEditorTabBuilder : BsBaseComponent<BsEditorTabBuilder>
    {
        #region Properties
        internal BsPagerSettings pagerSettings;
        internal string noResultsTemplate;
        protected string name { get; set; }
        protected object uid { get; set; }
        internal bool selected { get; set; }
        protected bool hasModel { get; set; }
        protected bool hasItems { get; set; }
        internal bool editable { get; set; }
        internal bool bulkMove { get; set; }
        internal object[] connectsWith { get; set; }
        internal string bulkMoveHtml { get; set; }
        protected bool isReadonly { get; set; }

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

        public bool BulkMove
        {
            set { this.bulkMove = value; }
        }

        internal object[] ConnectsWithIds
        {
            get
            {
                return this.connectsWith;
            }
        }

        internal bool IsReadonly
        {
            get
            {
                return this.isReadonly;
            }
            set
            {
                this.isReadonly = value;
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
        internal IBsEditorRowConfigurator rowConfigurator;
        private List<TabGroupConnection> connections { get; set; }

        internal List<TabGroupConnection> Connections
        {
            get
            {
                return this.connections;
            }
        }

        internal bool IsEditable
        {
            get
            {
                return this.editable;
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

        public BsEditorTabBuilder(IBsEditorTabModel model, ViewContext viewContext, List<TabGroupConnection> connections)
        {
            this.connections = connections;
            this.viewContext = viewContext;
            this.model = (TModel)model;
            this.toolbar = new BsEditorToolbarHtmlBuilder<TModel>(this, viewContext);
            this.pagerSettings = new BsPagerSettings();
        }

        internal void InitRenderer<TRow>(BsTheme theme) where TRow : BsItemModel
        {
            this.Theme = theme;
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
                // Remove Add Form if Readonly
                if (key == "New" && this.isReadonly)
                {
                    return this;
                }

                var part = this.toolbar.Add<TValue>(expression, template);

                if (key == "Search")
                {
                    part.Button(BsResourceManager.Resource("BF_Search"), Glyphicon.Search);
                }
                else if (key == "New")
                {
                    part.Button(BsResourceManager.Resource("BF_New"), Glyphicon.Plus);
                }
            }

            return this;
        }

        public BsEditorTabBuilder<TModel> Editable()
        {
            this.editable = true;

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
            this.uid = MvcHelpers.Serialize(uid);

            this.toolbar.Id(this.uid);

            return this;
        }
        public BsEditorTabBuilder<TModel> ConnectsWith(params object[] ids)
        {
            this.connectsWith = ids;

            return this;
        }

        public BsEditorTabBuilder<TModel> NoResultsTemplate(string template)
        {
            this.noResultsTemplate = template;

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