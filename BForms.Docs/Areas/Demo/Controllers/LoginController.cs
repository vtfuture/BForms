using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.ComponentModel.DataAnnotations;
using BootstrapForms.Models;
using BootstrapForms.Mvc;
using BootstrapForms.Utilities;
using BForms.Docs.Areas.Demo.Mock;
using BForms.Docs.Areas.Demo.Models;
using BForms.Docs.Areas.Demo.Helpers;
using BForms.Docs.Controllers;

namespace BForms.Docs.Areas.Demo.Controllers
{

    public class LoginController : BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            var model = new AuthenticationModel()
            {
                LoginModel = new LoginModel(),
                RegisterModel = InitRegisterModel()
            };

            RequireJsOptions.Add("registerUrl",Url.Action("Register"));

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(AuthenticationModel model)
        {
            //add global validation error
            ModelState.AddFormError("LoginModel",
                "<strong>This account has been suspended!</strong> <a href=\"#\" class=\"alert-link\">Contact us</a> for more details.");

            model.RegisterModel = InitRegisterModel();

            return View(model);
        }

        public BsJsonResult Register(AuthenticationModel model)
        {
            //keep errors only for RegisterModel
            ModelState.ClearModelState(model.GetPropertyName(m => m.RegisterModel) + ".");

            //add validation error to field
            ModelState.AddModelError("RegisterModel.Email", "This email address is in use");

            //add global validation error
            ModelState.AddFormError("RegisterModel", "This email address is in use.");

            if (ModelState.IsValid)
            {
                
            }
            else
            {
                return new BsJsonResult(new Dictionary<string, object> { { "Errors", ModelState.GetErrors() } }, BsResponseStatus.ValidationError);
            }

            return new BsJsonResult();
        }

        private RegisterModel InitRegisterModel()
        {
            return new RegisterModel()
                {
                    CountriesList = Lists.AllCounties<string>(),
                    NotificationList = BsSelectList<NotificationTypes?>.FromEnum(typeof(NotificationTypes)),
                    TechnologiesList = Lists.AllTech<List<int>>(),
                    TechnologiesCheckboxList = Lists.AllAsp<List<int>>(),
                    LanguagesList = Lists.AllLanguages<List<string>>(),
                    IdeList = Lists.AllIde<string>(),
                    GenderList = Lists.AllGenders<int>().ToSelectList().ToList()
                };
        }
    }
}