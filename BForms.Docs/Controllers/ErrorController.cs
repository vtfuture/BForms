using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BForms.Docs.Controllers
{
    public class ErrorController : BaseController
    {
        //
        // GET: /Error/
        public ActionResult Index()
        {
            return View("Error");
        }

        public ViewResult NotFound()
        {
            Response.StatusCode = 404;
            return View("NotFound");
        }
	}
}