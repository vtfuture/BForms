using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BootstrapForms.Models;
using BForms.Docs.Areas.Demo.Models;

namespace BForms.Docs.Areas.Demo.Helpers
{
    public static class Utils
    {
        public static string GetRoleIcon<T>(this HtmlHelper<T> helper, ProjectRole role)
        {
            switch (role)
            {
                case ProjectRole.TeamLeader:
                    return "icn-three_stars";
                case ProjectRole.Developer:
                    return "icn-two_stars";
                default:
                    return "icn-one_star";
            }
        }
    }
}