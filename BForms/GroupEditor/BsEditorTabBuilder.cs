using BForms.Grid;
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
    #region BsEditorTabBuilder Base
    public abstract class BsEditorTabBuilder : BaseComponent
    {
        #region Properties
        protected string rowTemplate { get; set; }
        protected BsPagerSettings pagerSettings;
        protected string name { get; set; }
        protected object uid { get; set; }
        protected bool selected { get; set; }
        internal bool HasModel { get; set; }
        #endregion

        #region Methods
        internal abstract BsGroupEditor GetModel();

        internal abstract string RenderAjax();

        internal abstract bool IsSelected();

        internal abstract void SetSelected(bool selected);
        #endregion
    }
    #endregion

    #region BsEditorTabBuilder Search
    public class BsEditorTabBuilder<TRow, TSearch> : BsEditorTabBuilder<TRow> where TRow : new()
    {
        #region Constructor and Properties
        private BsGroupEditor<TRow, TSearch> model;
        protected BsEditorToolbarPart searchPart;

        public BsEditorTabBuilder(BsGroupEditor<TRow, TSearch> model, ViewContext viewContext)
            : base(model, viewContext)
        {
            this.model = model;
            this.searchPart = new BsEditorToolbarPart().Button("Cauta", Glyphicon.Search);
        }
        #endregion

        #region Public Methods
        public BsEditorTabBuilder<TRow, TSearch> Template(Expression<Func<BsGroupEditor<TRow, TSearch>, TSearch>> expression, string template)
        {
            this.searchPart.Template(template);

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

        #region Render
        internal override void BeforeRender()
        {


            this.toolbar.Add(searchPart);
        }
        #endregion
    }
    #endregion

    #region BsEditorTabBuilder Search and New
    public class BsEditorTabBuilder<TRow, TSearch, TNew> : BsEditorTabBuilder<TRow, TSearch> where TRow : new()
    {
        #region Constructor and Properties
        private BsGroupEditor<TRow, TSearch, TNew> model;
        private BsEditorToolbarPart newPart;

        public BsEditorTabBuilder(BsGroupEditor<TRow, TSearch, TNew> model, ViewContext viewContext)
            : base(model, viewContext)
        {
            this.model = model;
            this.newPart = new BsEditorToolbarPart().Button("Adauga", Glyphicon.Plus);
        }
        #endregion

        #region Public Methods
        public BsEditorTabBuilder<TRow, TSearch, TNew> Template(Expression<Func<BsGroupEditor<TRow, TSearch, TNew>, TSearch>> expression, string template)
        {
            this.searchPart.Template(template);

            return this;
        }

        public BsEditorTabBuilder<TRow, TSearch, TNew> Template(Expression<Func<BsGroupEditor<TRow, TSearch, TNew>, TNew>> expression, string template)
        {
            this.newPart.Template(template);

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
        protected BsEditorToolbarHtmlBuilder toolbar { get; set; }
        protected bool inlineSearch { get; set; }
        protected bool hasItems { get; set; }
        private BsGridPagerBuilder pagerBuilder;
        private BsGroupEditor<TRow> model;

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
            this.viewContext = viewContext;
            this.model = model;
            this.HasModel = this.model != null && this.model.Grid != null;
            this.hasItems = this.HasModel ? this.model.Grid.Items != null && this.model.Grid.Items.Any() : false;
            this.toolbar = new BsEditorToolbarHtmlBuilder(this);
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
        internal virtual void BeforeRender()
        {

        }

        internal string RenderPager()
        {
            this.pagerBuilder = new BsGridPagerBuilder(this.model.Grid.Pager, this.pagerSettings, this.model.Grid.BaseSettings);

            return this.pagerBuilder.Render();
        }

        internal string RenderItems()
        {
            var result = "";

            if (!string.IsNullOrEmpty(this.rowTemplate))
            {
                var list = new TagBuilder("ul");

                list.AddCssClass("group_profiles");

                foreach (var item in this.model.Grid.Items)
                {
                    var listItem = new TagBuilder("li");

                    var listItemWrapper = new TagBuilder("div");

                    listItemWrapper.AddCssClass("media profile large");

                    var anchorLeft = new TagBuilder("a");

                    anchorLeft.MergeAttribute("href", "#");

                    anchorLeft.AddCssClass("pull-left");

                    var img = new TagBuilder("img");

                    img.AddCssClass("media-object");

                    img.MergeAttribute("src", "http://www.cloverwos.ro/1.0.0.217/Content/images/bg-user.png");

                    anchorLeft.InnerHtml += img;

                    listItemWrapper.InnerHtml += anchorLeft;

                    var anchorRight = new TagBuilder("a");

                    anchorRight.MergeAttribute("href", "#");

                    anchorRight.AddCssClass("btn btn-default select_profile");

                    anchorRight.InnerHtml += GetGlyphcon(Glyphicon.Ok);

                    listItemWrapper.InnerHtml += anchorRight;

                    var templateWrapper = new TagBuilder("div");

                    templateWrapper.AddCssClass("media-body");

                    templateWrapper.InnerHtml += this.viewContext.Controller.BsRenderPartialView(this.rowTemplate, item);

                    listItemWrapper.InnerHtml += templateWrapper;

                    listItem.InnerHtml += listItemWrapper;

                    list.InnerHtml += listItem;
                }

                result += list;
            }
            else
            {
                throw new Exception("You must set the template for tab " + this.uid.ToString());
            }

            return result;
        }

        internal string RenderContent()
        {
            BeforeRender();

            if (this.HasModel)
            {
                var result = this.toolbar.Render();

                if (this.hasItems)
                {
                    result += this.RenderItems();
                }

                result += RenderPager();

                return result;
            }

            return "";
        }

        public override string Render()
        {
            var wrapper = new TagBuilder("div");

            wrapper.MergeAttribute("data-tabid", ((int)this.uid).ToString());

            wrapper.MergeAttribute("data-loaded", this.HasModel.ToString());

            if (!this.selected)
            {
                wrapper.MergeAttribute("style", "display: none;");
            }

            wrapper.InnerHtml += this.RenderContent();

            return wrapper.ToString();
        }

        internal override string RenderAjax()
        {
            var result = "";

            if (this.hasItems)
            {
                result += this.RenderItems();
            }

            result += RenderPager();

            return result;
        }

        #endregion
    }
    #endregion
}