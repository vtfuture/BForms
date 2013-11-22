﻿using BForms.Editor;
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
    public class BsEditorTabRenderer<TModel, TRow> : BsBaseRenderer<BsEditorTabBuilder<TModel>> where TModel : IBsEditorTabModel
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
            }
        }

        protected virtual string RenderItems()
        {
            var result = "";

            if (!string.IsNullOrEmpty(this.Builder.template))
            {
                var list = new TagBuilder("ul");

                list.AddCssClass("group_profiles");

                foreach (var item in this.Builder.Model.GetItems<TRow>())
                {
                    var listItem = new TagBuilder("li");

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

                    anchorRight.InnerHtml += GetGlyphicon(Glyphicon.Plus);

                    listItemWrapper.InnerHtml += anchorRight;

                    var templateWrapper = new TagBuilder("div");

                    templateWrapper.AddCssClass("media-body");

                    templateWrapper.InnerHtml += this.Builder.RenderModel<TRow>(item, "");

                    listItemWrapper.InnerHtml += templateWrapper;

                    listItem.InnerHtml += listItemWrapper;

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
            return this.Builder.PagerBuilder.ToString();
        }

        protected virtual string RenderContent()
        {
            var result = this.Builder.Toolbar.ToString();

            var wrapper = new TagBuilder("div");

            wrapper.AddCssClass("bs-tabContent");

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

            var result = "";

            if (this.Builder.HasItems)
            {
                result += RenderItems();
            }

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

            wrapper.MergeAttribute("data-loaded", this.Builder.HasModel.ToString());

            if (!this.Builder.selected)
            {
                wrapper.MergeAttribute("style", "display: none;");
            }

            wrapper.InnerHtml += this.RenderContent();

            return wrapper.ToString();
        }
    }
}
