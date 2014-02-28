using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using BForms.Grid;
using BForms.Models;
using BForms.Mvc;
using BForms.Utilities;

namespace BForms.Html
{
    /// <summary>
    /// Represents bootstrap support for form tag
    /// </summary>
    public static class FormExtensions
    {

        /// <summary>
        /// Renders a form suitable for BFroms fields
        /// </summary>
        public static BsMvcForm BsBeginForm(this HtmlHelper htmlHelper)
        {
            var routeValues = htmlHelper.ExtractRouteValues();
            return BsBeginForm(htmlHelper, routeValues["action"].ToString(), routeValues["controller"].ToString(), routeValues, FormMethod.Post, new RouteValueDictionary(), BsTheme.Default);
        }

        /// <summary>
        /// Renders a form suitable for BFroms fields
        /// </summary>
        public static BsMvcForm BsBeginForm(this HtmlHelper htmlHelper, BsTheme theme)
        {
            var routeValues = htmlHelper.ExtractRouteValues();
            return BsBeginForm(htmlHelper, routeValues["action"].ToString(), routeValues["controller"].ToString(), routeValues, FormMethod.Post, new RouteValueDictionary(), theme);
        }

        /// <summary>
        /// Renders a form suitable for BFroms fields
        /// </summary>
        public static BsMvcForm BsBeginForm(this HtmlHelper htmlHelper, BsTheme theme, object htmlAttributes)
        {
            var routeValues = htmlHelper.ExtractRouteValues();
            return BsBeginForm(htmlHelper, routeValues["action"].ToString(), routeValues["controller"].ToString(), routeValues, FormMethod.Post, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), theme);
        }

        /// <summary>
        /// Renders a form suitable for BFroms fields
        /// </summary>
        public static BsMvcForm BsBeginForm(this HtmlHelper htmlHelper, object routeValues)
        {
            return BsBeginForm(htmlHelper, null, null, new RouteValueDictionary(routeValues), FormMethod.Post, new RouteValueDictionary(), BsTheme.Default);
        }

        /// <summary>
        /// Renders a form suitable for BFroms fields
        /// </summary>
        public static BsMvcForm BsBeginForm(this HtmlHelper htmlHelper, object routeValues, BsTheme theme)
        {
            return BsBeginForm(htmlHelper, null, null, new RouteValueDictionary(routeValues), FormMethod.Post, new RouteValueDictionary(), theme);
        }

        /// <summary>
        /// Renders a form suitable for BFroms fields
        /// </summary>
        public static BsMvcForm BsBeginForm(this HtmlHelper htmlHelper, RouteValueDictionary routeValues)
        {
            return BsBeginForm(htmlHelper, null, null, routeValues, FormMethod.Post, new RouteValueDictionary(), BsTheme.Default);
        }

        /// <summary>
        /// Renders a form suitable for BFroms fields
        /// </summary>
        public static BsMvcForm BsBeginForm(this HtmlHelper htmlHelper, RouteValueDictionary routeValues, BsTheme theme)
        {
            return BsBeginForm(htmlHelper, null, null, routeValues, FormMethod.Post, new RouteValueDictionary(), theme);
        }

        /// <summary>
        /// Renders a form suitable for BFroms fields
        /// </summary>
        public static BsMvcForm BsBeginForm(this HtmlHelper htmlHelper, string actionName, string controllerName)
        {
            return BsBeginForm(htmlHelper, actionName, controllerName, new RouteValueDictionary(), FormMethod.Post, new RouteValueDictionary(), BsTheme.Default);
        }

        /// <summary>
        /// Renders a form suitable for BFroms fields
        /// </summary>
        public static BsMvcForm BsBeginForm(this HtmlHelper htmlHelper, string actionName, string controllerName, BsTheme theme)
        {
            return BsBeginForm(htmlHelper, actionName, controllerName, new RouteValueDictionary(), FormMethod.Post, new RouteValueDictionary(), theme);
        }

