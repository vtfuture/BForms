using BForms.Docs.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BForms.Docs.Areas.Demo.Controllers
{
    public class LoginController : BaseController
    {
        //
        // GET: /Demo/Login/
        public ActionResult Index()
        {
            return View();
        }
	}
}