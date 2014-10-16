using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using BForms.Grid;
using BForms.Models;

namespace BForms.Html
{
    public static class BsPagerExtensions
    {
        public static BsGridPagerBuilder BsPager(this HtmlHelper html, BsPagerModel model, BsPagerSettings pagerSettings)
        {
            var builder = new BsGridPagerBuilder(model, pagerSettings, null) { viewContext = html.ViewContext };

            return builder;
        }

        public static BsGridPagerBuilder BsPager(this HtmlHelper html, BsPagerModel model)
        {
            var builder = BsPager(html, model, null);

            return builder;
        }

        public static BsGridPagerBuilder BsPagerFor<TModel>(this HtmlHelper<TModel> html, Expression<Func<TModel, BsPagerModel>> expression)
        {
            var model = expression.Compile().Invoke(html.ViewData.Model);
            return BsPager(html, model);
        }
    }
}
