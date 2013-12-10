using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BForms.Models;

namespace BForms.EndToEnd.Controllers
{
    public class PagerForController : Controller
    {
        //
        // GET: /PagerFor/
        public ActionResult Index()
        {
            return View(new PagerViewModel
            {
                PagerModel = new BsPagerModel(10,page:2)
            });
        }
	}

    public class PagerViewModel
    {
        public BsPagerModel PagerModel { get; set; }
    }
}