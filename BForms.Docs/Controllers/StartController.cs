using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BForms.Docs.Controllers
{
    public class StartController : BaseController
    {

        public ActionResult Setup()
        {
            return View("Setup/Index");
        }

        public ActionResult Form()
        {
            return View("Form/Index");
        }

        public ActionResult Grid()
        {
            return View("Grid/Index");
        }

        public ActionResult Group()
        {
            return View("Group/Index");
        }
    }
}