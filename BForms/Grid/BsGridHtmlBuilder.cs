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
using BForms.Renderers;

namespace BForms.Grid
{
    public class BsGridHtmlBuilder<TModel, TRow> : BsBaseComponent<BsGridHtmlBuilder<TModel, TRow>> where TRow : new()
    {
        private BsGridModel<TRow> model;

        internal BsGridModel<TRow> Model
        {
            get
            {
                return this.model;
            }

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

        internal BsGridPagerBuilder pagerBuilder;
        internal bool renderTitle = true;
        internal string resetButtonHtml;
        internal Func<TRow, string> rowHighlighter;
        internal Func<TRow, IDictionary<string, object>> rowData;
        internal Func<TRow, bool> rowDetails;
        internal Func<TRow, bool> rowCheckbox;
        internal Func<TRow, string> rowDetailsTemplate;
        internal bool hasDetails;
        internal bool hasBulkActions;
        internal bool allowAddIfEmpty;
        internal List<BsGridColumn<TRow>> columns;
        internal bool showColumnsHeader = true;
        internal List<BsBulkAction> bulkActions;
        internal List<BsBulkSelector> bulkSelectors;
        internal bool renderPager = true;
        internal BsPagerSettings pagerSettings = new BsPagerSettings();
        internal BsTheme theme = BsTheme.Default;
        //private BsBulkActionsFactory BulkActionsFactory { get; set; }
        internal string noRecordsTemplate;
        internal string noResultsTemplate;
        internal bool isAjaxRequest = false;

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
            this.renderer = new BsGridBaseRenderer<TModel, TRow>(this);

            this.SetColumnsFromModel();
        }

        public BsGridHtmlBuilder(string fullName, BsGridModel<TRow> model, ModelMetadata metadata, ViewContext viewContext)
            : base(viewContext)
        {
            this.renderer = new BsGridBaseRenderer<TModel, TRow>(this);

            this.fullName = fullName;
            this.model = model;
            this.metadata = metadata;

            this.SetColumnsFromModel();
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

        public BsGridHtmlBuilder<TModel, TRow> GridResetButton(string title)
        {
            var resetButton = new TagBuilder("div");
            resetButton.MergeAttribute("class", "btn btn-info bs-resetGrid reset-grid");
            resetButton.MergeAttribute("style", "display:none");
            resetButton.MergeAttribute("title", title);

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

        internal void SetColumnsFromModel()
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

        internal void OrderColumns()
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

        internal bool HideDetails()
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