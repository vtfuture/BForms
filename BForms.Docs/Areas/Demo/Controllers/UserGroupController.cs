using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.ComponentModel.DataAnnotations;
using BForms.Models;
using BForms.Mvc;
using BForms.Utilities;
using BForms.Docs.Areas.Demo.Mock;
using BForms.Docs.Areas.Demo.Models;
using BForms.Docs.Areas.Demo.Helpers;
using BForms.Docs.Controllers;

namespace BForms.Docs.Areas.Demo.Controllers
{
    public class UserGroupViewModel
    {
        [BsGroupEditor(Name = "Contributors1", Id = YesNoValueTypes.Yes)]
        public BsGridModel<Contributor> Contributors { get; set; }

        [BsGroupEditor(Name = "Contributors2", Id = YesNoValueTypes.No)]
        public BsGridModel<Contributor> Contributors2 { get; set; }
    }

    public class UserGroupController : BaseController
    {
        //
        // GET: /Demo/UserGroup/
        public ActionResult Index()
        {
            var model = new UserGroupViewModel()
            {
                Contributors = new BsGridModel<Contributor>
                {
                    Items = new List<Contributor>(),
                    Pager = new BsPagerModel(10)
                }
            };
            return View(model);
        }
    }
}