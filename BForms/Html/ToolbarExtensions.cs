using BForms.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using BForms.Utilities;

namespace BForms.Html
{
    public static class ToolbarExtensions
    {
        public static BsToolbarHtmlBuilder<TToolbar> BsToolbarFor<TModel, TToolbar>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TToolbar>> expression) where TToolbar : new()
        {
            var toolbar = expression.Compile().Invoke(htmlHelper.ViewData.Model);

            if (toolbar == null)
            {
                toolbar = new TToolbar();
                foreach (var prop in toolbar.GetType().GetProperties())
                {
                    prop.SetValue(toolbar, Activator.CreateInstance(prop.PropertyType));
                }
                var model = htmlHelper.ViewData.Model;
                var toolbarProp = expression.GetPropertyInfo();
                toolbarProp.SetValue(model, toolbar);
            }

            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

            var fullName = ExpressionHelper.GetExpressionText(expression);

            var attributes = expression.GetPropertyInfo().GetCustomAttributes(true);
           
            return new BsToolbarHtmlBuilder<TToolbar>(fullName, toolbar, metadata, attributes, htmlHelper.ViewContext);
        }
    }
}