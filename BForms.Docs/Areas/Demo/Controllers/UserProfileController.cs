using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Web;
using System.Web.Mvc;
using BForms.Docs.Areas.Demo.Models;
using BForms.Docs.Controllers;

namespace BForms.Docs.Areas.Demo.Controllers
{
    public class UserProfileController : BaseController
    {
        //
        // GET: /Demo/UserProfile/
        public ActionResult Index()
        {
            var model = GetContributorModel();

            return View(model);
        }

        private UserProfileModel GetContributorModel()
        {
            return new UserProfileModel()
            {
                Firstname = "John",
                Lastname = "Doe",
                Department = "Web",
                Organization = "Google"
            };
        }

	}
}