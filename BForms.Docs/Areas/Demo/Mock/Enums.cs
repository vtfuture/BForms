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

    public enum YesNoValueTypes
    {
        [Display(Name = "Any", ResourceType = typeof(Resource))]
        Both = 1,
        [Display(Name = "Yes", ResourceType = typeof(Resource))]
        Yes = 2,
        [Display(Name = "No", ResourceType = typeof(Resource))]
        No = 3,
    }
}