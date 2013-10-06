using BootstrapForms.Grid;
using BootstrapForms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace BootstrapForms.Html
{
    public static class GridExtensions
    {
        public static BsGridHtmlBuilder<TModel, TRow> BsGridFor<TModel, TRow>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, BsGridModel<TRow>>> expression) where TRow : new()
        {
            var grid = expression.Compile().Invoke(htmlHelper.ViewData.Model);

            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

            var fullName = ExpressionHelper.GetExpressionText(expression);

            return new BsGridHtmlBuilder<TModel, TRow>(fullName, grid, metadata, htmlHelper.ViewContext);
        }

        public static BsHtmlTag BsGridWrapper<TModel>(this HtmlHelper<TModel> htmlHelper)
        {
            var container = new TagBuilder("div");
            container.MergeAttribute("class", "grids_container");

            htmlHelper.ViewContext.Writer.Write(container.ToString(TagRenderMode.StartTag));
            BsHtmlTag htmlTag = new BsHtmlTag(htmlHelper.ViewContext, "div");

            return htmlTag;
        }
    }
}