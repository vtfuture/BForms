using BootstrapForms.Models;
using BootstrapForms.Mvc;
using BootstrapForms.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace BootstrapForms.Grid
{
    public class BsGridHtmlBuilder<TModel, TRow> : BaseComponent where TRow : new()
    {
        private BsGridModel<TRow> model;

        internal BsGridModel<TRow> Model
        {
            set
            {
                this.model = value;
            }
        }

        private ModelMetadata metadata;

        public ModelMetadata Metadata
        {
            get { return metadata; }
            set { metadata = value; }
        }

        private string fullName;

        public string FullName
        {
            get { return fullName; }
            set { fullName = value; }
        }

        private Dictionary<string, object> htmlAttributes;
        private string multipleSelectActionsHtml;
        private Func<TRow, string> rowHighlighter;
        private Func<TRow, Dictionary<string, object>> rowData;
        private bool hasDetails;
        private List<BsGridColumn<TRow>> columns;
        private BsPagerSettings pagerSettings = new BsPagerSettings();
        private string noRecordsTemplate;
        private string noResultsTemplate;

        public BsGridHtmlBuilder() { }

        public BsGridHtmlBuilder(string fullName, BsGridModel<TRow> model, ModelMetadata metadata, ViewContext viewContext)
            : base(viewContext)
        {
            this.fullName = fullName;
            this.model = model;
            this.metadata = metadata;


            BsGridAttribute gridAttr = null;
            if (ReflectionHelpers.TryGetAttribute(fullName, typeof(TModel), out gridAttr))
            {
                this.hasDetails = gridAttr.HasDetails;
            }

            this.SetColumnsFromModel();
        }
        public BsGridHtmlBuilder<TModel, TRow> HtmlAttributes(Dictionary<string, object> htmlAttributes)
        {
            this.htmlAttributes = htmlAttributes;
            return this;
        }

        public BsGridHtmlBuilder<TModel, TRow> ConfigureColumns(Action<BsGridColumnFactory<TRow>> configurator)
        {
            var columnFactory = new BsGridColumnFactory<TRow>(this.viewContext, this.columns);
            configurator(columnFactory);
            this.columns = columnFactory.Columns;

            return this;
        }

        public BsGridHtmlBuilder<TModel, TRow> MultipleSelectActions(string multipleSelectActionsHtml)
        {
            this.multipleSelectActionsHtml = multipleSelectActionsHtml;
            return this;
        }

        public BsGridHtmlBuilder<TModel, TRow> RowHighlighter(Func<TRow, string> higlighter)
        {
            this.rowHighlighter = higlighter;

            return this;
        }

        public BsGridHtmlBuilder<TModel, TRow> RowData(Func<TRow, Dictionary<string, object>> rowData)
        {
            this.rowData = rowData;

            return this;
        }

        public BsGridHtmlBuilder<TModel, TRow> PagerSettings(Func<TRow, Dictionary<string, object>> rowData)
        {
            this.rowData = rowData;

            return this;
        }

        public BsGridHtmlBuilder<TModel, TRow> NoRecordsTemplate(string template)
        {
            this.noRecordsTemplate = template;

            return this;
        }

        public BsGridHtmlBuilder<TModel, TRow> NoResultsTemplate(string template)
        {
            this.noResultsTemplate = template;

            return this;
        }

        private string RenderIndex()
        {
            var gridBuilder = new TagBuilder("div");
            gridBuilder.MergeAttribute("id", this.fullName.Split('.').Last().ToLower());
            gridBuilder.MergeClassAttribute("grid_view", this.htmlAttributes);
            gridBuilder.MergeAttributes(this.htmlAttributes, true);

            #region header builder
            var headerBuilder = new TagBuilder("h2");

            var badgeBuilder = new TagBuilder("span");
            badgeBuilder.MergeAttribute("class", "badge");
            badgeBuilder.InnerHtml += this.model.Pager.TotalRecords;
            headerBuilder.InnerHtml += badgeBuilder.ToString();

            headerBuilder.InnerHtml += this.metadata.DisplayName;

            var filterIconBuilder = new TagBuilder("span");
            filterIconBuilder.MergeAttribute("class", "glyphicon glyphicon-filter icon_filter js-filter");
            filterIconBuilder.MergeAttribute("title", "");
            headerBuilder.InnerHtml += filterIconBuilder.ToString();

            headerBuilder.InnerHtml += this.multipleSelectActionsHtml;

            gridBuilder.InnerHtml += headerBuilder.ToString();
            #endregion

            var wrapper = new TagBuilder("div");
            wrapper.MergeAttribute("class", "grid_wrapper");

            #region columns builder

            if (!this.columns.Any())
            {
                throw new NotImplementedException("You must define your grid columns either in model as data attributes or in the view");
            }

            var columnsBuilder = new TagBuilder("div");

            columnsBuilder.MergeAttribute("class", "row grid_row title");

            foreach (var column in this.columns)
            {
                columnsBuilder.InnerHtml += column.Render();
                wrapper.InnerHtml = columnsBuilder.ToString();
            }
            #endregion

            wrapper.InnerHtml += this.RenderRows();

            gridBuilder.InnerHtml += wrapper.ToString();

            #region pager builder
            if (this.model.Pager != null && this.model.Pager.TotalRecords > 0)
            {
                var pagerWrapper = new TagBuilder("div");
                pagerWrapper.MergeAttribute("class", "row grid_pager");

                pagerWrapper.InnerHtml += this.RenderPages();

                if (this.pagerSettings.HasPageSizeSelector)
                {
                    int pageSize = this.model.Pager.PageSize;
                    if (!this.pagerSettings.PageSizeValues.Contains(pageSize))
                        throw new ArgumentOutOfRangeException("The page size you selected is not in the list");

                    var selectWrapperBuilder = new TagBuilder("div");
                    selectWrapperBuilder.MergeAttribute("class", "col-md-6 col-lg-6 results_per_page");

                    TagBuilder divBuilder = new TagBuilder("div");
                    divBuilder.MergeAttribute("class", "pull-right");

                    TagBuilder spanBuilder = new TagBuilder("span");
                    //TODO:
                    spanBuilder.InnerHtml += "Rezultate per pagina";

                    divBuilder.InnerHtml += spanBuilder.ToString();

                    var selectBuilder = new TagBuilder("select");
                    selectBuilder.MergeAttribute("class", "rowsPerPageSelector");

                    var selectedVal = this.model.Pager.PageSize;

                    foreach (var item in this.pagerSettings.PageSizeValues)
                    {
                        var optionsBuilder = new TagBuilder("option");
                        if (selectedVal == item)
                        {
                            optionsBuilder.MergeAttribute("selected", string.Empty);
                        }
                        optionsBuilder.InnerHtml += item;
                        selectBuilder.InnerHtml += optionsBuilder.ToString();
                    }

                    divBuilder.InnerHtml += selectBuilder.ToString();

                    selectWrapperBuilder.InnerHtml += divBuilder.ToString();

                    pagerWrapper.InnerHtml += selectWrapperBuilder.ToString();
                }

                gridBuilder.InnerHtml += pagerWrapper.ToString();
            }

            #endregion

            return gridBuilder.ToString();
        }

        private string RenderRows()
        {
            var result = string.Empty;

            if (this.model.Items.Any())
            {
                var rowsBuilder = new TagBuilder("div");
                rowsBuilder.MergeAttribute("class", "row_wrapper");

                foreach (var row in this.model.Items)
                {
                    var rowBuilder = new TagBuilder("div");
                    rowBuilder.MergeAttribute("class", "row grid_row");
                    if (rowData != null)
                    {
                        rowBuilder.MergeAttributes(rowData(row));
                    }

                    var headerBuilder = new TagBuilder("header");

                    for (var i = 0; i < this.columns.Count; i++)
                    {
                        var column = this.columns.ElementAt(i);

                        var cellBuilder = new TagBuilder("div");
                        cellBuilder.MergeAttribute("class", "col-lg-" + column.Width);

                        if (i == 0)
                        {
                            if (this.hasDetails)
                            {
                                var detailsBUilder = new TagBuilder("a");
                                detailsBUilder.MergeAttribute("class", "expand");
                                detailsBUilder.MergeAttribute("href", "#");
                                detailsBUilder.InnerHtml += "&nbsp;";

                                cellBuilder.InnerHtml += detailsBUilder.ToString();
                            }

                            if (this.rowHighlighter != null)
                            {
                                var rowHighlighterBuilder = new TagBuilder("span");
                                rowHighlighterBuilder.MergeAttribute("class", "row_color");
                                rowHighlighterBuilder.MergeAttribute("style", "background-color: " + rowHighlighter(row) + ";");

                                cellBuilder.InnerHtml += rowHighlighterBuilder.ToString();
                            }
                        }

                        var text = string.Empty;

                        if (column.CellText == null)
                        {
                            text = column.Property.GetValue(row).ToString();
                        }
                        else
                        {
                            text = column.CellText(row).ToString();
                        }

                        if (column.IsEditable)
                        {
                            var editBuilder = new TagBuilder("span");
                            editBuilder.MergeAttribute("class", "edit_col");
                            editBuilder.InnerHtml = text;

                            cellBuilder.InnerHtml += editBuilder.ToString();
                        }
                        else
                        {
                            cellBuilder.InnerHtml += text;
                        }

                        if (i == this.columns.Count - 1 && !string.IsNullOrEmpty(this.multipleSelectActionsHtml))
                        {
                            var checkBuilder = new TagBuilder("input");
                            checkBuilder.MergeAttribute("type", "checkbox");
                            checkBuilder.MergeAttribute("class", "js-row_check");

                            cellBuilder.InnerHtml += checkBuilder.ToString(TagRenderMode.SelfClosing);
                        }

                        headerBuilder.InnerHtml += cellBuilder.ToString();
                    }

                    rowBuilder.InnerHtml += headerBuilder.ToString();

                    rowsBuilder.InnerHtml += rowBuilder.ToString();
                }
                result += rowsBuilder.ToString();
            }
            else
            {
                var rowBuilder = new TagBuilder("div");
                rowBuilder.MergeAttribute("class", "row grid_row");

                var divBuilder = new TagBuilder("div");
                divBuilder.MergeAttribute("class", "col-12 col-sm-12 col-lg-12");

                var infoBuilder = new TagBuilder("div");
                infoBuilder.MergeAttribute("class", "alert alert-info");

                //TODO:
                if (true/*searched*/)
                {
                    infoBuilder.InnerHtml += "Your search generated no results. Modify your search.";//"Cautarea ta nu a generat rezultate. Modifica criteriile de cautare";
                }
                else
                {
                    infoBuilder.InnerHtml += "There are no records.";//"Nu sunt inregistrari";

                    var addBtnBuilder = new TagBuilder("button");
                    addBtnBuilder.MergeAttribute("type", "button");
                    addBtnBuilder.MergeAttribute("class", "btn btn-primary js-add");
                    addBtnBuilder.InnerHtml += "Adauga";

                    infoBuilder.InnerHtml += addBtnBuilder.ToString();
                }

                divBuilder.InnerHtml += infoBuilder.ToString();
                rowBuilder.InnerHtml += divBuilder.ToString();
            }

            return result;
        }

        private string RenderPages()
        {
            if (this.model.Pager != null)
            {
                var pagesBuilder = new TagBuilder("div");
                pagesBuilder.MergeAttribute("class", "col-md-6 col-lg-6 js-pages");

                #region pagination

                var paginationBuilder = new TagBuilder("ul");
                paginationBuilder.MergeAttribute("class", "pagination pagination-sm");

                #region first page button

                if (pagerSettings.ShowFirstLastButtons)
                {
                    var firstPageBuilder = new TagBuilder("li");
                    if (this.model.Pager.CurrentPage == 1)
                    {
                        firstPageBuilder.MergeAttribute("class", "disabled");
                    }
                    var anchorBuilder = new TagBuilder("a");
                    anchorBuilder.MergeAttribute("href", "#");
                    anchorBuilder.MergeAttribute("data-page", "1");
                    anchorBuilder.InnerHtml += "&laquo;";

                    firstPageBuilder.InnerHtml += anchorBuilder.ToString();

                    paginationBuilder.InnerHtml += firstPageBuilder.ToString();
                }

                #endregion

                #region prev page button

                if (pagerSettings.ShowPrevNextButtons)
                {
                    var prevPageBuilder = new TagBuilder("li");
                    if (this.model.Pager.CurrentPage == 1)
                    {
                        prevPageBuilder.MergeAttribute("class", "disabled");
                    }
                    var anchorBuilder = new TagBuilder("a");
                    anchorBuilder.MergeAttribute("href", "#");
                    anchorBuilder.MergeAttribute("data-page", (model.Pager.CurrentPage - 1).ToString());
                    anchorBuilder.InnerHtml += "&lsaquo;";

                    prevPageBuilder.InnerHtml += anchorBuilder.ToString();

                    paginationBuilder.InnerHtml += prevPageBuilder.ToString();
                }

                #endregion

                #region pages buttons

                var startPage = this.model.Pager.GetStartPage(this.pagerSettings.Size);
                int nr = this.model.Pager.TotalPages % this.pagerSettings.Size;
                for (int i = 0; i < nr; i++)
                {
                    var page = i + startPage;
                    var pageBtnBuilder = new TagBuilder("li");
                    if (this.model.Pager.CurrentPage == page)
                    {
                        pageBtnBuilder.MergeAttribute("class", "active");
                    }
                    var anchorBuilder = new TagBuilder("a");
                    anchorBuilder.MergeAttribute("href", "#");
                    anchorBuilder.MergeAttribute("data-page", page.ToString());
                    anchorBuilder.InnerHtml += page;

                    pageBtnBuilder.InnerHtml += anchorBuilder.ToString();

                    paginationBuilder.InnerHtml += pageBtnBuilder.ToString();
                }

                #endregion

                #region next page button

                if (pagerSettings.ShowPrevNextButtons)
                {
                    var nextPageBuilder = new TagBuilder("li");
                    if (this.model.Pager.CurrentPage == this.model.Pager.TotalPages)
                    {
                        nextPageBuilder.MergeAttribute("class", "disabled");
                    }
                    var anchorBuilder = new TagBuilder("a");
                    anchorBuilder.MergeAttribute("href", "#");
                    anchorBuilder.MergeAttribute("data-page", (model.Pager.CurrentPage + 1).ToString());
                    anchorBuilder.InnerHtml += "&rsaquo;";

                    nextPageBuilder.InnerHtml += anchorBuilder.ToString();

                    paginationBuilder.InnerHtml += nextPageBuilder.ToString();
                }

                #endregion

                #region last page button

                if (pagerSettings.ShowFirstLastButtons)
                {
                    var lastPageBuilder = new TagBuilder("li");
                    if (this.model.Pager.CurrentPage == this.model.Pager.TotalPages)
                    {
                        lastPageBuilder.MergeAttribute("class", "disabled");
                    }
                    var anchorBuilder = new TagBuilder("a");
                    anchorBuilder.MergeAttribute("href", "#");
                    anchorBuilder.MergeAttribute("data-page", this.model.Pager.TotalPages.ToString());
                    anchorBuilder.InnerHtml += "&raquo;";

                    lastPageBuilder.InnerHtml += anchorBuilder.ToString();

                    paginationBuilder.InnerHtml += lastPageBuilder.ToString();
                }

                #endregion

                pagesBuilder.InnerHtml += paginationBuilder.ToString();

                #endregion

                #region text

                if (this.pagerSettings.HasPagesText)
                {
                    var textBuilder = new TagBuilder("div");
                    textBuilder.MergeAttribute("class", "results_number");

                    var firstIdx = (this.model.Pager.CurrentPage - 1) * this.model.Pager.PageSize + 1;
                    var lastIdx = this.model.Pager.CurrentPage == this.model.Pager.TotalPages
                                      ? this.model.Pager.TotalRecords
                                      : this.model.Pager.CurrentPage * this.model.Pager.PageSize;

                    var totalCountBuilder = new TagBuilder("span");
                    totalCountBuilder.InnerHtml += this.model.Pager.TotalRecords;
                    
                    //TODO:
                    var template = "{0}-{1} of {2} items";
                    var result = string.Format(template, firstIdx, lastIdx, totalCountBuilder.ToString()); //"Rezultate " + firstIdx + "–" + lastIdx + " din";

                    textBuilder.InnerHtml += result;

                    pagesBuilder.InnerHtml += textBuilder.ToString();
                }

                #endregion

                return pagesBuilder.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        private string RenderAjax()
        {
            return this.RenderRows() + this.RenderPages();
        }

        public override string Render()
        {
            var result = this.viewContext.RequestContext.HttpContext.Request.IsAjaxRequest() ?
                this.RenderAjax() :
                this.RenderIndex();
            return result;
        }

        private void SetColumnsFromModel()
        {
            this.columns = new List<BsGridColumn<TRow>>();

            Type type = typeof(TRow);
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                BsGridColumnAttribute columnAttr = null;
                if (ReflectionHelpers.TryGetAttribute(property, out columnAttr))
                {
                    var column = new BsGridColumn<TRow>(property, this.viewContext);

                    column.IsSortable = columnAttr.IsSortable;
                    column.Width = columnAttr.Width;

                    System.ComponentModel.DataAnnotations.DisplayAttribute displayAttribute = null;
                    if (ReflectionHelpers.TryGetAttribute(property, out displayAttribute))
                    {
                        column.DisplayName = displayAttribute.GetName();
                    }
                    else
                    {
                        column.DisplayName = property.Name;
                    }

                    this.columns.Add(column);
                }
            }
        }
    }
}