        /// <summary>
        /// Renders a form suitable for BFroms fields
        /// </summary>
        public static BsMvcForm BsBeginForm(this HtmlHelper htmlHelper, string actionName, string controllerName, object routeValues)
        {
            return BsBeginForm(htmlHelper, actionName, controllerName, new RouteValueDictionary(routeValues), FormMethod.Post, new RouteValueDictionary(), BsTheme.Default);
        }

        /// <summary>
        /// Renders a form suitable for BFroms fields
        /// </summary>
        public static BsMvcForm BsBeginForm(this HtmlHelper htmlHelper, string actionName, string controllerName, object routeValues, BsTheme theme)
        {
            return BsBeginForm(htmlHelper, actionName, controllerName, new RouteValueDictionary(routeValues), FormMethod.Post, new RouteValueDictionary(), theme);
        }

        /// <summary>
        /// Renders a form suitable for BFroms fields
        /// </summary>
        public static BsMvcForm BsBeginForm(this HtmlHelper htmlHelper, string actionName, string controllerName, RouteValueDictionary routeValues)
        {
            return BsBeginForm(htmlHelper, actionName, controllerName, routeValues, FormMethod.Post, new RouteValueDictionary(), BsTheme.Default);
        }

        /// <summary>
        /// Renders a form suitable for BFroms fields
        /// </summary>
        public static BsMvcForm BsBeginForm(this HtmlHelper htmlHelper, string actionName, string controllerName, RouteValueDictionary routeValues, BsTheme theme)
        {
            return BsBeginForm(htmlHelper, actionName, controllerName, routeValues, FormMethod.Post, new RouteValueDictionary(), theme);
        }

        /// <summary>
        /// Renders a form suitable for BFroms fields
        /// </summary>
        public static BsMvcForm BsBeginForm(this HtmlHelper htmlHelper, string actionName, string controllerName, FormMethod method)
        {
            return BsBeginForm(htmlHelper, actionName, controllerName, new RouteValueDictionary(), method, new RouteValueDictionary(), BsTheme.Default);
        }

        /// <summary>
        /// Renders a form suitable for BFroms fields
        /// </summary>
        public static BsMvcForm BsBeginForm(this HtmlHelper htmlHelper, string actionName, string controllerName, FormMethod method, BsTheme theme)
        {
            return BsBeginForm(htmlHelper, actionName, controllerName, new RouteValueDictionary(), method, new RouteValueDictionary(), theme);
        }

        /// <summary>
        /// Renders a form suitable for BFroms fields
        /// </summary>
        public static BsMvcForm BsBeginForm(this HtmlHelper htmlHelper, string actionName, string controllerName, object routeValues, FormMethod method)
        {
            return BsBeginForm(htmlHelper, actionName, controllerName, new RouteValueDictionary(routeValues), method, new RouteValueDictionary(), BsTheme.Default);
        }

        /// <summary>
        /// Renders a form suitable for BFroms fields
        /// </summary>
        public static BsMvcForm BsBeginForm(this HtmlHelper htmlHelper, string actionName, string controllerName, object routeValues, FormMethod method, BsTheme theme)
        {
            return BsBeginForm(htmlHelper, actionName, controllerName, new RouteValueDictionary(routeValues), method, new RouteValueDictionary(), theme);
        }

        /// <summary>
        /// Renders a form suitable for BFroms fields
        /// </summary>
        public static BsMvcForm BsBeginForm(this HtmlHelper htmlHelper, string actionName, string controllerName, RouteValueDictionary routeValues, FormMethod method)
        {
            return BsBeginForm(htmlHelper, actionName, controllerName, routeValues, method, new RouteValueDictionary(), BsTheme.Default);
        }

        /// <summary>
        /// Renders a form suitable for BFroms fields
        /// </summary>
        public static BsMvcForm BsBeginForm(this HtmlHelper htmlHelper, string actionName, string controllerName, RouteValueDictionary routeValues, FormMethod method, BsTheme theme)
        {
            return BsBeginForm(htmlHelper, actionName, controllerName, routeValues, method, new RouteValueDictionary(), theme);
        }

