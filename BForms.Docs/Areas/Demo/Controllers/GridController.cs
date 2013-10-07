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

namespace BForms.Docs.Areas.Demo.Controllers
{
    public class GridController : BaseController
    {
        #region Properties and Constructor
        private readonly GridRepository _gridRepository;

        public GridController()
        {
            _gridRepository = new GridRepository(Db);
        }
        #endregion

        #region Pages
        public ActionResult Index()
        {
            var gridModel = _gridRepository.ToBsGridViewModel(new BsGridRepositorySettings<UsersSearchModel>
            {
                Page = 1,
                PageSize = 5
            });

            var model = new UsersViewModel
            {
                Grid = gridModel,
                Toolbar = new Toolbar<UsersSearchModel, UsersNewModel>
                {
                    Search = _gridRepository.GetSearchForm(),
                    New = _gridRepository.GetNewForm()
                }
            };

            var options = new Dictionary<string, string>
            {
                {"pagerUrl", Url.Action("Pager")},
                {"detailsUrl", Url.Action("Details")},
                {"getRowUrl", Url.Action("GetRow")},
                {"enableDisableUrl", Url.Action("EnableDisable")},
                {"newUrl", Url.Action("New")},
                {"updateUrl", Url.Action("Update")},
                {"deleteUrl", Url.Action("Delete")}
            };

            RequireJsOptions.Add("index", options);

            return View(model);
        }
        #endregion

        #region Ajax
        public BsJsonResult Pager(BsGridRepositorySettings<UsersSearchModel> model)
        {
            var msg = string.Empty;
            var status = BsResponseStatus.Success;
            var html = string.Empty;
            var count = 0;

            try
            {
                var viewModel = _gridRepository.ToBsGridViewModel<UsersViewModel>(x => x.Grid, model, out count);

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

        public BsJsonResult New(Toolbar<UsersSearchModel, UsersNewModel> model)
        {
            var msg = string.Empty;
            var status = BsResponseStatus.Success;
            var row = string.Empty;

            try
            {
                var rowModel = _gridRepository.Create(model.New);

                var viewModel = _gridRepository.ToBsGridViewModel<UsersViewModel>(x => x.Grid, rowModel);

                row = this.BsRenderPartialView("Grid/_Grid", viewModel);
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

        public BsJsonResult Update(UsersDetailsModel model, int objId)
        {
            var msg = string.Empty;
            var status = BsResponseStatus.Success;
            var html = string.Empty;

            try
            {
                var detailsModel = _gridRepository.Update(model, objId);

                detailsModel.Jobs = _gridRepository.GetJobsDropdown(detailsModel.IdJob);

                html = this.BsRenderPartialView("Grid/Details/_Readonly", detailsModel);
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

        public BsJsonResult GetRow(int objId, bool getDetails = false)
        {
            var msg = string.Empty;
            var status = BsResponseStatus.Success;
            var row = string.Empty;
            var details = string.Empty;

            try
            {
                var rowModel = _gridRepository.ReadRow(objId);

                var viewModel = _gridRepository.ToBsGridViewModel<UsersViewModel>(x => x.Grid, rowModel);

                row = this.BsRenderPartialView("Grid/_Grid", viewModel);

                if (getDetails)
                {
                    var detailsModel = _gridRepository.ReadDetails(objId);

                    detailsModel.Jobs = _gridRepository.GetJobsDropdown(detailsModel.IdJob);

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

                model.Jobs = _gridRepository.GetJobsDropdown(model.IdJob);

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

        public BsJsonResult Delete(int objId)
        {
            var msg = string.Empty;
            var status = BsResponseStatus.Success;

            try
            {
                _gridRepository.Delete(objId);
            }
            catch (Exception ex)
            {
                msg = Resource.ServerError;
                status = BsResponseStatus.ServerError;
            }

            return new BsJsonResult(null, status, msg);
        }

        public BsJsonResult EnableDisable(int objId)
        {
            var msg = string.Empty;
            var status = BsResponseStatus.Success;

            try
            {
                _gridRepository.EnableDisable(objId);
            }
            catch (Exception ex)
            {
                msg = Resource.ServerError;
                status = BsResponseStatus.ServerError;
            }

            return new BsJsonResult(null, status, msg);
        }
        #endregion
    }
}