using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using BForms.Grid;
using BForms.Models;

namespace BForms.Html
{
    public static class BsPagerExtensions
    {
        public static BsGridPagerBuilder BsPager(this HtmlHelper html, BsPagerModel model)
        {
            var builder = new BsGridPagerBuilder(model, null, null) { viewContext = html.ViewContext };

            return builder;
        }

        public static BsGridPagerBuilder BsPagerFor<TModel>(this HtmlHelper<TModel> html, Expression<Func<TModel, BsPagerModel>> expression)
        {
            var model = expression.Compile().Invoke(html.ViewData.Model);
            return BsPager(html, model);
        }
    }
}
