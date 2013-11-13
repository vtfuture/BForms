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
using BForms.Docs.Areas.Demo.Repositories;
using BForms.Grid;

namespace BForms.Docs.Areas.Demo.Controllers
{
    public class UserGroupViewModel
    {
        [BsGroupEditor(Name = "Contributors1", Id = YesNoValueTypes.Yes, Selected = false)]
        public BsGroupEditor<ContributorRowModel, ContributorSearchModel> Contributors { get; set; }

        [BsGroupEditor(Name = "Contributors2", Id = YesNoValueTypes.No, Selected = false)]
        public BsGroupEditor<ContributorRowModel> Contributors2 { get; set; }

        [BsGroupEditor(Name = "Contributors3", Id = YesNoValueTypes.Both, Selected = true)]
        public BsGroupEditor<ContributorRowModel, ContributorSearchModel, ContributorNewModel> Contributors3 { get; set; }
    }

    public class UserGroupController : BaseController
    {
        private readonly ContributorsRepository repo;

        public UserGroupController()
        {
            repo = new ContributorsRepository(Db);
        }

        //
        // GET: /Demo/UserGroup/
        public ActionResult Index()
        {
            var model = new UserGroupViewModel()
            {
                Contributors = new BsGroupEditor<ContributorRowModel, ContributorSearchModel>
                {
                    Grid = new BsGridModel<ContributorRowModel>
                    {
                        Items = new List<ContributorRowModel>(),
                        Pager = new BsPagerModel(5)
                    },
                    Search = repo.GetSearchForm(),
                    InlineSearch = true
                },
                Contributors2 = new BsGroupEditor<ContributorRowModel>
                {
                    Grid = new BsGridModel<ContributorRowModel>
                    {
                        Items = new List<ContributorRowModel>(),
                        Pager = new BsPagerModel(5)
                    }
                },
                Contributors3 = new BsGroupEditor<ContributorRowModel, ContributorSearchModel, ContributorNewModel>
                {
                    Grid = repo.ToBsGridViewModel(new BsGridRepositorySettings<ContributorSearchModel>
                    {
                        Page = 1,
                        PageSize = 5
                    })
                }
            };
            return View(model);
        }
    }
}