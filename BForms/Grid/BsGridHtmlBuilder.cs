using BForms.Models;
using BForms.Mvc;
using BForms.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Wordprocessing;

namespace BForms.Grid
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

        internal ModelMetadata Metadata
        {
            get { return metadata; }
            set { metadata = value; }
        }

        private string fullName;

        internal string FullName
        {
            get { return fullName; }
            set { fullName = value; }
        }

        private BsGridPagerBuilder pagerBuilder;
        private IDictionary<string, object> htmlAttributes;
        private bool renderTitle = true;
        private string resetButtonHtml;
        private Func<TRow, string> rowHighlighter;
        private Func<TRow, IDictionary<string, object>> rowData;
        private Func<TRow, bool> rowDetails;
        private Func<TRow, bool> rowCheckbox;
        private Func<TRow, string> rowDetailsTemplate;
        private bool hasDetails;
        private bool hasBulkActions;
        private bool allowAddIfEmpty;
        private List<BsGridColumn<TRow>> columns;
        private bool showColumnsHeader = true;
        private List<BsBulkAction> bulkActions;
        private List<BsBulkSelector> bulkSelectors;
        private bool renderPager = true;
        private BsPagerSettings pagerSettings = new BsPagerSettings();
        private BsTheme theme = BsTheme.Default;
        //private BsBulkActionsFactory BulkActionsFactory { get; set; }
        private string noRecordsTemplate;
        private string noResultsTemplate;
        private bool isAjaxRequest = false;

        internal void SetPropsFromAttributes(object[] attributes)
        {
            foreach (var item in attributes)
            {
                var attr = item as BsGridAttribute;
                if (attr != null)
                {
                    this.hasDetails = attr.HasDetails;
                    if (attr.Theme > 0)
                    {
                        this.theme = attr.Theme;
                    }
                }
            }
        }

        public BsGridHtmlBuilder()
        {
            this.SetColumnsFromModel();
        }

        public BsGridHtmlBuilder(string fullName, BsGridModel<TRow> model, ModelMetadata metadata, ViewContext viewContext)
            : base(viewContext)
        {
            this.fullName = fullName;
            this.model = model;
            this.metadata = metadata;

            this.SetColumnsFromModel();
        }

        /// <summary>
        /// Appends html attributes to grid_view div element
        /// </summary>
        public BsGridHtmlBuilder<TModel, TRow> HtmlAttributes(Dictionary<string, object> htmlAttributes)
        {
            this.htmlAttributes = htmlAttributes;
            return this;
        }

        /// <summary>
        /// Appends html attributes to grid_view div element
        /// </summary>
        public BsGridHtmlBuilder<TModel, TRow> HtmlAttributes(object htmlAttributes)
        {
            this.htmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            return this;
        }

        /// <summary>
        /// Set grids name
        /// </summary>
        public BsGridHtmlBuilder<TModel, TRow> DisplayName(string name)
        {
            this.metadata.DisplayName = name;
            return this;
        }

        public BsGridHtmlBuilder<TModel, TRow> ConfigureColumns(Action<BsGridColumnFactory<TRow>> configurator)
        {
            var columnFactory = new BsGridColumnFactory<TRow>(this.viewContext, this.columns);
            configurator(columnFactory);

            columnFactory.Validate();

            this.showColumnsHeader = columnFactory.ShowHeader;
            this.columns = columnFactory.Columns;

            return this;
        }

        public BsGridHtmlBuilder<TModel, TRow> ConfigureRows(Action<BsGridRowConfigurator<TRow>> configurator)
        {
            var rowConfigurator = new BsGridRowConfigurator<TRow>();

            configurator(rowConfigurator);

            this.rowData = rowConfigurator.HtmlAttr;

            this.rowHighlighter = rowConfigurator.Highlight;

            this.rowDetails = rowConfigurator.Details;

            this.rowCheckbox = rowConfigurator.Checkbox;

            this.rowDetailsTemplate = rowConfigurator.DetailsTmpl;

            return this;
        }

        public BsGridHtmlBuilder<TModel, TRow> ConfigureBulkActions(Action<BsBulkActionsFactory> configurator)
        {
            var bulkActionsFactory = new BsBulkActionsFactory(this.viewContext);
            configurator(bulkActionsFactory);

            this.bulkActions = bulkActionsFactory.BulkActions;
            this.bulkSelectors = bulkActionsFactory.BulkSelectors;
            this.hasBulkActions = true;

            return this;
        }

        public BsGridHtmlBuilder<TModel, TRow> GridResetButton()
        {
            var resetButton = new TagBuilder("div");
            resetButton.MergeAttribute("class", "btn btn-info bs-resetGrid reset-grid");
            resetButton.MergeAttribute("style", "display:none");
            resetButton.MergeAttribute("title", "Reset");

            var resetButtonSpan = new TagBuilder("span");
            resetButtonSpan.MergeAttribute("class", "glyphicon glyphicon-repeat");

            resetButton.InnerHtml += resetButtonSpan.ToString();

            this.resetButtonHtml = resetButton.ToString();

            return this;
        }

        public BsGridHtmlBuilder<TModel, TRow> AllowAddIfEmpty()
        {
            this.allowAddIfEmpty = true;
            return this;
        }

        public BsGridHtmlBuilder<TModel, TRow> NoPager()
        {
            this.renderPager = false;

            return this;
        }

        public BsGridHtmlBuilder<TModel, TRow> NoTitle()
        {
            this.renderTitle = false;

            return this;
        }

        public BsGridHtmlBuilder<TModel, TRow> PagerSettings(BsPagerSettings pagerSettings)
        {
            this.pagerSettings = pagerSettings;

            return this;
        }

        public BsGridHtmlBuilder<TModel, TRow> Theme(BsTheme theme)
        {
            this.theme = theme;

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
            var hideDetails = this.HideDetails();

            gridBuilder.MergeAttribute("id", this.fullName.Split('.').Last().ToLower());
            gridBuilder.MergeClassAttribute("grid_view", this.htmlAttributes);

            if (this.hasDetails)
            {
                gridBuilder.AddCssClass("is_expandable");
            }

            if (this.hasBulkActions)
            {
                gridBuilder.AddCssClass("is_checkable");
            }

            gridBuilder.MergeAttribute("data-settings",
                HtmlHelper.AnonymousObjectToHtmlAttributes(this.model.BaseSettings).ToJsonString());

            gridBuilder.MergeAttributes(this.htmlAttributes, true);

            gridBuilder.AddCssClass(this.theme.GetDescription());

            #region header builder
            if (this.renderTitle)
            {
                var headerBuilder = new TagBuilder("h2");

                var badgeBuilder = new TagBuilder("span");
                badgeBuilder.AddCssClass("badge");
                badgeBuilder.InnerHtml += this.model.Pager != null ? this.model.Pager.TotalRecords : 0;
                headerBuilder.InnerHtml += badgeBuilder.ToString();

                headerBuilder.InnerHtml += this.metadata.DisplayName;

                var filterIconBuilder = new TagBuilder("span");
                filterIconBuilder.AddCssClass("glyphicon glyphicon-filter icon_filter bs-filter");
                filterIconBuilder.MergeAttribute("title", "");
                headerBuilder.InnerHtml += filterIconBuilder.ToString();

                #region BulkActions

                if (this.hasBulkActions)
                {
                    var bulkActionsWrapper = new TagBuilder("div");
                    bulkActionsWrapper.MergeAttribute("class", "grid_bulk_controls bs-group_actions");

                    if (this.model.Items == null || !this.model.Items.Any())
                    {
                        bulkActionsWrapper.MergeAttribute("style","display:none");
                    }

                    var orderedBulkActions = this.bulkActions.OrderBy(x => x.BulkActionOrder);
                    foreach (var bulkAction in orderedBulkActions)
                    {
                        bulkActionsWrapper.InnerHtml += bulkAction.Render();
                    }


                    var bulkActionsSelectWrapper = new TagBuilder("div");
                    bulkActionsSelectWrapper.MergeAttribute("class", "btn-group check_all");

                    var bulkActionsSelectToggle = new TagBuilder("button");
                    bulkActionsSelectToggle.MergeAttribute("type", "button");
                    bulkActionsSelectToggle.MergeAttribute("class", "btn btn-default dropdown-toggle");
                    bulkActionsSelectToggle.MergeAttribute("data-toggle", "dropdown");
                    bulkActionsSelectToggle.MergeAttribute("title", "Select");

                    var bulkActionsSelectToggleCaret = new TagBuilder("span");
                    bulkActionsSelectToggleCaret.MergeAttribute("class", "caret");
                    bulkActionsSelectToggle.InnerHtml += bulkActionsSelectToggleCaret.ToString();

                    bulkActionsSelectWrapper.InnerHtml += bulkActionsSelectToggle.ToString();

                    var bulkActionSelectList = new TagBuilder("ul");
                    bulkActionSelectList.MergeAttribute("class", "dropdown-menu pull-right");
                    bulkActionSelectList.MergeAttribute("role", "menu");

                    var orderedBulkSelectors = bulkSelectors.OrderBy(x => x.Order);
                    foreach (var bulkSelector in orderedBulkSelectors)
                    {
                        bulkActionSelectList.InnerHtml += bulkSelector.Render();
                    }

                    bulkActionsSelectWrapper.InnerHtml += bulkActionSelectList.ToString();

                    var bulkActionsSelectCheckbox = new TagBuilder("input");
                    bulkActionsSelectCheckbox.MergeAttribute("type", "checkbox");

                    bulkActionsSelectWrapper.InnerHtml += bulkActionsSelectCheckbox.ToString();

                    bulkActionsWrapper.InnerHtml += bulkActionsSelectWrapper.ToString();

                    headerBuilder.InnerHtml += bulkActionsWrapper.ToString();
                }

                if (!String.IsNullOrEmpty(this.resetButtonHtml))
                {
                    headerBuilder.InnerHtml += this.resetButtonHtml;
                }
                #endregion

                gridBuilder.InnerHtml += headerBuilder.ToString();
            }
            #endregion

            var wrapper = new TagBuilder("div");
            wrapper.AddCssClass("grid_rows");

            #region columns builder
            if (this.showColumnsHeader)
            {
                var columnsBuilder = new TagBuilder("div");

                columnsBuilder.AddCssClass("row grid_row title");

                this.OrderColumns();

                if (this.hasDetails)
                {
                    var detailsBuilder = new TagBuilder("a");
                    detailsBuilder.MergeAttribute("class", "expand bs-toggleExpand");

                    if (this.model.BaseSettings.DetailsAll || this.model.BaseSettings.DetailsStartIndex == 0 && this.model.BaseSettings.DetailsCount >= this.model.Items.Count())
                    {
                        detailsBuilder.AddCssClass("open");
                    }

                    if (hideDetails)
                    {
                        detailsBuilder.MergeAttribute("style", "display:none");
                    }

                    detailsBuilder.MergeAttribute("href", "#");
                    detailsBuilder.InnerHtml += "&nbsp;";

                    columnsBuilder.InnerHtml += detailsBuilder.ToString();
                }

                var headerBuilder = new TagBuilder("header");

                headerBuilder.AddCssClass("bs-header");

                if (this.model.Items == null || !this.model.Items.Any())
                {
                    headerBuilder.MergeAttribute("style", "display: none;");
                }

                for (var i = 0; i < this.columns.Count; i++)
                {
                    var column = this.columns[i];

                    if (this.model.BaseSettings.OrderableColumns != null)
                    {
                        var orderModel = this.model.BaseSettings.OrderableColumns.Find(x => x.Name == column.PrivateName);
                        if (orderModel != null)
                        {
                            column.OrderType = orderModel.Type;
                        }
                    }

                    headerBuilder.InnerHtml += column.Render();
                }

                columnsBuilder.InnerHtml += headerBuilder.ToString();

                wrapper.InnerHtml += columnsBuilder.ToString();
            }
            #endregion

            wrapper.InnerHtml += this.RenderRows();

            gridBuilder.InnerHtml += wrapper.ToString();

            #region pager builder
            if (this.renderPager)
            {
                var pagerWrapper = pagerBuilder.Render();

                gridBuilder.InnerHtml += pagerWrapper;
            }

            #endregion

            return gridBuilder.ToString();
        }

        private string RenderRows()
        {
            var result = string.Empty;

            var rowsBuilder = new TagBuilder("div");
            rowsBuilder.MergeAttribute("class", "grid_rows_wrapper");

            if (this.model.Items.Any())
            {
                PropertyInfo hasDetailsProp = null;
                var rowType = typeof(TRow);

                var isSubClassOfBaseRowModel = rowType.IsSubclassOfRawGeneric(typeof(BsGridRowModel<>));
                if (isSubClassOfBaseRowModel)
                {
                    hasDetailsProp = rowType.GetProperty("HasDetails", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                }

                for (var rowIndex = 0; rowIndex < this.model.Items.Count(); rowIndex++)
                {
                    var row = this.model.Items.ElementAt(rowIndex);
             
                    var rowBuilder = new TagBuilder("div");
                    rowBuilder.MergeAttribute("class", "row grid_row");

                    if (this.model.BaseSettings.DetailsAll || this.model.BaseSettings.HasDetails(rowIndex))
                    {
                        rowBuilder.AddCssClass("open");
                        rowBuilder.MergeAttribute("data-hasdetails", true.ToString());
                        rowBuilder.MergeAttribute("data-expandedonload", true.ToString());
                    }

                    var rowHasDetails = this.hasDetails && (this.rowDetails == null || this.rowDetails(row));

                    if (rowData != null)
                    {
                        rowBuilder.MergeAttributes(rowData(row));
                    }

                    if (this.rowHighlighter != null)
                    {
                        var rowHighlighterBuilder = new TagBuilder("span");
                        rowHighlighterBuilder.MergeAttribute("class", "grid_row_color");
                        rowHighlighterBuilder.MergeAttribute("style", "background-color: " + rowHighlighter(row) + ";");

                        rowBuilder.InnerHtml += rowHighlighterBuilder.ToString();
                    }

                    if (rowHasDetails)
                    {
                        var detailsBUilder = new TagBuilder("a");
                        detailsBUilder.MergeAttribute("class", "expand bs-expand");
                        detailsBUilder.MergeAttribute("href", "#");
                        detailsBUilder.InnerHtml += "&nbsp;";

                        rowBuilder.InnerHtml += detailsBUilder.ToString();
                    }

                    var headerBuilder = new TagBuilder("header");

                    this.OrderColumns();

                    for (var i = 0; i < this.columns.Count; i++)
                    {
                        var column = this.columns.ElementAt(i);

                        var cellBuilder = new TagBuilder("div");

                        if (column.htmlAttributes != null)
                        {
                            cellBuilder.MergeAttributes(column.htmlAttributes);
                        }

                        cellBuilder.AddCssClass(column.GetWidthClasses());
                        
                        var text = string.Empty;

                        if (column.CellText == null)
                        {
                            if (column.Property != null)
                            {
                                text = column.Property.GetValue(row).ToString();
                            }
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

                        headerBuilder.InnerHtml += cellBuilder.ToString();
                    }

                    rowBuilder.InnerHtml += headerBuilder.ToString();

                    if (this.hasBulkActions)
                    {
                        var checkBuilder = new TagBuilder("input");
                        checkBuilder.MergeAttribute("type", "checkbox");
                        checkBuilder.MergeAttribute("class", "row_check bs-row_check");

                        if (this.rowCheckbox != null && !this.rowCheckbox(row))
                        {
                            checkBuilder.MergeAttribute("disabled", "disabled");
                        }

                        rowBuilder.InnerHtml += checkBuilder.ToString(TagRenderMode.SelfClosing);
                    }

                    if (rowHasDetails && ((hasDetailsProp != null && (bool)hasDetailsProp.GetValue(row)) || this.model.BaseSettings.HasDetails(rowIndex)))
                    {
                        rowBuilder.InnerHtml += this.rowDetailsTemplate(row);
                    }

                    rowsBuilder.InnerHtml += rowBuilder.ToString();
                }
                result += rowsBuilder.ToString();
            }
            else
            {
                rowsBuilder.AddCssClass("no_results");

                var rowBuilder = new TagBuilder("div");
                rowBuilder.MergeAttribute("class", "row grid_row");
                rowBuilder.AddCssClass("bs-noResultsRow");

                var divBuilder = new TagBuilder("div");
                divBuilder.MergeAttribute("class", "col-12 col-sm-12 col-lg-12");

                var infoBuilder = new TagBuilder("div");
                infoBuilder.MergeAttribute("class", "alert alert-info");


                var template = string.Empty;
                var stateClass = string.Empty;

                if (this.isAjaxRequest)
                {
                    template = this.noResultsTemplate;
                    stateClass = "noresults-state";
                }
                else
                {
                    template = this.noRecordsTemplate;
                    stateClass = "blank-state";
                }

                infoBuilder.AddCssClass(stateClass);

                infoBuilder.InnerHtml += !string.IsNullOrEmpty(template) ?
                                    this.viewContext.Controller.BsRenderPartialView(template, null) :
                                    BsResourceManager.Resource("NoResults");

                if (this.allowAddIfEmpty)
                {
                    var addBtnBuilder = new TagBuilder("button");
                    addBtnBuilder.MergeAttribute("type", "button");
                    addBtnBuilder.MergeAttribute("class", "btn btn-primary bs-triggerAdd");
                    //TODO:
                    addBtnBuilder.InnerHtml += "Add";

                    infoBuilder.InnerHtml += addBtnBuilder.ToString();
                }

                divBuilder.InnerHtml += infoBuilder.ToString();
                rowBuilder.InnerHtml += divBuilder.ToString();

                rowsBuilder.InnerHtml += rowBuilder.ToString();

                result += rowsBuilder.ToString();
            }

            return result;
        }

        private string RenderPages()
        {
            return pagerBuilder.RenderPages();
        }

        private string RenderAjax()
        {
            this.isAjaxRequest = true;

            return this.RenderRows() + this.RenderPages();
        }

        public override string Render()
        {
            this.pagerBuilder = new BsGridPagerBuilder(this.model.Pager, this.pagerSettings, this.model.BaseSettings).Theme(this.theme);

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
                    if (columnAttr.Usage != BsGridColumnUsage.Excel)
                    {
                        var column = new BsGridColumn<TRow>(property, this.viewContext);

                        column.IsSortable = columnAttr.IsSortable;
                        column.SetWidth(columnAttr.Width);
                        column.SetOrder(columnAttr.Order);

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

        private void OrderColumns()
        {
            if (this.model.BaseSettings.OrderColumns != null && this.model.BaseSettings.OrderColumns.Any())
            {
                foreach (var column in this.columns)
                {
                    var name = column.PrivateName;
                    if (this.model.BaseSettings.OrderColumns.ContainsKey(name))
                    {
                        column.Order = this.model.BaseSettings.OrderColumns[name];
                    }

                }
            }

            this.columns = this.columns.OrderByDescending(c => c.Order.HasValue).ThenBy(x => x.Order).ToList();
        }

        private bool HideDetails()
        {
            if (this.hasDetails)
            {
                if (this.rowDetails == null)
                {
                    return false;
                }
                else if (this.model.Items != null && this.model.Items.Any())
                {
                    return this.model.Items.All(item => !this.rowDetails(item));
                }
            }
            return true;
        }
    }
}