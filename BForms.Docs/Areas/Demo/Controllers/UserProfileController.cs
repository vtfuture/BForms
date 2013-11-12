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

        #region Ajax

        public BsJsonResult GetReadonlyContent(PanelComponentsEnum component)
        {
            var html = string.Empty;

            switch (component)
            {
                case PanelComponentsEnum.UserInfo:
                    html = this.BsRenderPartialView("Readonly/_UserInfo", GetUserProfileModel().UserInfo);
                    break;

                case PanelComponentsEnum.Contact:
                    html = this.BsRenderPartialView("Readonly/_Contact", GetUserProfileModel().Contact);
                    break;
            }

            return new BsJsonResult(new
            {
                Html = html
            });
        }

        public BsJsonResult GetEditableContent(PanelComponentsEnum component)
        {
            var html = string.Empty;

            switch (component)
            {
                case PanelComponentsEnum.UserInfo:
                    html = this.BsRenderPartialView("Editable/_UserInfo", GetUserProfileModelEditable().UserInfo);
                    break;
                case PanelComponentsEnum.Contact:
                    html = this.BsRenderPartialView("Editable/_Contact", GetUserProfileModelEditable().Contact);
                    break;
            }

            return new BsJsonResult(new
            {
                Html = html
            });
        }

        public BsJsonResult SaveUserInfoForm(UserProfileEditableModel model, PanelComponentsEnum component)
        {
            return new BsJsonResult(new
            {
            });
        }
        #endregion

        #region Helpers
        private UserProfileModel GetUserProfileModel()
        {
            return new UserProfileModel()
            {
                Basic = new UserProfileBasicModel
                {
                    Username = "ms sam",
                    Department = "Web",
                    Organization = "Google"
                },
                UserInfo = new UserProfileInfoModel
                {
                    Firstname = "John",
                    Lastname = "Doe",
                    Password = "password1",
                    Role = "Team leader",
                    HireDate = DateTime.Now
                },
                Contact = new UserProfileContactModel
                {
                    Mail = "bforms@mvc.fast",
                    Website = "http://bforms.stefanprodan.eu"
                }
            };
        }

        private UserProfileEditableModel GetUserProfileModelEditable()
        {
            return new UserProfileEditableModel()
            {
                UserInfo = new UserProfileInfoEditableModel
                {
                    Firstname = "John",
                    Lastname = "Doe"
                },
                Contact = new UserProfileContactEditableModel
                {
                    Mail = "bforms@mvc.fast",
                    Website = "http://bforms.stefanprodan.eu"
                }
            };
        }
        #endregion
    }
}