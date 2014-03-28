using BForms.Grid;
using BForms.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BForms.Renderers
{
    public class BsPagerBaseRenderer : BsBaseRenderer<BsGridPagerBuilder>
    {
        public BsPagerBaseRenderer() { }

        public BsPagerBaseRenderer(BsGridPagerBuilder builder)
            : base(builder)
        {
        }

        public string RenderPages()
        {
            if (this.Builder.pager != null)
            {
                var pagesBuilder = new TagBuilder("div");
                pagesBuilder.AddCssClass("col-md-9 col-lg-9 bs-pages");

                #region pagination

                var paginationBuilder = new TagBuilder("ul");
                paginationBuilder.AddCssClass("pagination pagination-md");

                if (this.Builder.pager.TotalPages <= 1)
                {
                    paginationBuilder.MergeAttribute("style", "display:none");
                }

                #region first page button

                if (this.Builder.settings.ShowFirstLastButtons)
                {
                    var firstPageBuilder = new TagBuilder("li");
                    if (this.Builder.pager.CurrentPage == 1)
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

                if (this.Builder.settings.ShowPrevNextButtons)
                {
                    var prevPageBuilder = new TagBuilder("li");
                    if (this.Builder.pager.CurrentPage == 1)
                    {
                        prevPageBuilder.MergeAttribute("class", "disabled");
                    }
                    var anchorBuilder = new TagBuilder("a");
                    anchorBuilder.MergeAttribute("href", "#");
                    anchorBuilder.MergeAttribute("data-page", (this.Builder.pager.CurrentPage - 1).ToString());
                    anchorBuilder.InnerHtml += "&lsaquo;";

                    prevPageBuilder.InnerHtml += anchorBuilder.ToString();

                    paginationBuilder.InnerHtml += prevPageBuilder.ToString();
                }

                #endregion

                #region pages buttons

                var startPage = this.Builder.pager.GetStartPage(this.Builder.settings.Size);
                int nr = this.Builder.settings.Size > this.Builder.pager.TotalPages ? this.Builder.pager.TotalPages % this.Builder.settings.Size : this.Builder.settings.Size;
                for (int i = 0; i < nr; i++)
                {
                    var page = i + startPage;
                    var pageBtnBuilder = new TagBuilder("li");
                    if (this.Builder.pager.CurrentPage == page)
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

                if (this.Builder.settings.ShowPrevNextButtons)
                {
                    var nextPageBuilder = new TagBuilder("li");
                    if (this.Builder.pager.CurrentPage == this.Builder.pager.TotalPages)
                    {
                        nextPageBuilder.MergeAttribute("class", "disabled");
                    }
                    var anchorBuilder = new TagBuilder("a");
                    anchorBuilder.MergeAttribute("href", "#");
                    anchorBuilder.MergeAttribute("data-page", (this.Builder.pager.CurrentPage + 1).ToString());
                    anchorBuilder.InnerHtml += "&rsaquo;";

                    nextPageBuilder.InnerHtml += anchorBuilder.ToString();

                    paginationBuilder.InnerHtml += nextPageBuilder.ToString();
                }

                #endregion

                #region last page button

                if (this.Builder.settings.ShowFirstLastButtons)
                {
                    var lastPageBuilder = new TagBuilder("li");
                    if (this.Builder.pager.CurrentPage == this.Builder.pager.TotalPages)
                    {
                        lastPageBuilder.MergeAttribute("class", "disabled");
                    }
                    var anchorBuilder = new TagBuilder("a");
                    anchorBuilder.MergeAttribute("href", "#");
                    anchorBuilder.MergeAttribute("data-page", this.Builder.pager.TotalPages.ToString());
                    anchorBuilder.InnerHtml += "&raquo;";

                    lastPageBuilder.InnerHtml += anchorBuilder.ToString();

                    paginationBuilder.InnerHtml += lastPageBuilder.ToString();
                }

                #endregion

                pagesBuilder.InnerHtml += paginationBuilder.ToString();

                #endregion

                #region text

                if (this.Builder.settings.HasPagesText)
                {
                    var textBuilder = new TagBuilder("div");
                    textBuilder.MergeAttribute("class", "results_number");

                    var firstIdx = (this.Builder.pager.CurrentPage - 1) * this.Builder.pager.PageSize + 1;
                    var lastIdx = this.Builder.pager.CurrentPage == this.Builder.pager.TotalPages
                                      ? this.Builder.pager.TotalRecords
                                      : this.Builder.pager.CurrentPage * this.Builder.pager.PageSize;

                    var lastBuilder = new TagBuilder("span");
                    lastBuilder.AddCssClass("bs-topResultsMargin");
                    lastBuilder.InnerHtml += lastIdx;

                    var totalCountBuilder = new TagBuilder("span");
                    totalCountBuilder.InnerHtml += this.Builder.pager.TotalRecords;

                    //TODO:
                    var template = "{0}-{1} " + BsResourceManager.Resource("Of") + " {2} " + BsResourceManager.Resource("Items");
                    var result = string.Format(template, firstIdx, lastBuilder, totalCountBuilder.ToString()); //"Rezultate " + firstIdx + "–" + lastIdx + " din";

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

        public override string Render()
        {
            var pagerWrapper = new TagBuilder("div");

            if (this.Builder.pager == null || this.Builder.pager.TotalRecords == 0)
            {
                pagerWrapper.MergeAttribute("style", "display: none;");
            }

            pagerWrapper.AddCssClass("row bs-pager");
            pagerWrapper.AddCssClass(this.Builder.Theme.GetDescription());

            pagerWrapper.InnerHtml += this.RenderPages();

            if (this.Builder.settings.HasPageSizeSelector)
            {
                int pageSize = this.Builder.pager != null ? this.Builder.pager.PageSize : this.Builder.baseSettings.PageSize;
                if (!this.Builder.settings.PageSizeValues.Contains(pageSize))
                    throw new ArgumentOutOfRangeException("The page size you selected is not in the list");

                var selectWrapperBuilder = new TagBuilder("div");

                selectWrapperBuilder.AddCssClass("col-md-3 col-lg-3 results_per_page");

                if (this.Builder.hidePageSize)
                {
                    selectWrapperBuilder.AddCssClass("hidden-md hidden-sm hidden-xs");
                }

                TagBuilder divBuilder = new TagBuilder("div");
                divBuilder.AddCssClass("pull-right");

                #region right side
                var dropdownContainerBuilder = new TagBuilder("div");
                dropdownContainerBuilder.AddCssClass("dropdown dropup");

                var dropdownTriggerBuilder = new TagBuilder("a");
                dropdownTriggerBuilder.MergeAttribute("data-toggle", "dropdown");
                dropdownTriggerBuilder.MergeAttribute("href", "#");



                var dropdownListBuilder = new TagBuilder("ul");
                dropdownListBuilder.MergeAttribute("class", "dropdown-menu");
                dropdownListBuilder.MergeAttribute("role", "menu");

                foreach (var item in this.Builder.settings.PageSizeValues)
                {
                    var dropdownLiBuilder = new TagBuilder("li");
                    var dropdownLiAnchorBuilder = new TagBuilder("a");

                    if (pageSize == item)
                    {
                        var dropdownCountBuilder = new TagBuilder("span");
                        dropdownCountBuilder.AddCssClass("btn btn-white bs-perPageDisplay");
                        var caret = new TagBuilder("span");
                        caret.AddCssClass("caret");
                        dropdownCountBuilder.InnerHtml += item.ToString() + caret.ToString();
                        dropdownTriggerBuilder.InnerHtml += dropdownCountBuilder.ToString();

                        dropdownLiAnchorBuilder.AddCssClass("selected");
                    }

                    dropdownLiAnchorBuilder.InnerHtml += item;
                    dropdownLiAnchorBuilder.MergeAttribute("data-value", item.ToString());
                    dropdownLiAnchorBuilder.AddCssClass("bs-perPage");
                    dropdownLiAnchorBuilder.Attributes.Add("href", "#");

                    dropdownLiBuilder.InnerHtml += dropdownLiAnchorBuilder.ToString();
                    dropdownListBuilder.InnerHtml += dropdownLiBuilder.ToString();
                }

                dropdownContainerBuilder.InnerHtml += dropdownTriggerBuilder.ToString();
                dropdownContainerBuilder.InnerHtml += dropdownListBuilder.ToString();

                divBuilder.InnerHtml += dropdownContainerBuilder.ToString();

                var goTopBuilder = new TagBuilder("button");
                goTopBuilder.AddCssClass("btn btn-white btn-go_up bs-goTop");
                goTopBuilder.MergeAttribute("title", "Go top");
                var goTopSpanBuilder = new TagBuilder("span");
                goTopSpanBuilder.AddCssClass("glyphicon glyphicon-arrow-up");

                goTopBuilder.InnerHtml += goTopSpanBuilder.ToString();

                divBuilder.InnerHtml += goTopBuilder.ToString();
                #endregion

                selectWrapperBuilder.InnerHtml += divBuilder.ToString();

                pagerWrapper.InnerHtml += selectWrapperBuilder.ToString();
            }

            return pagerWrapper.ToString();
        }
    }
}
