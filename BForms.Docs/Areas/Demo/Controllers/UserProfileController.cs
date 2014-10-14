using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using BForms.Docs.Areas.Demo.Models;
using BForms.Docs.Controllers;
using BForms.Docs.Helpers;
using BForms.Docs.Resources;
using BForms.Models;
using BForms.Mvc;

using RequireJsNet;

namespace BForms.Docs.Areas.Demo.Controllers
{
    public class UserProfileController : BaseController
    {
        //
        // GET: /Demo/UserProfile/
        public ActionResult Index()
        {
            var model = GetUserProfileModel();

            RequireJsOptions.Add("uploadUrl", Url.Action("UploadAvatar"));
            RequireJsOptions.Add("avatarUrl", Url.Action("GetAvatar"));
            RequireJsOptions.Add("deleteAvatarUrl", Url.Action("DeleteAvatar"));

            return View(model);
        }

        #region Ajax

        public BsJsonResult GetReadonlyContent(PanelComponentsEnum componentId)
        {
            var html = string.Empty;
            var msg = string.Empty;
            var status = BsResponseStatus.Success;

            try
            {
                switch (componentId)
                {
                    case PanelComponentsEnum.UserInfo:

                        var userInfo = GetUserProfileModel().UserInfo;

                        var passwordRegex = new Regex(".");
                        userInfo.Password = passwordRegex.Replace(userInfo.Password, "*");

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

        public BsJsonResult GetEditableContent(PanelComponentsEnum componentId)
        {
            var html = string.Empty;
            var model = GetUserProfileModelEditable();

            switch (componentId)
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

        public BsJsonResult SetContent(UserProfileEditableModel model, PanelComponentsEnum componentId)
        {

            var html = string.Empty;
            var status = BsResponseStatus.Success;
            var msg = string.Empty;

            switch (componentId)
            {
                case PanelComponentsEnum.UserInfo:
                    ModelState.ClearModelState(model.GetPropertyName(m => m.UserInfo) + ".");
                    break;

                case PanelComponentsEnum.Contact:
                    ModelState.ClearModelState(model.GetPropertyName(m => m.Contact) + ".");
                    break;
            }

            try
            {

                if (ModelState.IsValid)
                {
                    var profileModel = GetUserProfileModel();

                    switch (componentId)
                    {
                        case PanelComponentsEnum.UserInfo:

                            var dateValue = model.UserInfo.HireDate.DateValue.Value;

                            if (dateValue.Year == 2014 && dateValue.Day == 1 && dateValue.Month == 1)
                            {
                                throw new Exception("This is how an exception message is displayed inside a panel");
                            }

                            profileModel.UserInfo.Firstname = model.UserInfo.Firstname;
                            profileModel.UserInfo.Lastname = model.UserInfo.Lastname;
                            profileModel.UserInfo.Password = model.UserInfo.Password;
                            profileModel.UserInfo.HireDate = model.UserInfo.HireDate.DateValue.Value;
                            profileModel.Basic.Username = model.UserInfo.Firstname + " " + model.UserInfo.Lastname;

                            SaveUserProfileModel(profileModel);

                            var passwordRegex = new Regex(".");
                            profileModel.UserInfo.Password = passwordRegex.Replace(profileModel.UserInfo.Password, "*");

                            html = this.BsRenderPartialView("Readonly/_UserInfo", profileModel.UserInfo);

                            return new BsJsonResult(new
                            {
                                Html = html,
                                Username = profileModel.Basic.Username
                            });

                            break;

                        case PanelComponentsEnum.Contact:
                            profileModel.Contact.Mail = model.Contact.Mail;
                            profileModel.Contact.Website = model.Contact.Website;

                            SaveUserProfileModel(profileModel);


                            html = this.BsRenderPartialView("Readonly/_Contact", profileModel.Contact);

                            break;
                    }


                }
                else
                {
                    //JSON serialize ModelState errors
                    return new BsJsonResult(
                        new Dictionary<string, object> {{"Errors", ModelState.GetErrors()}},
                        BsResponseStatus.ValidationError);
                }

            }

            catch (Exception ex)
            {
                msg = "<strong>" + Resource.ServerError + "!</strong> " + ex.Message;
                status = BsResponseStatus.ServerError;
            }

            return new BsJsonResult(new
            {
                Html = html
            },status,msg);
        }

        public BsJsonResult UploadAvatar()
        {
            var status = BsResponseStatus.Success;
            var msg = string.Empty;

            try
            {
                if (Request.Files != null && Request.Files.Count > 0)
                {
                    var postedFile = Request.Files[0];
                    var target = new MemoryStream();
                    postedFile.InputStream.CopyTo(target);
                    byte[] data = target.ToArray();

                    Session["Avatar"] = data;
                }
            }
            catch (Exception ex)
            {
                status = BsResponseStatus.ServerError;
                msg = ex.Message;
            }


            return new BsJsonResult(new
            {
                AvatarUrl = Url.Action("GetAvatar")
            }, status, msg);
        }

        public BsJsonResult DeleteAvatar()
        {
            var status = BsResponseStatus.Success;
            var msg = string.Empty;

            try
            {
                Session["Avatar"] = null;
            }

            catch (Exception ex)
            {
                status = BsResponseStatus.ServerError;
                msg = ex.Message;
            }

            return new BsJsonResult(new
            {
                AvatarUrl = Url.Action("GetAvatar")
            }, status, msg);
        }


        public ActionResult GetAvatar()
        {
            if (Session["Avatar"] != null)
            {
                return new ImageResult
                {
                    ImageData = Session["Avatar"] as byte[],
                    MimeType = "image/png",
                    Cacheability = HttpCacheability.NoCache
                };
            }
            else
                return File(Url.Content("~/Scripts/BForms/Images/bg-user.png"), "image/png");
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
                        Department = "Web",
                        Organization = "BForms"
                    },
                    UserInfo = new UserProfileInfoModel
                    {
                        Firstname = "Stefan",
                        Lastname = "Prodan",
                        Password = "password1",
                        Role = "Team leader",
                        HireDate = new DateTime(2013, 8, 1)
                    },
                    Contact = new UserProfileContactModel
                    {
                        Mail = "bforms@mvc.fast",
                        Website = "http://www.stefanprodan.com"
                    }
                };

                userModel.Basic.Username = userModel.UserInfo.Firstname + " " + userModel.UserInfo.Lastname;

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