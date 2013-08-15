using BForms.Docs.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BForms.Docs.Areas.Demo.Models;
using BForms.Docs.Areas.Demo.Helpers;

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
                RegisterModel = new RegisterModel() { CountriesDropdown = Utils.GetCounties() }
            };

            return View(model);
        }
	}
}