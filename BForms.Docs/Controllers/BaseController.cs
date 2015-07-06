using BForms.Docs.Areas.Demo.Mock;
using BForms.Docs.Models;
using BForms.Grid;
using BForms.Models;
using RequireJsNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BForms.Docs.Controllers
{
    [OutputCache(Duration = 0, NoStore = true)]
    public class BaseController : Controller
    {
        public BFormsContext Db 
        {
            get
            {
                return BFormsContext.Get();
            }
        }

        public ThemeSettings GetTheme
        {
            get
            {
                var theme = Session["ThemeSettings"] as ThemeSettings;
                
                if (theme == null)
                {
                    var newThemeSettings = new ThemeSettings()
                    {
                        Open = false,
                        Theme = BForms.Utilities.BsUIManager.GetGlobalTheme()
                    };

                    Session["ThemeSettings"] = newThemeSettings;

                    return newThemeSettings;
                }

                return theme;
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            RequireJsOptions.Add(
                "homeUrl",
                Url.Action("Index", "Home", new { area = "" }),
                RequireJsOptionsScope.Global);

            RequireJsOptions.Add(
                "saveThemeUrl",
                Url.Action("SaveTheme", "Home", new { area = "" }),
                RequireJsOptionsScope.Global);

            RequireJsOptions.Add(
               "startTheme",
               Enum.GetName(typeof(BsTheme), GetTheme.Theme),
               RequireJsOptionsScope.Global);

            RequireJsOptions.Add(
              "themeEnum",
              RequireJsHtmlHelpers.ToJsonDictionary<BsTheme>(),
              RequireJsOptionsScope.Global);

            //RequireJsOptions.Add("loggerUrl", Url.Action("LogException", "Error", new { area = string.Empty }), RequireJsOptionsScope.Global);
        }
    }
}