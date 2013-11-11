using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Web;
using System.Web.Mvc;
using BForms.Docs.Areas.Demo.Models;
using BForms.Docs.Controllers;
using BForms.Models;
using BForms.Mvc;

namespace BForms.Docs.Areas.Demo.Controllers
{
    public class UserProfileController : BaseController
    {
        //
        // GET: /Demo/UserProfile/
        public ActionResult Index()
        {
            var model = GetUserProfileModel();

            return View(model);
        }

        private UserProfileModel GetUserProfileModel()
        {
            return new UserProfileModel()
            {
                Username = "ms sam",
                Firstname = "John",
                Lastname = "Doe",
                Department = "Web",
                Organization = "Google",
                Password = "password1",
                HireDate = DateTime.Now
            };
        }

        #region Ajax
        public BsJsonResult GetUserInfo()
        {
            var html = this.BsRenderPartialView("Readonly/_UserInfo", GetUserProfileModel());

            return new BsJsonResult(new
            {
                Html = html
            });
        }
        #endregion
    }
}