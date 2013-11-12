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
    public class UserProfileModel
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Department { get; set; }

        public string Role { get; set; }

        public string Password { get; set; }

        public string Organization { get; set; }

        public string Mail { get; set; }

        public string Phone { get; set; }

        public DateTime HireDate { get; set; }
    }

    public class UserProfileEditableModel
    {
        public UserProfileInfoModel UserInfo { get; set; }
    }

    public class UserProfileInfoModel
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

}