        /// <summary>
        /// Renders a form suitable for BFroms fields
        /// </summary>
        public static BsMvcForm BsBeginForm(this HtmlHelper htmlHelper, string actionName, string controllerName, FormMethod method, object htmlAttributes)
        {
            return BsBeginForm(htmlHelper, actionName, controllerName, new RouteValueDictionary(), method, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), BsTheme.Default);
        }

        /// <summary>
        /// Renders a form suitable for BFroms fields
        /// </summary>
        public static BsMvcForm BsBeginForm(this HtmlHelper htmlHelper, string actionName, string controllerName, FormMethod method, object htmlAttributes, BsTheme theme)
        {
            return BsBeginForm(htmlHelper, actionName, controllerName, new RouteValueDictionary(), method, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), theme);
        }

        /// <summary>
        /// Renders a form suitable for BFroms fields
        /// </summary>
        public static BsMvcForm BsBeginForm(this HtmlHelper htmlHelper, string actionName, string controllerName, FormMethod method, IDictionary<string, object> htmlAttributes)
        {
            return BsBeginForm(htmlHelper, actionName, controllerName, new RouteValueDictionary(), method, htmlAttributes, BsTheme.Default);
        }

        /// <summary>
        /// Renders a form suitable for BFroms fields
        /// </summary>
        public static BsMvcForm BsBeginForm(this HtmlHelper htmlHelper, string actionName, string controllerName, FormMethod method, IDictionary<string, object> htmlAttributes, BsTheme theme)
        {
            return BsBeginForm(htmlHelper, actionName, controllerName, new RouteValueDictionary(), method, htmlAttributes, theme);
        }

        /// <summary>
        /// Renders a form suitable for BFroms fields
        /// </summary>
        public static BsMvcForm BsBeginForm(this HtmlHelper htmlHelper, string actionName, string controllerName, object routeValues, FormMethod method, object htmlAttributes)
        {
            return BsBeginForm(htmlHelper, actionName, controllerName, new RouteValueDictionary(routeValues), method, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), BsTheme.Default);
        }

        /// <summary>
        /// Renders a form suitable for BFroms fields
        /// </summary>
        public static BsMvcForm BsBeginForm(this HtmlHelper htmlHelper, string actionName, string controllerName, object routeValues, FormMethod method, object htmlAttributes, BsTheme theme)
        {
            return BsBeginForm(htmlHelper, actionName, controllerName, new RouteValueDictionary(routeValues), method, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), theme);
        }

        /// <summary>
        /// Renders a form suitable for BFroms fields
        /// </summary>
        public static BsMvcForm BsBeginForm(this HtmlHelper htmlHelper, string actionName, string controllerName, RouteValueDictionary routeValues, FormMethod method, IDictionary<string, object> htmlAttributes, BsTheme theme)
        {
            var form = htmlHelper.BeginForm(actionName, controllerName, routeValues, method, htmlAttributes);

            var tag = new TagBuilder("div");
            tag.AddCssClass("row");
            tag.AddCssClass("form_container");
            tag.AddCssClass("loading");
            tag.AddCssClass(theme.GetDescription());

            var myForm = new BsMvcForm(htmlHelper.ViewContext, form, tag.ToString(TagRenderMode.StartTag), tag.ToString(TagRenderMode.EndTag));

            //htmlHelper.ViewContext.Writer.Write("<span>Test</span>");

            return myForm;
        }
    }

    public class BsMvcForm : IDisposable
    {
        private bool _disposed;
        private string _endTags;
        private readonly MvcForm _form;
        protected readonly ViewContext _viewContext;

        /// <summary>
        /// HTML Form wrapper
        /// </summary>
        /// <param name="startTags">html content injected after begin form</param>
        /// <param name="endTags">html content injected before end form</param>
        public BsMvcForm(ViewContext viewContext, MvcForm form, string startTags = null, string endTags = null)
        {
            _form = form;
            _endTags = endTags;
            _viewContext = viewContext;

            if (!string.IsNullOrEmpty(startTags))
            {
                _viewContext.Writer.Write(startTags);
            }
        }

        public void Dispose()
        {
            Dispose(true /* disposing */);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;

                if (!string.IsNullOrEmpty(_endTags))
                {
                    _viewContext.Writer.Write(_endTags);
                }

                _form.Dispose();
            }
        }
    }
}
