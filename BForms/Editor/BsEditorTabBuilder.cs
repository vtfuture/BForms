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
    public abstract class BsEditorTabBuilder : BsBaseComponent
    {
        #region Properties
        internal BsPagerSettings pagerSettings;
        protected string name { get; set; }
        internal object uid { get; set; }
        internal bool selected { get; set; }
        internal bool hasModel { get; set; }
        #endregion

        #region Methods
        internal abstract IBsEditorTabModel GetModel();

        internal abstract bool IsSelected();

        internal abstract void SetSelected(bool selected);
        #endregion
    }
    #endregion

    #region BsEditorTabBuilder Basic
    public class BsEditorTabBuilder<TModel> : BsEditorTabBuilder where TModel : IBsEditorTabModel
    {
        #region Properties and Constructor
        internal BsEditorToolbarHtmlBuilder<TModel> toolbar { get; set; }
        internal bool quickSearch { get; set; }
        internal bool hasItems { get; set; }
        internal BsGridPagerBuilder pagerBuilder;
        internal TModel model;
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

        public void InitRenderer<TRow>()
        {
            this.renderer = new BsEditorTabRenderer<TModel, TRow>(this);
            this.hasModel = this.model != null && this.model.GetGrid<TRow>() != null;
            this.hasItems = this.hasModel ? !this.model.GetGrid<TRow>().IsEmpty() : false;
        }
        #endregion

        #region Public Methods
        public BsEditorTabBuilder<TModel> HtmlAttributes(Dictionary<string, object> htmlAttributes)
        {
            base.HtmlAttributes(htmlAttributes);

            return this;
        }

        public BsEditorToolbarPart For<TValue>(Expression<Func<TModel, TValue>> expression) where TValue : class
        {
            var key = this.model.GetPropertyName(expression);

            if (this.toolbar.parts.Any(x => x.uid == key))
            {
                return this.toolbar.parts.FirstOrDefault(x => x.uid == key);
            }

            throw new Exception("Couldn't find " + key + " toolbar part in the tab builder");
        }

        public void For<TRow>(Expression<Func<TModel, BsGridModel<TRow>>> expression)
        {
            // return some configurator for rows
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
        #endregion

        #region Helpers
        internal BsEditorTabBuilder<TModel> DisplayName(string name)
        {
            this.name = name;

            return this;
        }

        internal BsEditorTabBuilder<TModel> Id(object uid)
        {
            this.uid = uid;

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