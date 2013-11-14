using System.ComponentModel;
using BForms.Models;
using BForms.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BForms.Docs.Resources;


namespace BForms.Docs.Areas.Demo.Models
{
    #region Readonly
    [Serializable]
    public class UserProfileModel
    {
        [BsPanel(Id = PanelComponentsEnum.Basic, Expandable = false, Editable = false)]
        [Display(Name = "User")]
        public UserProfileBasicModel Basic { get; set; }

        [BsPanel(Id = PanelComponentsEnum.UserInfo)]
        [Display(Name = "User Info")]
        public UserProfileInfoModel UserInfo { get; set; }

        [BsPanel(Id = PanelComponentsEnum.Contact)]
        [Display(Name = "Contact")]
        public UserProfileContactModel Contact { get; set; }
    }

    [Serializable]
    public class UserProfileBasicModel
    {
        public string Username { get; set; }

        public string Department { get; set; }

        public string Organization { get; set; }
    }

    [Serializable]
    public class UserProfileContactModel
    {
        public string Mail { get; set; }

        public string Website { get; set; }
    }

    public class UserProfileInfoModel
    {
        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }

        public DateTime HireDate { get; set; }
    }
    #endregion

    #region Editable
    public class UserProfileEditableModel
    {
        public UserProfileInfoEditableModel UserInfo { get; set; }

        public UserProfileContactEditableModel Contact { get; set; }
    }

    public class UserProfileInfoEditableModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "FirstName", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.TextBox)]
        public string Firstname { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "LastName", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.TextBox)]
        public string Lastname { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Password", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "RetypePassword", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.Password)]
        [Compare("Password")]
        public string RetypePassword { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "HireDate", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.DatePicker)]
        public BsDateTime HireDate { get; set; }

    }

    public class UserProfileContactEditableModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Email", Prompt = "EmailPrompt", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.Email)]
        public string Mail { get; set; }

        [Display(Name = "PersonalWebsite", Prompt = "SitePrompt", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.Url)]
        public string Website { get; set; }
    }
    #endregion

    public enum PanelComponentsEnum
    {
        Basic = 1,
        UserInfo = 2,
        Contact = 3
    }

}