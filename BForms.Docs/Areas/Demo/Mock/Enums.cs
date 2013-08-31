using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BForms.Docs.Resources;

namespace BForms.Docs.Areas.Demo.Mock
{
    public enum NotificationType
    {
        [Display(Name = "Password", ResourceType = typeof(Resource))]
        Never = 1,
        [Display(Name = "Daily")]
        Daily = 2,
        [Display(Name = "Weekly")]
        Weekly = 3,
        [Display(Name = "Monthly")]
        Monthly = 4
    }
}