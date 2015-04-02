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
using RequireJsNet;
using BForms.Editor;
using BForms.Docs.Resources;

namespace BForms.Docs.Areas.Demo.Controllers
{
    public class GroupEditorController : BaseController
    {
        #region Ctor and Properties
        private readonly ContributorsRepository repo;

        public GroupEditorController()
        {
            repo = new ContributorsRepository(Db);
        }
        #endregion

        #region Pages
        public ActionResult Index()
        {
            var model = new GroupEditorModel()
            {
                Developers = new BsEditorTabModel<ContributorRowModel, ContributorSearchModel, ContributorNewModel>
                {
                    Grid = repo.ToBsGridViewModel(new BsGridRepositorySettings<ContributorSearchModel>
                    {
                        Page = 1,
                        PageSize = 10,
                        Search = new ContributorSearchModel
                        {
                            RolesFilter = new List<ProjectRole>() { ProjectRole.Developer, ProjectRole.TeamLeader }
                        }
                    }),
                    Search = repo.GetSearchForm(null),
                    New = repo.GetNewForm()
                },

                Testers = new ContributorsInheritExample
                {
                    Grid = repo.ToBsGridViewModel(new BsGridRepositorySettings<ContributorSearchModel>
                    {
                        Page = 1,
                        PageSize = 10,
                        Search = new ContributorSearchModel
                        {
                            RolesFilter = new List<ProjectRole>() { ProjectRole.Tester }
                        }
                    }),
                    Search = repo.GetSearchForm(null)
                },

                BFormsProject = new BsEditorGroupModel<ContributorsGroupRowModel, ContributorsRowFormModel>
                {
                    Items = new List<ContributorsGroupRowModel>()
                    {
                        new ContributorsGroupRowModel
                        {
                            Id = 1,
                            Name = "Stefan P.",
                            TabId = ContributorType.Developer,
                            Contributions = "concept, api design, razor helpers, documentation, c# bug fixing, testing",
                            Form = new ContributorsRowFormModel()
                            {
                                Contributions = "concept, api design, razor helpers, documentation, c# bug fixing, testing"
                            }
                        },
                        new ContributorsGroupRowModel
                        {
                            Id = 6,
                            Name = "Oana M.",
                            TabId = ContributorType.Developer,
                            Contributions = "UI & UX, css master",
                            Form = new ContributorsRowFormModel()
                            {
                                Contributions = "UI & UX, css master"
                            }
                        },
                         new ContributorsGroupRowModel
                        {
                            Id = 3,
                            Name = "Cezar C.",
                            TabId = ContributorType.Developer,
                            Contributions = "documentation, razor helpers",
                            Form = new ContributorsRowFormModel()
                            {
                                Contributions = "documentation, razor helpers"
                            }
                        },
                        new ContributorsGroupRowModel
                        {
                            Id = 4,
                            Name = "Marius C.",
                            TabId = ContributorType.Developer,
                            Contributions = "js framework, datetime picker, automated tests for js",
                            Form = new ContributorsRowFormModel()
                            {
                                Contributions = "js framework, datetime picker, automated tests for js"
                            }
                        }
                    },
                    Form = new ContributorsRowFormModel()
                },

                RequireJsProject = new BsEditorGroupModel<ContributorsGroupRowModel>
                {
                    Items = new List<ContributorsGroupRowModel>()
                    {
                        new ContributorsGroupRowModel
                        {
                            Id = 1,
                            Name = "Stefan P.",
                            Contributions = "concept, api design, razor helpers, documentation, c# bug fixing, testing",
                            TabId = ContributorType.Developer
                        },
                        new ContributorsGroupRowModel
                        {
                            Id = 3,
                            Name = "Cezar C.",
                            Contributions = "documentation, razor helpers",
                            TabId = ContributorType.Developer
                        }
                    }
                },
                Form = new GroupFormModel()
            };

            var viewModel = new GroupEditorViewModel
            {
                Editor = model
            };

            var options = new
            {
                getTabUrl = Url.Action("GetTab"),
                save = Url.Action("Save"),
                advancedSearchUrl = Url.Action("Search"),
                addUrl = Url.Action("New"),
                contributorType = RequireJsHtmlHelpers.ToJsonDictionary<ContributorType>(),
                projectRole = RequireJsHtmlHelpers.ToJsonDictionary<ProjectRole>()
            };

            RequireJsOptions.Add("index", options);

            return View(viewModel);
        }
        #endregion

