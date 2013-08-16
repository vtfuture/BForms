using BForms.Docs.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using BForms.Docs.Areas.Demo.Models;
using BForms.Docs.Areas.Demo.Helpers;
using System.ComponentModel.DataAnnotations;
using BootstrapForms.Attributes;
using BootstrapForms.Models;
using BootstrapForms.Utilities;

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
                    CountriesDropdown = Utils.AllCounties<List<string>>(),
                    NotificationDropdown = Utils.GetNotificationTypes<int?>()
                }
            };

            RequireJsOptions.Add("registerUrl",Url.Action("Register"));

            return View(model);
        }

        public BsJsonResult Register(AuthenticationModel model)
        {
            ModelState.ClearModelState(model.GetPropertyName(m => m.LoginModel) + ".");
            ModelState.AddModelError("RegisterModel.Name","Testing server validation error");
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