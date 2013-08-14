using BForms.Docs.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BForms.Docs.Areas.Demo.Models
{
    public class LoginModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "UserName", Prompt = "UserName", ResourceType = typeof(Resource))]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [DataType(DataType.Password)]
        [Display(Name = "Password", Prompt = "Password", ResourceType = typeof(Resource))]
        public string Password { get; set; }
    }
}