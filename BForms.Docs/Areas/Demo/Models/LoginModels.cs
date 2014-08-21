using BForms.Docs.Areas.Demo.Mock;
using BForms.Docs.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BForms.Models;
using BForms.Mvc;

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
        [Display(Name = "Email", Prompt = "Email", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.Email)]
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Password", Prompt = "Password", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.Password)]
        public string Password { get; set; }

        [Display(Name = "RememberMe", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.CheckBox)]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Name", Prompt = "NamePrompt", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.TextBox)]
        public string Name { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Email", Prompt = "EmailPrompt", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.Email)]
        public string Email { get; set; }

        [Display(Name = "PersonalWebsite", Prompt = "SitePrompt", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.Url)]
        public string Website { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Password", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "RetypePassword", ResourceType = typeof(Resource))]
        [Compare("Password")]
        [BsControl(BsControlType.Password)]
        public string PasswordRetyped { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Birthday", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.DatePicker)]
        public BsDateTime Birthday { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "AnnualIncome", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.Number)]
        public decimal? AnnualIncome { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Location", Prompt = "PromptLocation", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.DropDownList)]
        public BsSelectList<string> CountriesList { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "EmailNotifications", Description = "EmailNotificationsPrompt", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.RadioButtonList)]
        public BsSelectList<NotificationType?> NotificationList { get; set; }

        [Display(Name = "ReceiveEmailsAtSpecifiedTime", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.TimePicker)]
        public BsDateTime NotificationTime { get; set; }

        [Display(Name = "AspTechnologiesQuestion", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.CheckBoxList)]
        public BsSelectList<List<int>> TechnologiesCheckboxList { get; set; }    

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Technologies", Prompt = "TechnologiesPrompt", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.ListBox)]
        public BsSelectList<List<int>> TechnologiesList { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "ProgrammingLanguages", Prompt = "ProgrammingLanguagesPrompt", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.TagList)]
        public BsSelectList<List<string>> LanguagesList { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "ProgrammingIDE", Prompt = "ProgrammingIDEPrompt", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.Autocomplete)]
        public BsSelectList<string> IdeList { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Gender", Prompt = "GenderPrompt", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.DropDownList)]
        public int? Gender { get; set; }
        public List<System.Web.Mvc.SelectListItem> GenderList { get; set; }

        [BsMandatory(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "ConsentAgreement", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.CheckBox)]
        public bool ConsentAgreement { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Interval", Prompt = "IntervalPrompt", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.DateTimePickerRange, IsReadonly = true)]
        public BsRange<DateTime> Interval { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "JavascriptMvcFramework", Prompt = "JavascriptMvcFrameworkPrompt", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.ButtonGroupDropdown)]
        public BsSelectList<int> JavascriptMvcFramework { get; set; }
    }

}