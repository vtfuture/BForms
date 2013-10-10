using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BForms.Docs.Areas.Demo.Models;
using BForms.Docs.Areas.Demo.Repositories;
using BForms.Docs.Controllers;
using BForms.Docs.Helpers;
using BForms.Docs.Resources;
using BootstrapForms.Grid;
using BootstrapForms.Models;
using BootstrapForms.Mvc;
using RequireJS;

namespace BForms.Docs.Areas.Demo.Controllers
{
    public class ContributorsController : BaseController
    {
        #region Properties and Constructor
        private readonly ContributorsRepository _gridRepository;

        public ContributorsController()
        {
            _gridRepository = new ContributorsRepository(Db);
        }
        #endregion

        #region Pages
        public ActionResult Index()
        {
            var gridModel = _gridRepository.ToBsGridViewModel(new BsGridRepositorySettings<ContributorSearchModel>
            {
                Page = 1,
                PageSize = 5
            });

            var model = new ContributorsViewModel
            {
                Grid = gridModel,
                Toolbar = new BsToolbarModel<ContributorSearchModel, ContributorNewModel>
                {
                    Search = _gridRepository.GetSearchForm(),
                    New = _gridRepository.GetNewForm()
                }
            };

            var options = new Dictionary<string, object>
            {
                {"pagerUrl", Url.Action("Pager")},
                {"detailsUrl", Url.Action("Details")},
                {"getRowUrl", Url.Action("GetRow")},
                {"enableDisableUrl", Url.Action("EnableDisable")},
                {"newUrl", Url.Action("New")},
                {"updateUrl", Url.Action("Update")},
                {"deleteUrl", Url.Action("Delete")},
                {"editComponents", RequireJsHtmlHelpers.ToJsonDictionary<EditComponents>()}
            };

            RequireJsOptions.Add("index", options);

            return View(model);
        }
        #endregion

        #region Ajax
        public BsJsonResult Pager(BsGridRepositorySettings<ContributorSearchModel> model)
        {
            var msg = string.Empty;
            var status = BsResponseStatus.Success;
            var html = string.Empty;
            var count = 0;

            try
            {
                var viewModel = _gridRepository.ToBsGridViewModel<ContributorsViewModel>(x => x.Grid, model, out count);

                html = this.BsRenderPartialView("Grid/_Grid", viewModel);
            }
            catch (Exception ex)
            {
                msg = Resource.ServerError;
                status = BsResponseStatus.ServerError;
            }

            return new BsJsonResult(new
            {
                Count = count,
                Html = html
            }, status, msg);
        }

        public BsJsonResult New(BsToolbarModel<ContributorSearchModel, ContributorNewModel> model)
        {
            var msg = string.Empty;
            var status = BsResponseStatus.Success;
            var row = string.Empty;

            try
            {
                if (ModelState.IsValid)
                {
                    var rowModel = _gridRepository.Create(model.New);

                    var viewModel = _gridRepository.ToBsGridViewModel<ContributorsViewModel>(x => x.Grid, rowModel);

                    row = this.BsRenderPartialView("Grid/_Grid", viewModel);
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

        public BsJsonResult Update(ContributorDetailsModel model, int objId, EditComponents componentId)
        {
            var msg = string.Empty;
            var status = BsResponseStatus.Success;

            try
            {
                ClearModelState(ModelState, componentId);

                if (ModelState.IsValid)
                {
                    _gridRepository.Update(model, objId, componentId);
                }
            }
            catch (Exception ex)
            {
                msg = Resource.ServerError;
                status = BsResponseStatus.ServerError;
            }

            return new BsJsonResult(new
            {
            }, status, msg);
        }

        public BsJsonResult GetRow(int objId, bool getDetails = false)
        {
            var msg = string.Empty;
            var status = BsResponseStatus.Success;
            var row = string.Empty;
            var details = string.Empty;

            try
            {
                var rowModel = _gridRepository.ReadRow(objId);

                var viewModel = _gridRepository.ToBsGridViewModel<ContributorsViewModel>(x => x.Grid, rowModel);

                row = this.BsRenderPartialView("Grid/_Grid", viewModel);

                if (getDetails)
                {
                    var detailsModel = _gridRepository.ReadDetails(objId);

                    details = this.BsRenderPartialView("Grid/Details/_Index", detailsModel);
                }

            }
            catch (Exception ex)
            {
                msg = Resource.ServerError;
                status = BsResponseStatus.ServerError;
            }

            return new BsJsonResult(new
            {
                Row = row,
                Details = details
            }, status, msg);
        }

        public BsJsonResult Details(int objId)
        {
            var msg = string.Empty;
            var status = BsResponseStatus.Success;
            var html = string.Empty;

            try
            {
                var model = _gridRepository.ReadDetails(objId);

                html = this.BsRenderPartialView("Grid/Details/_Index", model);
            }
            catch (Exception ex)
            {
                msg = Resource.ServerError;
                status = BsResponseStatus.ServerError;
            }

            return new BsJsonResult(new
            {
                Html = html
            }, status, msg);
        }

        public BsJsonResult Delete(List<int> ids)
        {
            var msg = string.Empty;
            var status = BsResponseStatus.Success;

            try
            {
                foreach (var id in ids)
                {
                    _gridRepository.Delete(id);
                }
            }
            catch (Exception ex)
            {
                msg = Resource.ServerError;
                status = BsResponseStatus.ServerError;
            }

            return new BsJsonResult(null, status, msg);
        }

        public BsJsonResult EnableDisable(List<int> ids, bool? enable)
        {
            var msg = string.Empty;
            var status = BsResponseStatus.Success;

            try
            {
                foreach (var id in ids)
                {
                    _gridRepository.EnableDisable(id, enable);
                }
            }
            catch (Exception ex)
            {
                msg = Resource.ServerError;
                status = BsResponseStatus.ServerError;
            }

            return new BsJsonResult(null, status, msg);
        }
        #endregion

        #region Helpers
        [NonAction]
        public void ClearModelState(ModelStateDictionary ms, EditComponents componentId)
        {
            switch (componentId)
            {
                case EditComponents.Identity:
                    ms.ClearModelState(new List<string>() { "FirstName", "LastName", "Url", "CountriesList"});
                    break;
                case EditComponents.ProjectRelated:
                    ms.ClearModelState(new List<string>() { "RoleList", "StartDate", "LanguagesList", "Contributions" });
                    break;
            }
        }
        #endregion
    }
}