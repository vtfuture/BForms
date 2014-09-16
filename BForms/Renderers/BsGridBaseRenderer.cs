using System.Runtime.CompilerServices;
using BForms.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BForms.Utilities;
using BForms.Models;
using BForms.Mvc;
using DocumentFormat.OpenXml.Spreadsheet;

namespace BForms.Renderers
{
    public class BsGridBaseRenderer<TModel, TRow> : BsBaseRenderer<BsGridHtmlBuilder<TModel, TRow>> where TRow : BsItemModel, new()
    {
        public BsGridBaseRenderer()
        {

        }

        public BsGridBaseRenderer(BsGridHtmlBuilder<TModel, TRow> builder)
            : base(builder)
        {

        }

        public override string Render()
        {
            this.Builder.pagerBuilder = new BsGridPagerBuilder(this.Builder.Model.Pager, this.Builder.pagerSettings, this.Builder.Model.BaseSettings)
            {
                Theme = this.Builder.Theme
            };

            var ignoreAjaxRequest = this.Builder.ignoreAjaxRequest.HasValue && this.Builder.ignoreAjaxRequest.Value;
            var result = this.Builder.IsAjaxRequest() && !ignoreAjaxRequest ?
               this.RenderAjax() :
               this.RenderIndex();

            return result;
        }

        public override string RenderAjax()
        {
            this.Builder.isAjaxRequest = true;

            return this.RenderRows() + this.RenderPages();
        }

