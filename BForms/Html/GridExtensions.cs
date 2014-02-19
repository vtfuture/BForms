using BForms.Grid;
using BForms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using BForms.Mvc;
using BForms.Utilities;


namespace BForms.Html
{
    public static class GridExtensions
    {
        public static BsGridHtmlBuilder<TModel, TRow> BsGridFor<TModel, TRow>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, BsGridModel<TRow>>> expression) where TRow : BsItemModel,new()
        {
            return htmlHelper.BsGridFor(expression, null);
        }

        public static BsGridHtmlBuilder<TModel, TRow> BsGridFor<TModel, TRow>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, BsGridModel<TRow>>> expression,
            BsGridHtmlBuilder<TModel, TRow> baseBuilder) where TRow : BsItemModel, new()
        {
            var grid = expression.Compile().Invoke(htmlHelper.ViewData.Model);

            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

            var fullName = ExpressionHelper.GetExpressionText(expression);

            var attributes = expression.GetPropertyInfo().GetCustomAttributes(true);

            if (baseBuilder == null)
            {
                baseBuilder = new BsGridHtmlBuilder<TModel, TRow>(fullName, grid, metadata, htmlHelper.ViewContext);
            }
            else
            {
                baseBuilder.Model = grid;
                baseBuilder.FullName = fullName;
                baseBuilder.Metadata = metadata;
                baseBuilder.viewContext = htmlHelper.ViewContext;                
            }

            baseBuilder.SetPropsFromAttributes(attributes);

            return baseBuilder;
        }

        public static BsHtmlTag BsGridWrapper<TModel>(this HtmlHelper<TModel> htmlHelper)
        {
            var container = new TagBuilder("div");
            container.AddCssClass("grid_container");

            htmlHelper.ViewContext.Writer.Write(container.ToString(TagRenderMode.StartTag));
            BsHtmlTag htmlTag = new BsHtmlTag(htmlHelper.ViewContext, "div");

            return htmlTag;
        }
    }
}