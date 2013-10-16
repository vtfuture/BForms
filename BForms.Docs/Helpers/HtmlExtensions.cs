using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BForms.Docs.Controllers;
using BForms.Grid;
using BForms.Html;

namespace BForms.Docs.Helpers
{
    public static class HtmlExtensions
    {
        public static BsTheme GetTheme(this HtmlHelper html)
        {
            var theme = (html.ViewContext.Controller as BaseController).GetTheme;
            return theme.Theme;
        }
    }
}