        protected virtual string RenderIndex()
        {
            var gridBuilder = new TagBuilder("div");
            var hideDetails = this.Builder.HideDetails();

            gridBuilder.MergeAttribute("id", this.Builder.FullName.Split('.').Last().ToLower());
            gridBuilder.MergeClassAttribute("grid_view", this.Builder.htmlAttributes);

            if (this.Builder.hasDetails)
            {
                gridBuilder.AddCssClass("is_expandable");
            }

            if (this.Builder.hasBulkActions)
            {
                gridBuilder.AddCssClass("is_checkable");
            }

            gridBuilder.MergeAttribute("data-settings",
                HtmlHelper.AnonymousObjectToHtmlAttributes(this.Builder.Model.BaseSettings).ToJsonString());

            gridBuilder.MergeAttributes(this.Builder.htmlAttributes, true);

            gridBuilder.AddCssClass(this.Builder.Theme.GetDescription());

            #region header builder
            if (this.Builder.renderTitle)
            {
                var headerBuilder = new TagBuilder("h2");

                var badgeBuilder = new TagBuilder("span");
                badgeBuilder.AddCssClass("badge");
                badgeBuilder.InnerHtml += this.Builder.Model.Pager != null ? this.Builder.Model.Pager.TotalRecords : 0;
                headerBuilder.InnerHtml += badgeBuilder.ToString();

                headerBuilder.InnerHtml += this.Builder.Metadata.DisplayName;

                var filterIconBuilder = new TagBuilder("span");
                filterIconBuilder.AddCssClass("glyphicon glyphicon-filter icon_filter bs-filter");
                filterIconBuilder.MergeAttribute("title", "");
                headerBuilder.InnerHtml += filterIconBuilder.ToString();

                #region BulkActions

                if (this.Builder.hasBulkActions)
                {
                    var bulkActionsWrapper = new TagBuilder("div");
                    bulkActionsWrapper.MergeAttribute("class", "grid_bulk_controls bs-group_actions");

                    if (this.Builder.Model.Items == null || !this.Builder.Model.Items.Any())
                    {
                        bulkActionsWrapper.MergeAttribute("style", "display:none");
                    }

                    var orderedBulkActions = this.Builder.bulkActions.OrderBy(x => x.BulkActionOrder);
                    foreach (var bulkAction in orderedBulkActions)
                    {
                        bulkActionsWrapper.InnerHtml += bulkAction.ToString();
                    }


                    var bulkActionsSelectWrapper = new TagBuilder("div");
                    bulkActionsSelectWrapper.MergeAttribute("class", "btn-group check_all");

                    var bulkActionsSelectToggle = new TagBuilder("button");
                    bulkActionsSelectToggle.MergeAttribute("type", "button");
                    bulkActionsSelectToggle.MergeAttribute("class", "btn btn-default dropdown-toggle bs-selectorsContainer");
                    bulkActionsSelectToggle.MergeAttribute("data-toggle", "dropdown");
                    bulkActionsSelectToggle.MergeAttribute("title", BsResourceManager.Resource("BF_Select"));

                    var bulkActionsSelectToggleCaret = new TagBuilder("span");
                    bulkActionsSelectToggleCaret.MergeAttribute("class", "caret");
                    bulkActionsSelectToggle.InnerHtml += bulkActionsSelectToggleCaret.ToString();

                    bulkActionsSelectWrapper.InnerHtml += bulkActionsSelectToggle.ToString();

                    var bulkActionSelectList = new TagBuilder("ul");
                    bulkActionSelectList.MergeAttribute("class", "dropdown-menu pull-right");
                    bulkActionSelectList.MergeAttribute("role", "menu");

                    var orderedBulkSelectors = this.Builder.bulkSelectors.OrderBy(x => x.Order);
                    foreach (var bulkSelector in orderedBulkSelectors)
                    {
                        bulkActionSelectList.InnerHtml += bulkSelector.ToString();
                    }

                    bulkActionsSelectWrapper.InnerHtml += bulkActionSelectList.ToString();

                    var bulkActionsSelectCheckbox = new TagBuilder("input");
                    bulkActionsSelectCheckbox.MergeAttribute("type", "checkbox");

                    bulkActionsSelectWrapper.InnerHtml += bulkActionsSelectCheckbox.ToString();

                    bulkActionsWrapper.InnerHtml += bulkActionsSelectWrapper.ToString();

                    headerBuilder.InnerHtml += bulkActionsWrapper.ToString();
                }

                if (!String.IsNullOrEmpty(this.Builder.resetButtonHtml))
                {
                    headerBuilder.InnerHtml += this.Builder.resetButtonHtml;
                }
                #endregion

                gridBuilder.InnerHtml += headerBuilder.ToString();
            }
            #endregion

            var wrapper = new TagBuilder("div");
            wrapper.AddCssClass("grid_rows");

            #region columns builder
            if (this.Builder.showColumnsHeader)
            {
                var columnsBuilder = new TagBuilder("div");

                columnsBuilder.AddCssClass("row grid_row title");

                this.Builder.OrderColumns();

                if (this.Builder.hasDetails)
                {
                    var detailsBuilder = new TagBuilder("a");
                    detailsBuilder.MergeAttribute("class", "expand bs-toggleExpand");

                    if (this.Builder.Model.BaseSettings.DetailsAll || this.Builder.Model.BaseSettings.DetailsStartIndex == 0 && this.Builder.Model.BaseSettings.DetailsCount >= this.Builder.Model.Items.Count())
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

                if (this.Builder.Model.Items == null || !this.Builder.Model.Items.Any())
                {
                    columnsBuilder.MergeAttribute("style", "display: none;");
                }

                for (var i = 0; i < this.Builder.columns.Count; i++)
                {
                    var column = this.Builder.columns[i];

                    if (this.Builder.Model.BaseSettings.OrderableColumns != null)
                    {
                        var orderModel = this.Builder.Model.BaseSettings.OrderableColumns.Find(x => x.Name == column.PrivateName);
                        if (orderModel != null)
                        {
                            column.OrderType = orderModel.Type;
                        }
                    }

                    headerBuilder.InnerHtml += column.ToString();
                }

                columnsBuilder.InnerHtml += headerBuilder.ToString();

                wrapper.InnerHtml += columnsBuilder.ToString();
            }
            #endregion

            wrapper.InnerHtml += this.RenderRows();

            gridBuilder.InnerHtml += wrapper.ToString();

            #region pager builder
            if (this.Builder.renderPager)
            {
                var pagerWrapper = this.Builder.pagerBuilder.ToString();

                gridBuilder.InnerHtml += pagerWrapper;
            }

            #endregion

            return gridBuilder.ToString();
        }

        protected virtual string RenderPages()
        {
            return ((BsPagerBaseRenderer)this.Builder.pagerBuilder.renderer).RenderPages();
        }

        protected virtual string RenderRows()
        {
            var result = string.Empty;

            var rowsBuilder = new TagBuilder("div");
            rowsBuilder.MergeAttribute("class", "grid_rows_wrapper");

            if (this.Builder.Model.Items.Any())
            {
                PropertyInfo hasDetailsProp = null;
                var rowType = typeof(TRow);

                var isSubClassOfBaseRowModel = rowType.InheritsOrImplements(typeof(BsGridRowModel<>));
                if (isSubClassOfBaseRowModel)
                {
                    hasDetailsProp = rowType.GetProperty("HasDetails", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                }

                for (var rowIndex = 0; rowIndex < this.Builder.Model.Items.Count(); rowIndex++)
                {
                    var row = this.Builder.Model.Items.ElementAt(rowIndex);

                    var rowBuilder = new TagBuilder("div");
                    rowBuilder.MergeAttribute("class", "row grid_row");

                    rowBuilder.MergeAttribute("data-objid", row.GetUniqueID().ToString());

                    if (this.Builder.Model.BaseSettings.DetailsAll || this.Builder.Model.BaseSettings.HasDetails(rowIndex))
                    {
                        rowBuilder.AddCssClass("open");
                        rowBuilder.MergeAttribute("data-hasdetails", true.ToString());
                        rowBuilder.MergeAttribute("data-expandedonload", true.ToString());
                    }

                    var rowHasDetails = this.Builder.hasDetails && (this.Builder.rowDetails == null || this.Builder.rowDetails(row));

                    if (this.Builder.rowData != null)
                    {
                        rowBuilder.MergeAttributes(this.Builder.rowData(row));
                    }

                    if (this.Builder.rowHighlighter != null)
                    {
                        var rowHighlighterBuilder = new TagBuilder("span");
                        rowHighlighterBuilder.MergeAttribute("class", "grid_row_color");
                        rowHighlighterBuilder.MergeAttribute("style", "background-color: " + this.Builder.rowHighlighter(row) + ";");

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

                    this.Builder.OrderColumns();

                    for (var i = 0; i < this.Builder.columns.Count; i++)
                    {
                        var column = this.Builder.columns.ElementAt(i);

                        var cellBuilder = new TagBuilder("div");

                        if (column.htmlAttributes != null)
                        {
                            cellBuilder.MergeAttributes(column.htmlAttributes);
                        }

                        cellBuilder.AddCssClass(column.GetWidthClasses());
                        cellBuilder.AddCssClass("grid_cell");

                        var text = string.Empty;

                        if (column.CellText == null)
                        {
                            if (column.Property != null)
                            {
                                var value = column.Property.GetValue(row);
                                text = value != null ? value.ToString() : string.Empty;
                            }
                        }
                        else
                        {
                            var cellText = column.CellText(row);

                            text = cellText != null ? cellText.ToString() : string.Empty;
                        }
                        var title = string.Empty;
                        if (column.CellTitle != null)
                        {
                            title = (column.CellTitle(row) ?? string.Empty).ToString();
                            cellBuilder.MergeAttribute("title", title);
                        }

                        cellBuilder.InnerHtml += text;

                        if (column.IsEditable)
                        {
                            var editableContentContainerBuilder = new TagBuilder("span");

                            var editToggleBuilder = new TagBuilder("a");
                            var editGlyphiconBuilder = new TagBuilder("span");

                            editGlyphiconBuilder.AddCssClass("glyphicon glyphicon-pencil");

                            editToggleBuilder.AddCssClass("toggle_edit pull-right");
                            editToggleBuilder.MergeAttribute("style", "display:none;");
                            editToggleBuilder.MergeAttribute("href", "#");
                            editToggleBuilder.InnerHtml = editGlyphiconBuilder.ToString();

                            editableContentContainerBuilder.MergeAttribute("style", "display:none;");
                            editableContentContainerBuilder.AddCssClass("cell_editable_content");

                            editableContentContainerBuilder.InnerHtml = column.GetEditableContent();

                            cellBuilder.InnerHtml += editableContentContainerBuilder.ToString();
                            cellBuilder.InnerHtml += editToggleBuilder.ToString();
                        }

                        headerBuilder.InnerHtml += cellBuilder.ToString();
                    }

                    rowBuilder.InnerHtml += headerBuilder.ToString();

                    if (this.Builder.hasBulkActions)
                    {
                        var checkBuilder = new TagBuilder("input");
                        checkBuilder.MergeAttribute("type", "checkbox");
                        checkBuilder.MergeAttribute("class", "row_check bs-row_check");

                        if (this.Builder.rowCheckbox != null && !this.Builder.rowCheckbox(row))
                        {
                            checkBuilder.MergeAttribute("disabled", "disabled");
                        }

                        rowBuilder.InnerHtml += checkBuilder.ToString(TagRenderMode.SelfClosing);
                    }

                    if (rowHasDetails && ((hasDetailsProp != null && (bool)hasDetailsProp.GetValue(row)) || this.Builder.Model.BaseSettings.HasDetails(rowIndex)))
                    {
                        rowBuilder.InnerHtml += this.Builder.rowDetailsTemplate(row);
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
                divBuilder.MergeAttribute("class", "col-sm-12 col-lg-12");

                var infoBuilder = new TagBuilder("div");
                infoBuilder.MergeAttribute("class", "alert alert-info");


                var template = string.Empty;
                var stateClass = string.Empty;

                if (this.Builder.isAjaxRequest)
                {
                    template = this.Builder.noResultsTemplate;
                    stateClass = "noresults-state";
                }
                else
                {
                    template = this.Builder.noRecordsTemplate;
                    stateClass = "blank-state";
                }

                infoBuilder.AddCssClass(stateClass);

                infoBuilder.InnerHtml += !string.IsNullOrEmpty(template) ?
                                    this.Builder.viewContext.Controller.BsRenderPartialView(template, null) :
                                    BsResourceManager.Resource("BF_NoResults");

                if (this.Builder.allowAddIfEmpty)
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
    }
}
