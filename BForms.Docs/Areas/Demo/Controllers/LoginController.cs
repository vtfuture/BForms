using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.ComponentModel.DataAnnotations;
using BForms.Docs.Areas.Demo.Mock;
using BootstrapForms.Models;
using BootstrapForms.Utilities;
using BForms.Docs.Areas.Demo.Models;
using BForms.Docs.Areas.Demo.Helpers;
using BForms.Docs.Controllers;

namespace BForms.Docs.Areas.Demo.Controllers
{

    public class LoginController : BaseController
    {
        //
        // GET: /Demo/Login/
        public ActionResult Index()
        {
            var model = new AuthenticationModel()
            {
                LoginModel = new LoginModel(),
                RegisterModel = new RegisterModel()
                {
                    CountriesList = Lists.AllCounties<string>(),
                    NotificationList = BsSelectList<NotificationTypes?>.FromEnum(typeof(NotificationTypes)),
                    TechnologiesList = Lists.AllTech<List<int>>(),
                    TagList = Lists.AllLanguages<List<string>>(),
                    AutocompleteList = Lists.AllCounties<string>(),
                    GenderList = Lists.AllGenders<int>().ToSelectList().ToList()
                }
            };

            RequireJsOptions.Add("registerUrl",Url.Action("Register"));

            return View(model);
        }

        public BsJsonResult Register(AuthenticationModel model)
        {
            ModelState.ClearModelState(model.GetPropertyName(m => m.RegisterModel) + ".");
            ModelState.AddModelError("RegisterModel.EnableNotifications", "This email address is in use");
            if (ModelState.IsValid)
            {
                
            }
            else
            {
                return new BsJsonResult(new Dictionary<string, object> { { "Errors", ModelState.GetErrors() } }, BsResponseStatus.ValidationError);
            }

            return new BsJsonResult();
        }
    }
}