using BForms.Editor;
using BForms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BForms.Utilities;

namespace BForms.Html
{
    public static class GroupEditorExtensions
    {
        public static BsEditorHtmlBuilder<TModel> BsGroupEditorFor<TViewModel, TModel>(this HtmlHelper<TViewModel> htmlHelper,
            Expression<Func<TViewModel, TModel>> expression)
        {
            var model = expression.Compile().Invoke(htmlHelper.ViewData.Model);

            var baseBuilder = new BsEditorHtmlBuilder<TModel>(model, htmlHelper.ViewContext);

            return baseBuilder;
        }

        public static BsEditorHtmlBuilder<TModel> BsGroupEditorFor<TModel>(this HtmlHelper<TModel> htmlHelper,
            TModel model)
        {
            var baseBuilder = new BsEditorHtmlBuilder<TModel>(model, htmlHelper.ViewContext);

            return baseBuilder;
        }

        public static BsEditorHtmlBuilder<TModel> BsGroupEditorFor<TModel>(this HtmlHelper<TModel> htmlHelper,
            TModel model, BsEditorHtmlBuilder<TModel> builder)
        {
            return builder;
        }
    }
}
