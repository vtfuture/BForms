using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BForms.Docs.Models;

namespace BForms.Docs.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public void SaveTheme(ThemeSettings settings)
        {
            Session["ThemeSettings"] = settings;
        }
    }
}