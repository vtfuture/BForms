using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BForms.Docs.Resources;

namespace BForms.Docs.Areas.Demo.Mock
{
    public enum NotificationTypes
    {
        [Display(Name = "Password", ResourceType = typeof(Resource))]
        Never = 0,
        [Display(Name = "Daily")]
        Daily = 1,
        [Display(Name = "Weekly")]
        Weekly = 2,
        [Display(Name = "Monthly")]
        Monthly = 3
    }
}