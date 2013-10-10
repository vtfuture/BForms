using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BForms.Docs.Controllers
{
    public class ComponentsController : BaseController
    {

        public ActionResult Overview()
        {
            return View("Overview/Overview");
        }

        public ActionResult InputExtensions()
        {
            return View("InputExtensions/InputExtensions");
        }

        public ActionResult RangeExtensions()
        {
            return View("RangeExtensions/RangeExtensions");
        }

        public ActionResult FileExtensions()
        {
            return View();
        }

        public ActionResult SelectExtensions()
        {
            return View("SelectExtensions/SelectExtensions");
        }

        public ActionResult Grid()
        {
            return View("Grid/Index");
        }
    }
}