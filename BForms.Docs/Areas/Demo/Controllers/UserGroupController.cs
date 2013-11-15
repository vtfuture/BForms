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
using RequireJS;
using BForms.GroupEditor;

namespace BForms.Docs.Areas.Demo.Controllers
{
    public class ContributorsOrderModel {}

    public class ContributorsInheritExample : BsGroupEditor<ContributorRowModel, ContributorSearchModel>
    {
        public ContributorsOrderModel Order { get; set; }
    }

    public class UserGroupEditorViewModel
    {
        [BsGroupEditor(Name = "Contributors1", Id = YesNoValueTypes.Yes, Selected = false)]
        public ContributorsInheritExample Contributors { get; set; }

        [BsGroupEditor(Name = "Contributors2", Id = YesNoValueTypes.No, Selected = false)]
        public BsGroupEditor<ContributorRowModel> Contributors2 { get; set; }

        [BsGroupEditor(Name = "Contributors3", Id = YesNoValueTypes.Both, Selected = true)]
        public BsGroupEditor<ContributorRowModel, ContributorSearchModel, ContributorNewModel> Contributors3 { get; set; }
    }

    public class UserGroupViewModel
    {
        public UserGroupEditorViewModel Editor1 { get; set; }
        public UserGroupEditorViewModel Editor2 { get; set; }
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
            var model = new UserGroupEditorViewModel()
            {
                Contributors = new ContributorsInheritExample
                {
                    Grid = repo.ToBsGridViewModel(new BsGridRepositorySettings<ContributorSearchModel>
                    {
                        Page = 1,
                        PageSize = 5
                    }),
                    Search = repo.GetSearchForm(),
                    Order = new ContributorsOrderModel()
                },
                Contributors3 = new BsGroupEditor<ContributorRowModel, ContributorSearchModel, ContributorNewModel>
                {
                    Grid = repo.ToBsGridViewModel(new BsGridRepositorySettings<ContributorSearchModel>
                    {
                        Page = 1,
                        PageSize = 5
                    }),
                    Search = repo.GetSearchForm(),
                    New = repo.GetNewForm()
                }
            };

            var viewModel = new UserGroupViewModel
            {
                Editor2 = model
            };

            var options = new Dictionary<string, object>
            {
                {"getTabUrl", Url.Action("GetTab")},
            };

            RequireJsOptions.Add("index", options);

            return View(viewModel);
        }

        public class GroupEditorRequest
        {
            public int Page { get; set; }
            public int PageSize { get; set; }
            public int TabId { get; set; }
            public ContributorSearchModel Search1 { get; set; }
            public ContributorSearchModel Search2 { get; set; }

            public BsGridRepositorySettings<T> GetRepositorySettings<T>()
            {
                foreach (var item in this.GetType().GetProperties())
                {
                    if (item.PropertyType.IsAssignableFrom(typeof(T)))
                    {
                        return new BsGridRepositorySettings<T>
                        {
                            Search = (T)item.GetValue(this),
                            Page = this.Page,
                            PageSize = this.PageSize
                        };
                    }
                }

                throw new ArgumentException("The generic type was not found in object props");
            }
        }

        public BsJsonResult GetTab(BsGroupEditorRepositorySettings<YesNoValueTypes> settings)
        {
            var msg = string.Empty;
            var status = BsResponseStatus.Success;
            var html = string.Empty;
            var count = 0;

            try
            {
                UserGroupEditorViewModel model = new UserGroupEditorViewModel();
                
                switch (settings.TabId)
                {
                    case YesNoValueTypes.No:

                        //var grid2 = repo.ToBsGridViewModel(request.GetRepositorySettings<ContributorSearchModel>(), out count);
                        var grid2 = repo.ToBsGridViewModel(settings.ToBaseGridRepositorySettings(), out count);

                        model.Contributors2 = new BsGroupEditor<ContributorRowModel>
                        {
                            Grid = grid2
                        };
                        break;

                    case YesNoValueTypes.Yes:

                        var grid1 = repo.ToBsGridViewModel(settings.ToGridRepositorySettings<ContributorSearchModel>(), out count);

                        model.Contributors = new ContributorsInheritExample
                        {
                            Grid = grid1
                        };
                        break;

                    case YesNoValueTypes.Both:

                        var grid3 = repo.ToBsGridViewModel(settings.ToGridRepositorySettings<ContributorSearchModel>(), out count);

                        model.Contributors3 = new BsGroupEditor<ContributorRowModel, ContributorSearchModel, ContributorNewModel>
                        {
                            Grid = grid3
                        };
                        break;
                }

                var viewModel = new UserGroupViewModel()
                {
                    Editor2 = model
                };

                html = this.BsRenderPartialView("_Editors", viewModel);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                status = BsResponseStatus.ServerError;
            }

            return new BsJsonResult(new
            {
                Count = count,
                Html = html
            }, status, msg);
        }
    }
}