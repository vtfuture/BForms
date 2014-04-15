using BForms.Editor;
using BForms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BForms.Mvc;
using BForms.Grid;
using BForms.Utilities;

namespace BForms.Renderers
{
    public class BsEditorTabRenderer<TModel, TRow> : BsBaseRenderer<BsEditorTabBuilder<TModel>> 
        where TModel : IBsEditorTabModel
        where TRow : BsItemModel
    {
        protected BsEditorRowConfigurator<TRow> rowConfigurator;

        public BsEditorTabRenderer(){}

        public BsEditorTabRenderer(BsEditorTabBuilder<TModel> builder)
            : base(builder)
        {
            this.rowConfigurator = (BsEditorRowConfigurator<TRow>)builder.rowConfigurator;
        }

        protected void InitPager()
        {
            if (this.Builder.Model != null)
            {
                this.Builder.PagerBuilder = new BsGridPagerBuilder(
                this.Builder.Model.GetGrid<TRow>().Pager,
                this.Builder.pagerSettings,
                this.Builder.Model.GetGrid<TRow>().BaseSettings);

                this.Builder.PagerBuilder.hidePageSize = true;
            }
        }

        protected virtual string RenderItems()
        {
            var result = "";

            if (!string.IsNullOrEmpty(this.Builder.template))
            {
                var list = new TagBuilder("ul");

                list.AddCssClass("group_profiles");
                list.AddCssClass("bs-tabItemsList");

                if (this.Builder.HasItems)
                {
                    foreach (var item in this.Builder.Model.GetItems<TRow>())
                    {
                        var listItem = new TagBuilder("li");

                        listItem.MergeAttribute("data-objid", MvcHelpers.Serialize(item.GetUniqueID()));

                        listItem.MergeAttribute("data-model", MvcHelpers.Serialize(item));

                        IDictionary<string, object> itemAttributes = null;

                        if (this.rowConfigurator.htmlExpression != null)
                        {
                            itemAttributes = this.rowConfigurator.htmlExpression.Compile().Invoke(item);
                        }

                        listItem.MergeAttributes(itemAttributes);

                        listItem.MergeClassAttribute("bs-tabItem", itemAttributes);

                        var listItemWrapper = new TagBuilder("div");

                        listItemWrapper.AddCssClass("media profile large");

                        if (this.rowConfigurator.avatarExpression != null)
                        {
                            var anchorLeft = new TagBuilder("a");

                            anchorLeft.MergeAttribute("href", "#");

                            anchorLeft.AddCssClass("pull-left");

                            var img = new TagBuilder("img");

                            img.AddCssClass("media-object");

                            string avatar = this.rowConfigurator.avatarExpression.Compile().Invoke(item);

                            img.MergeAttribute("src", avatar);

                            anchorLeft.InnerHtml += img;

                            listItemWrapper.InnerHtml += anchorLeft;
                        }

                        var anchorRight = new TagBuilder("a");

                        anchorRight.MergeAttribute("href", "#");

                        anchorRight.AddCssClass("btn btn-white select_profile");

                        var isSelected = IsSelected(item);

                        if (this.Builder.IsReadonly)
                        {
                            anchorRight.AddCssClass("disabled");
                            listItem.AddCssClass("bs-notDraggable");
                        }

                        if (isSelected)
                        {
                            anchorRight.InnerHtml += GetGlyphicon(Glyphicon.Ok);

                            listItem.AddCssClass("selected");
                        }
                        else
                        {
                            anchorRight.AddCssClass("bs-addBtn");

                            anchorRight.InnerHtml += GetGlyphicon(Glyphicon.Plus);
                        }

                        listItemWrapper.InnerHtml += anchorRight;

                        var templateWrapper = new TagBuilder("div");

                        templateWrapper.AddCssClass("media-body");

                        templateWrapper.InnerHtml += this.Builder.RenderModel<TRow>(item, "");

                        listItemWrapper.InnerHtml += templateWrapper;

                        listItem.InnerHtml += listItemWrapper;

                        list.InnerHtml += listItem;
                    }
                }
                else
                {
                    var listItem = new TagBuilder("li");
                    listItem.AddCssClass("bs-noResultsTabItem");

                    var infoBuilder = new TagBuilder("div");
                    infoBuilder.MergeAttribute("class", "alert alert-info");

                    infoBuilder.InnerHtml += !string.IsNullOrEmpty(this.Builder.noResultsTemplate) ?
                                   this.Builder.viewContext.Controller.BsRenderPartialView(this.Builder.noResultsTemplate, null) :
                                   BsResourceManager.Resource("NoResults");

                    listItem.InnerHtml += infoBuilder.ToString();

                    list.InnerHtml += listItem;
                }

                result += list;
            }
            else
            {
                throw new Exception("You must set the template for tab " + this.Builder.Uid.ToString());
            }

            return result;
        }

        protected virtual string RenderPager()
        {
            this.Builder.PagerBuilder.Theme = this.Builder.Theme;

            return this.Builder.PagerBuilder.ToString();
        }

        protected virtual string RenderContent()
        {
            var result = this.Builder.Toolbar.ToString();

            result += this.Builder.bulkMoveHtml;

            var wrapper = new TagBuilder("div");

            wrapper.AddCssClass("bs-tabContent");
            wrapper.MergeAttribute("style","clear:both");

            if (this.Builder.HasModel)
            {
                wrapper.InnerHtml += RenderAjax();
            }

            result += wrapper;

            return result;
        }

        public override string RenderAjax()
        {
            this.InitPager();

            var result = RenderItems();

            result += RenderPager();

            return result;
        }

        public override string Render()
        {
            this.InitPager();

            var wrapper = new TagBuilder("div");
            
            wrapper.MergeAttribute("data-tabid", this.Builder.Uid.ToString());

            if (this.Builder.ConnectsWithIds != null)
            {
                wrapper.MergeAttribute("data-connectswith", MvcHelpers.Serialize(this.Builder.ConnectsWithIds));
            }

            wrapper.MergeAttribute("data-editable", MvcHelpers.Serialize(this.Builder.IsEditable));

            wrapper.MergeAttribute("data-readonly", MvcHelpers.Serialize(this.Builder.IsReadonly));

            wrapper.MergeAttribute("data-loaded", MvcHelpers.Serialize(this.Builder.HasModel));

            if (!this.Builder.selected)
            {
                wrapper.MergeAttribute("style", "display: none;");
            }

            wrapper.InnerHtml += this.RenderContent();

            return wrapper.ToString();
        }

        protected bool IsSelected(TRow item)
        {
            var connectsWith = this.Builder.ConnectsWithIds;
            var isSelected = true;

            if (connectsWith.Any())
            {
                foreach (var groupId in connectsWith)
                {
                    if (!this.Builder.Connections.Any(x =>
                        x.GroupId.Equals(groupId) &&
                        x.Items.Any(y =>
                            y.TabId.Equals(this.Builder.Uid) &&
                            y.Id.Equals(item.GetUniqueID()))))
                    {
                        isSelected = false;
                    }
                }
            }
            else
            {
                isSelected = false;
            }
            
            return isSelected;
        }
    }
}
