using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.ComponentModel.DataAnnotations;
using BForms.Models;
using BForms.Mvc;
using BForms.Utilities;
using BForms.Docs.Areas.Demo.Mock;
using BForms.Docs.Areas.Demo.Models;
using BForms.Docs.Areas.Demo.Helpers;
using BForms.Docs.Controllers;

namespace BForms.Docs.Areas.Demo.Controllers
{
    public class UserGroupController : BaseController
    {
        //
        // GET: /Demo/UserGroup/
        public ActionResult Index()
        {
            return View();
        }
	}
}