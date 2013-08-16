using BForms.Docs.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BForms.Docs.Areas.Demo.Models;
using BForms.Docs.Areas.Demo.Helpers;
using System.ComponentModel.DataAnnotations;
using BootstrapForms.Attributes;
using BootstrapForms.Models;

namespace BForms.Docs.Areas.Demo.Controllers
{

    public class LoginController : BaseController
    {
        [Display(Name = "Location", Prompt = "Chose your country")]
        [BsControl(BsControlType.ListBoxGrouped)]
        public BsSelectList<string> MyList { get; set; }

        //
        // GET: /Demo/Login/
        public ActionResult Index()
        {
            MyList = Utils.AllCounties();
            ViewBag.MyList = MyList;

            var model = new AuthenticationModel()
            {
                LoginModel = new LoginModel(),
                RegisterModel = new RegisterModel()
                {
                    CountriesDropdown = Utils.AllCounties(),
                    NotificationDropdown = Utils.GetNotificationTypes()
                }
            };

            return View(model);
        }


    }
}