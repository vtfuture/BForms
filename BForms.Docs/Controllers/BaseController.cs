using BForms.Docs.Areas.Demo.Mock;
using BForms.Docs.Models;
using BForms.Grid;
using BForms.Models;
using RequireJS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BForms.Docs.Controllers
{
    public class BaseController : RequireJS.RequireJsController
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
                        Theme = BsTheme.Default
                    };

                    Session["ThemeSettings"] = newThemeSettings;

                    return newThemeSettings;
                }

                return theme;
            }
        }

        public override void RegisterGlobalOptions()
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
        }
    }
}