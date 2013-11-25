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
using BForms.Editor;
using BForms.Docs.Resources;

namespace BForms.Docs.Areas.Demo.Controllers
{
    public class ContributorsGroupRowModel : BsEditorGroupItemModel<ContributorsRowFormModel>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override object GetUniqueID()
        {
            return this.Id;
        }
    }

    public class ContributorsRowFormModel
    {
        [Display(Name = "Name", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.TextBox)]
        public string Name { get; set; }

        [Display(Name = "Location", Prompt = "PromptLocation", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.DropDownList)]
        public BsSelectList<string> CountriesList { get; set; }
    }

    public class ContributorsOrderModel {}

    public class ContributorsInheritExample : BsEditorTabModel<ContributorRowModel, ContributorSearchModel>
    {
        public ContributorsOrderModel Order { get; set; }
    }

    public class GroupEditorModel
    {
        [BsEditorTab(Name = "Contributors1", Id = YesNoValueTypes.Yes, Selected = true)]
        public ContributorsInheritExample Contributors { get; set; }

        [BsEditorTab(Name = "Contributors2", Id = YesNoValueTypes.No, Selected = false)]
        public BsEditorTabModel<ContributorRowModel> Contributors2 { get; set; }

        [BsEditorTab(Name = "Contributors3", Id = YesNoValueTypes.Both, Selected = false)]
        public BsEditorTabModel<ContributorRowModel, ContributorSearchModel, ContributorNewModel> Contributors3 { get; set; }

        [BsEditorGroup(Id = NotificationType.Daily)]
        public BsEditorGroupModel<ContributorsGroupRowModel, ContributorsRowFormModel> Group1 { get; set; }

        [BsEditorGroup(Id = NotificationType.Monthly)]
        public BsEditorGroupModel<ContributorsGroupRowModel> Group2 { get; set; }
    }

    public class GroupEditorViewModel
    {
        public GroupEditorModel Editor1 { get; set; }
        public GroupEditorModel Editor2 { get; set; }
    }

    public class GroupEditorController : BaseController
    {
        private readonly ContributorsRepository repo;

        public GroupEditorController()
        {
            repo = new ContributorsRepository(Db);
        }

        //
        // GET: /Demo/UserGroup/
        public ActionResult Index()
        {
            var model = new GroupEditorModel()
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

                Contributors3 = new BsEditorTabModel<ContributorRowModel, ContributorSearchModel, ContributorNewModel>
                {
                    Grid = repo.ToBsGridViewModel(new BsGridRepositorySettings<ContributorSearchModel>
                    {
                        Page = 1,
                        PageSize = 5
                    }),
                    Search = repo.GetSearchForm(),
                    New = repo.GetNewForm()
                },

                Group1 = new BsEditorGroupModel<ContributorsGroupRowModel, ContributorsRowFormModel>
                {
                    Items = new List<ContributorsGroupRowModel>()
                    {
                        new ContributorsGroupRowModel
                        {
                            Id = 4,
                            Name = "Marius C.",
                            TabId = YesNoValueTypes.Yes,
                            Form = new ContributorsRowFormModel()
                            {
                                Name = "Marius C."
                            }
                        },
                        new ContributorsGroupRowModel
                        {
                            Id = 2,
                            Name = "Ciprian P.",
                            TabId = YesNoValueTypes.Yes,
                            Form = new ContributorsRowFormModel()
                            {
                                Name = "Ciprian P."
                            }
                        }
                    },
                    Form = new ContributorsRowFormModel()
                },

                Group2 = new BsEditorGroupModel<ContributorsGroupRowModel>
                {
                    Items = new List<ContributorsGroupRowModel>()
                    {
                        new ContributorsGroupRowModel
                        {
                            Id = 4,
                            Name = "Marius C.",
                            TabId = YesNoValueTypes.Yes
                        }
                    }
                }
            };

            var viewModel = new GroupEditorViewModel
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

        public BsJsonResult GetTab(BsEditorRepositorySettings<YesNoValueTypes> settings)
        {
            var msg = string.Empty;
            var status = BsResponseStatus.Success;
            var html = string.Empty;
            var count = 0;

            try
            {
                GroupEditorModel model = new GroupEditorModel();
                
                switch (settings.TabId)
                {
                    case YesNoValueTypes.No:

                        var grid2 = repo.ToBsGridViewModel(settings.ToBaseGridRepositorySettings(), out count);

                        model.Contributors2 = new BsEditorTabModel<ContributorRowModel>
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

                        model.Contributors3 = new BsEditorTabModel<ContributorRowModel, ContributorSearchModel, ContributorNewModel>
                        {
                            Grid = grid3
                        };
                        break;
                }

                var viewModel = new GroupEditorViewModel()
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