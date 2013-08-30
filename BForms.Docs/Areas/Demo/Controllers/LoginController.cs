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
        public ActionResult Index(string mode)
        {
            var model = new AuthenticationModel()
            {
                LoginModel = new LoginModel(),
                RegisterModel = InitRegisterModel()
            };

            if(!string.IsNullOrEmpty(mode))
            if(mode.ToLower() == "login")
            {
                model.RegisterModel = null;
            }
            else if (mode.ToLower() == "register")
            {
                model.LoginModel = null;
            }

            RequireJsOptions.Add("registerUrl",Url.Action("Register"));

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(AuthenticationModel model, string mode)
        {
            //add global validation error
            ModelState.AddFormError("LoginModel",
                "<strong>This account has been suspended!</strong> <a href=\"#\" class=\"alert-link\">Contact us</a> for more details.");

            return View(model);
        }

        public BsJsonResult Register(AuthenticationModel model)
        {
            //keep errors only for RegisterModel
            ModelState.ClearModelState(model.GetPropertyName(m => m.RegisterModel) + ".");

            //add validation error to field
            if (model.RegisterModel != null && 
                model.RegisterModel.Interval != null &&
                model.RegisterModel.Interval.To.HasValue && 
                model.RegisterModel.Interval.From.HasValue &&
                model.RegisterModel.Interval.From.Value > model.RegisterModel.Interval.To.Value)
            {
                ModelState.AddModelError("RegisterModel.Interval", "Invalid interval");
            }
            

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
            var listWithSelected = Lists.AllAsp<List<int>>();
            listWithSelected.SelectedValues = new List<int> {1, 2};

            var enumWithSelected = BsSelectList<NotificationTypes?>.FromEnum(typeof(NotificationTypes));
            enumWithSelected.SelectedValues = NotificationTypes.Monthly;

            var ddlWithSelected = Lists.AllCounties<string>();
            ddlWithSelected.SelectedValues = "ROU";

            var enumListWithSelected = BsSelectList<List<NotificationTypes?>>.FromEnum(typeof(NotificationTypes));
            enumListWithSelected.SelectedValues = new List<NotificationTypes?>() {NotificationTypes.Monthly, NotificationTypes.Daily};

            return new RegisterModel()
                {
                    CountriesList = ddlWithSelected,
                    NotificationList = enumWithSelected,
                    TechnologiesList = Lists.AllTech<List<int>>(),
                    TechnologiesCheckboxList = listWithSelected,
                    LanguagesList = Lists.AllLanguages<List<string>>(),
                    IdeList = Lists.AllIde<string>(),
                    GenderList = Lists.AllGenders<int>().ToSelectList().ToList(),
                    Interval = new BsRange<DateTime?> { From = DateTime.Now.AddDays(-1), To = DateTime.Now }
                };
        }
    }
}