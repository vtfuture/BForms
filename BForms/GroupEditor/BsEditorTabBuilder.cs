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
    public abstract class BsEditorTabBuilder : BaseComponent
    {
        protected string rowTemplate { get; set; }

        internal abstract bool IsSelected();

        internal abstract void SetSelected(bool selected);

        public BsEditorTabBuilder<TRow, TSearch> For<TModel, TRow, TSearch>(TModel model, Expression<Func<TModel, BsGroupEditor<TRow, TSearch>>> expression) where TRow : new ()
        {
            return this as BsEditorTabBuilder<TRow, TSearch>;
        }
    }

    public class BsEditorTabBuilder<TRow, TSearch> : BsEditorTabBuilder<TRow> where TRow : new ()
    {
        private BsGroupEditor<TRow, TSearch> model;
        protected string searchTemplate;

        public BsEditorTabBuilder(BsGroupEditor<TRow, TSearch> model, ViewContext viewContext)
            : base(model.Grid, model.InlineSearch, viewContext)
        {
            this.model = model;
            this.toolbar = new BsEditorToolbarHtmlBuilder(model.InlineSearch);
                                    
            this.toolbar.AddButton(Glyphicon.Search).DisplayName("Cauta");
        }

        public BsEditorTabBuilder<TRow, TSearch> Template(Expression<Func<BsGroupEditor<TRow, TSearch>, TSearch>> expression, string template)
        {
            this.searchTemplate = template;

            return this;
        }

        public BsEditorTabBuilder<TRow, TSearch> Template(Expression<Func<BsGroupEditor<TRow, TSearch>, BsGridModel<TRow>>> expression, string template)
        {
            this.rowTemplate = template;

            return this;
        }
    }

    public class BsEditorTabBuilder<TRow, TSearch, TNew> : BsEditorTabBuilder<TRow, TSearch> where TRow : new ()
    {
        private BsGroupEditor<TRow, TSearch, TNew> model;
        protected string newTemplate;

        public BsEditorTabBuilder(BsGroupEditor<TRow, TSearch, TNew> model, ViewContext viewContext)
            : base(model, viewContext)
        {
            this.model = model;
            this.toolbar = new BsEditorToolbarHtmlBuilder(model.InlineSearch);

            this.toolbar.AddButton(Glyphicon.Search).DisplayName("Cauta");
            this.toolbar.AddButton(Glyphicon.Plus).DisplayName("Adauga");
        }

        public BsEditorTabBuilder<TRow, TSearch, TNew> Template(Expression<Func<BsGroupEditor<TRow, TSearch, TNew>, TSearch>> expression, string template)
        {
            this.searchTemplate = template;

            return this;
        }

        public BsEditorTabBuilder<TRow, TSearch, TNew> Template(Expression<Func<BsGroupEditor<TRow, TSearch, TNew>, TNew>> expression, string template)
        {
            this.newTemplate = template;

            return this;
        }

        public BsEditorTabBuilder<TRow, TSearch, TNew> Template(Expression<Func<BsGroupEditor<TRow, TSearch, TNew>, BsGridModel<TRow>>> expression, string template)
        {
            this.rowTemplate = template;

            return this;
        }
    }

    public class BsEditorTabBuilder<TRow> : BsEditorTabBuilder where TRow : new()
    {
        #region Properties and Constructor
        protected BsEditorToolbarHtmlBuilder toolbar { get; set; }
        private BsGridPagerBuilder pagerBuilder;
        private BsPagerSettings pagerSettings;
        private BsGridModel<TRow> model;

        private string name { get; set; }
        private object uid { get; set; }
        private bool selected { get; set; }

        internal string Name
        {
            get { return this.name; }
        }

        internal object Uid
        {
            get { return this.uid; }
        }

        public BsEditorTabBuilder(BsGridModel<TRow> model, bool inlineSearch, ViewContext viewContext)
        {
            this.viewContext = viewContext;
            this.model = model;
            this.toolbar = new BsEditorToolbarHtmlBuilder(inlineSearch);
            this.pagerSettings = new BsPagerSettings();
        }
        #endregion

        #region Config
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

        public BsEditorTabBuilder<TRow> DisplayName(string name)
        {
            this.name = name;

            return this;
        }

        public BsEditorTabBuilder<TRow> Id(object uid)
        {
            this.uid = uid;

            return this;
        }

        public BsEditorTabBuilder<TRow> PagerSettings(BsPagerSettings pagerSettings)
        {
            this.pagerSettings = pagerSettings;

            return this;
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
        internal string RenderItems()
        {
            var result = "";

            if (this.model.Items != null && this.model.Items.Any())
            {
                if (!string.IsNullOrEmpty(this.rowTemplate))
                {
                    var list = new TagBuilder("ul");

                    list.AddCssClass("group_profiles");

                    foreach (var item in this.model.Items)
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
            }

            return result;
        }

        internal string RenderContent()
        {
            return this.toolbar.Render() + this.RenderItems() + this.pagerBuilder.Render();
        }

        public override string Render()
        {
            this.pagerBuilder = new BsGridPagerBuilder(model.Pager, this.pagerSettings, model.BaseSettings);

            var wrapper = new TagBuilder("div");

            wrapper.MergeAttribute("data-tabid", ((int)this.uid).ToString());

            if (!this.selected)
            {
                wrapper.MergeAttribute("style", "display: none;");
            }

            wrapper.InnerHtml += this.RenderContent();

            return wrapper.ToString();
        }
        #endregion
    }
}
