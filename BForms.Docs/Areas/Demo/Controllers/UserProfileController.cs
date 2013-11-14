using System;
using System.Collections.Generic;
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
            var msg = string.Empty;
            var status = BsResponseStatus.Success;

            try
            {

                switch (component)
                {
                    case PanelComponentsEnum.UserInfo:
                        html = this.BsRenderPartialView("Readonly/_UserInfo", GetUserProfileModel().UserInfo);
                        break;

                    case PanelComponentsEnum.Contact:
                        html = this.BsRenderPartialView("Readonly/_Contact", GetUserProfileModel().Contact);
                        break;
                }

            }

            catch (Exception ex)
            {
                msg = ex.Message;
                status = BsResponseStatus.ServerError;
            }

            return new BsJsonResult(new
            {
                Html = html
            }, status, msg);
        }

        public BsJsonResult GetEditableContent(PanelComponentsEnum component)
        {
            var html = string.Empty;
            var model = GetUserProfileModelEditable();

            switch (component)
            {
                case PanelComponentsEnum.UserInfo:
                    html = this.BsRenderPartialView("Editable/_UserInfo", model.UserInfo, model.GetPropertyName(x => x.UserInfo));
                    break;
                case PanelComponentsEnum.Contact:
                    html = this.BsRenderPartialView("Editable/_Contact", model.Contact, model.GetPropertyName(x => x.Contact));
                    break;
            }

            return new BsJsonResult(new
            {
                Html = html
            });
        }

        public BsJsonResult SetContent(UserProfileEditableModel model, PanelComponentsEnum component)
        {

            var html = string.Empty;

            switch (component)
            {
                case PanelComponentsEnum.UserInfo:
                    ModelState.ClearModelState(model.GetPropertyName(m => m.UserInfo) + ".");
                    break;

                case PanelComponentsEnum.Contact:
                    ModelState.ClearModelState(model.GetPropertyName(m => m.Contact) + ".");
                    break;
            }

            if (ModelState.IsValid)
            {
                var profileModel = GetUserProfileModel();

                switch (component)
                {
                    case PanelComponentsEnum.UserInfo:
                        profileModel.UserInfo.Firstname = model.UserInfo.Firstname;
                        profileModel.UserInfo.Lastname = model.UserInfo.Lastname;
                        profileModel.UserInfo.Password = model.UserInfo.Password;
                        profileModel.UserInfo.HireDate = model.UserInfo.HireDate.DateValue.Value;

                        html = this.BsRenderPartialView("Readonly/_UserInfo", profileModel.UserInfo);

                        break;

                    case PanelComponentsEnum.Contact:
                        profileModel.Contact.Mail = model.Contact.Mail;
                        profileModel.Contact.Website = model.Contact.Website;

                        html = this.BsRenderPartialView("Readonly/_Contact", profileModel.Contact);

                        break;
                }

                SaveUserProfileModel(profileModel);

            }
            else
            {
                //JSON serialize ModelState errors
                return new BsJsonResult(
                    new Dictionary<string, object> { { "Errors", ModelState.GetErrors() } },
                    BsResponseStatus.ValidationError);
            }

            return new BsJsonResult(new
            {
                Html = html
            });
        }
        #endregion

        #region Helpers
        private UserProfileModel GetUserProfileModel()
        {
            var userModel = Session["UserProfileModel"] as UserProfileModel;

            if (userModel == null)
            {
                userModel = new UserProfileModel()
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

                Session["UserProfileModel"] = userModel;
            }

            return userModel;
        }

        private void SaveUserProfileModel(UserProfileModel model)
        {
            Session["UserProfileModel"] = model;
        }

        private UserProfileEditableModel GetUserProfileModelEditable()
        {
            var model = GetUserProfileModel();

            return new UserProfileEditableModel()
            {
                UserInfo = new UserProfileInfoEditableModel
                {
                    Firstname = model.UserInfo.Firstname,
                    Lastname = model.UserInfo.Lastname,
                    HireDate = new BsDateTime
                    {
                        DateValue = model.UserInfo.HireDate
                    }
                },
                Contact = new UserProfileContactEditableModel
                {
                    Mail = model.Contact.Mail,
                    Website = model.Contact.Website
                }
            };
        }
        #endregion
    }
}