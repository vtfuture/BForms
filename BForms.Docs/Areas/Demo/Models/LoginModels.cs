using BForms.Docs.Areas.Demo.Mock;
using BForms.Docs.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BootstrapForms.Models;
using BootstrapForms.Mvc;

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
        [Display(Name = "Name", Prompt = "Surname and name")]
        [BsControl(BsControlType.TextBox)]
        public string Name { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Email address", Prompt = "email@example.com")]
        [BsControl(BsControlType.Email)]
        public string Email { get; set; }

        
        [Display(Name = "Personal website", Prompt = "http://www.mysite.com")]
        [BsControl(BsControlType.Url)]
        public string Website { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Password", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Retype password")]
        [Compare("Password")]
        [BsControl(BsControlType.Password)]
        public string PasswordRetyped { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Birthday")]
        [BsControl(BsControlType.DatePicker)]
        public DateTime? Birthday { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Annual income")]
        [BsControl(BsControlType.Number)]
        public decimal? AnnualIncome { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Location", Prompt = "Choose your country")]
        [BsControl(BsControlType.DropDownList)]
        public BsSelectList<string> CountriesList { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Receive email notifications", Description = "Your register email address will be used")]
        [BsControl(BsControlType.RadioButtonList)]
        public BsSelectList<NotificationType?> NotificationList { get; set; }

        [Display(Name = "What ASP.NET flavors do you use")]
        [BsControl(BsControlType.CheckBoxList)]
        public BsSelectList<List<int>> TechnologiesCheckboxList { get; set; }    

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Technologies", Prompt = "Choose your favorite technologies")]
        [BsControl(BsControlType.ListBox)]
        public BsSelectList<List<int>> TechnologiesList { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Programming languages", Prompt = "Type your favorite programming languages")]
        [BsControl(BsControlType.TagList)]
        public BsSelectList<List<string>> LanguagesList { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Programming IDE", Prompt = "Type your favorite IDE")]
        [BsControl(BsControlType.Autocomplete)]
        public BsSelectList<string> IdeList { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Gender", Prompt = "Choose your gender")]
        [BsControl(BsControlType.DropDownList)]
        public int? Gender { get; set; }
        public List<System.Web.Mvc.SelectListItem> GenderList { get; set; }

        [BsMandatory(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "I agree with Terms & Conditions")]
        [BsControl(BsControlType.CheckBox)]
        public bool ConsentAgreement { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Interval", Prompt = "Choose dates")]
        [BsControl(BsControlType.DateTimePickerRange)]
        public BsRange<DateTime?> Interval { get; set; }
    }

}