        #region Ajax
        [HttpPost]
        public BsJsonResult GetTab(BsEditorRepositorySettings<ContributorType> settings)
        {
            var msg = string.Empty;
            var status = BsResponseStatus.Success;
            var html = string.Empty;
            var count = 0;

            try
            {
                html = RenderTab(settings, out count);
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

        [HttpPost]
        public BsJsonResult Save(GroupEditorModel model)
        {
            var errorMessage = "This is how a server error is displayed in group editor";

            return new BsJsonResult(new
            {
                Message = errorMessage
            },BsResponseStatus.ValidationError, "Server error");
        }

        [HttpPost]
        public BsJsonResult Search(ContributorSearchModel model, ContributorType tabId)
        {
            var settings = new BsEditorRepositorySettings<ContributorType>
            {
                Search = model,
                TabId = tabId
            };

            var count = 0;

            var html = this.RenderTab(settings, out count);

            return new BsJsonResult(new
            {
                Count = count,
                Html = html
            });
        }

        [HttpPost]
        public BsJsonResult New(ContributorNewModel model, ContributorType tabId)
        {
            var status = BsResponseStatus.Success;
            var row = string.Empty;
            var msg = string.Empty;

            try
            {
                if (ModelState.IsValid)
                {
                    var rowModel = repo.Create(model);

                    var groupEditorModel = new GroupEditorModel
                    {
                        Developers = new BsEditorTabModel<ContributorRowModel, ContributorSearchModel, ContributorNewModel>
                        {
                            Grid = new BsGridModel<ContributorRowModel>
                            {
                                Items = new List<ContributorRowModel>
                                {
                                    rowModel
                                }
                            }
                        }
                    };

                    var viewModel = new GroupEditorViewModel()
                    {
                        Editor = groupEditorModel
                    };

                    row = this.BsRenderPartialView("_Editors", viewModel);

                }
                else
                {
                    return new BsJsonResult(
                        new Dictionary<string, object> { { "Errors", ModelState.GetErrors() } },
                        BsResponseStatus.ValidationError);
                }
            }
            catch (Exception ex)
            {
                msg = Resource.ServerError;
                status = BsResponseStatus.ServerError;
            }

            return new BsJsonResult(new
            {
                Row = row
            }, status, msg);
        }
        #endregion

        #region Helpers
        [NonAction]
        public string RenderTab(BsEditorRepositorySettings<ContributorType> settings, out int count)
        {
            var html = string.Empty;
            count = 0;

            GroupEditorModel model = new GroupEditorModel();

            if (settings.Search == null)
            {
                settings.Search = new ContributorSearchModel();
            }

            switch (settings.TabId)
            {
                case ContributorType.Developer:

                    ((ContributorSearchModel)settings.Search).RolesFilter = new List<ProjectRole>() { ProjectRole.Developer, ProjectRole.TeamLeader };
                    
                    var gridDevelopers = repo.ToBsGridViewModel(settings.ToGridRepositorySettings<ContributorSearchModel>(), out count);
                   
                    model.Developers = new BsEditorTabModel<ContributorRowModel, ContributorSearchModel, ContributorNewModel>
                    {
                        Grid = gridDevelopers
                    };
                    
                    break;

                case ContributorType.Tester:

                    ((ContributorSearchModel)settings.Search).RolesFilter = new List<ProjectRole>() { ProjectRole.Tester };

                    var gridTesters = repo.ToBsGridViewModel(settings.ToGridRepositorySettings<ContributorSearchModel>(), out count);

                    model.Testers = new ContributorsInheritExample
                    {
                        Grid = gridTesters
                    };
                    break;
            }

            var viewModel = new GroupEditorViewModel()
            {
                Editor = model
            };

            html = this.BsRenderPartialView("_Editors", viewModel);

            return html;
        }
        #endregion
    }
}