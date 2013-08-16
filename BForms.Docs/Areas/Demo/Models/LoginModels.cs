using BForms.Docs.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BootstrapForms.Attributes;
using BootstrapForms.Models;

namespace BForms.Docs.Areas.Demo.Models
{
    public class AuthenticationModel
    {
        public LoginModel LoginModel { get; set; }
        public RegisterModel RegisterModel { get; set; }
    }

    public class LoginModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Email", Prompt = "Email")]
        [BsControl(BsControlType.Email)]
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Password", Prompt = "Password", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.Password)]
        public string Password { get; set; }

        [Display(Name = "RememberMe")]
        [BsControl(BsControlType.CheckBox)]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Name")]
        [BsControl(BsControlType.TextBox)]
        public string Name { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Email")]
        [BsControl(BsControlType.Email)]
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Password", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "PasswordRetyped")]
        [Compare("Password")]
        [BsControl(BsControlType.Password)]
        public string PasswordRetyped { get; set; }

        [Display(Name = "EnableNotifications")]
        [BsControl(BsControlType.CheckBox)]
        public bool EnableNotifications { get; set; }

        [Display(Name = "Location", Prompt = "Chose your country")]
        [BsControl(BsControlType.DropDownList)]
        public BsSelectList<string> CountriesDropdown { get; set; }

        [Display(Name = "Notifications", Description = "Your register email address will be used")]
        [BsControl(BsControlType.RadioButtonList)]
        public string NotificationTypeId { get; set; }
        public List<System.Web.Mvc.SelectListItem> NotificationDropdown { get; set; }
    }


}