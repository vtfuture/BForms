using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.ComponentModel.DataAnnotations;
using BForms.Docs.Helpers;
using BForms.Models;
using BForms.Mvc;
using BForms.Docs.Areas.Demo.Mock;
using BForms.Docs.Areas.Demo.Models;
using BForms.Docs.Areas.Demo.Helpers;
using BForms.Docs.Controllers;

using RequireJsNet;

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

            if (!string.IsNullOrEmpty(mode))
                if (mode.ToLower() == "login")
                {
                    model.RegisterModel = null;
                }
                else if (mode.ToLower() == "register")
                {
                    model.LoginModel = null;
                }

            RequireJsOptions.Add("registerUrl", Url.Action("Register"));

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

            //add validation error to BsRange field
            if (model.RegisterModel != null &&
                model.RegisterModel.Interval != null &&
                //model.RegisterModel.Interval.To.ItemValue.HasValue &&
                //model.RegisterModel.Interval.From.ItemValue.HasValue &&
                model.RegisterModel.Interval.From.ItemValue > model.RegisterModel.Interval.To.ItemValue)
            {
                ModelState.AddFieldError("RegisterModel.Interval",
                    model.RegisterModel.Interval.GetType(),
                    "Invalid interval");
            }

            //if (model.RegisterModel != null &&
            //   model.RegisterModel.Interval != null &&
            //   model.RegisterModel.Interval.To.ItemValue.HasValue &&
            //   model.RegisterModel.Interval.From.ItemValue.HasValue &&
            //   model.RegisterModel.Interval.From.ItemValue.Value > model.RegisterModel.Interval.To.ItemValue.Value)
            //{
            //    ModelState.AddFieldError("RegisterModel.Interval",
            //        model.RegisterModel.Interval.GetType(),
            //        "Invalid interval");
            //}

            //add validation error to BsSelectList field
            ModelState.AddFieldError("RegisterModel.CountriesList",
                model.RegisterModel.CountriesList.GetType(),
                "Selected location doesn't match your GPS location");

            //add global validation error
            ModelState.AddFormError("RegisterModel",
                "This email address is in use.");

            if (ModelState.IsValid)
            {

            }
            else
            {
                //JSON serialize ModelState errors
                return new BsJsonResult(
                    new Dictionary<string, object> { { "Errors", ModelState.GetErrors() } },
                    BsResponseStatus.ValidationError);
            }

            return new BsJsonResult();
        }

        private RegisterModel InitRegisterModel()
        {
            var listWithSelected = Lists.AllAsp<List<int>>();
            listWithSelected.SelectedValues = new List<int> { 1, 2 };

            var enumWithSelected = BsSelectList<NotificationType?>.FromEnum(typeof(NotificationType));
            enumWithSelected.SelectedValues = NotificationType.Monthly;

            var ddlWithSelected = Lists.AllCounties<string>();
            ddlWithSelected.SelectedValues = "ROU";

            var ideListWithSelected = Lists.AllIde<string>();
            ideListWithSelected.SelectedValues = "Visual Studio";

            var langListWithSelected = Lists.AllLanguages<List<string>>();
            langListWithSelected.SelectedValues = new List<string> { "C#" };

            var from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 10, 30, 0);
            var to = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(12).Day, 20, 00, 0);

            var javascriptMvcFrameworkWithSelected = Lists.AllJavascriptFrameworks<int>();
            javascriptMvcFrameworkWithSelected.SelectedValues = 2;

            var webBrowsers = Lists.AllWebBrowsers<int>();

            return new RegisterModel()
            {
                CountriesList = ddlWithSelected,
                NotificationList = enumWithSelected,
                TechnologiesList = Lists.AllTech<List<int>>(),
                TechnologiesCheckboxList = listWithSelected,
                LanguagesList = langListWithSelected,
                IdeList = ideListWithSelected,
                GenderList = Lists.AllGenders<int>().ToSelectList().ToList(),
                Birthday = new BsDateTime(),
                Interval = new BsRange<DateTime>
                {
                    From = new BsRangeItem<DateTime>
                    {
                        ItemValue = DateTime.Now
                    },
                    To = new BsRangeItem<DateTime>
                    {
                        ItemValue = DateTime.Now.AddDays(12)
                    }
                },
                JavascriptMvcFramework = javascriptMvcFrameworkWithSelected,
                WebBrowsers = webBrowsers
            };
        }
    }
}