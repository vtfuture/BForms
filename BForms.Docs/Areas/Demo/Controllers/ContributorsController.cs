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
using BForms.Grid;
using BForms.Models;
using BForms.Mvc;
using RequireJS;
using BForms.Utilities;
using System.Drawing;

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
            //test comment
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
                {"exportExcelUrl", Url.Action("ExportExcel")},
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
                //simulate exception
                if (model.Page == 3)
                {
                    throw new Exception("This is how an exception message is displayed in grid header");
                }

                var viewModel = _gridRepository.ToBsGridViewModel<ContributorsViewModel>(x => x.Grid, model, out count);

                html = this.BsRenderPartialView("Grid/_Grid", viewModel);
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
            var html = string.Empty;

            try
            {
                ClearModelState(ModelState, componentId);

                if (ModelState.IsValid)
                {
                    var detailsModel = _gridRepository.Update(model, objId, componentId);

                    //simulate exception
                    if (objId == 4)
                    {
                        throw new Exception("This is how an exception message is displayed inside row details");
                    }

                    switch (componentId)
                    {
                        case EditComponents.Identity:
                            html = this.BsRenderPartialView("Grid/Details/_IdentityReadonly", detailsModel);
                            break;
                        case EditComponents.ProjectRelated:
                            html = this.BsRenderPartialView("Grid/Details/_ProjectRelatedReadonly", detailsModel);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                msg = "<strong>" + Resource.ServerError + "!</strong> " + ex.Message;
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
                //simulate exception
                if (objId == 2)
                {
                    throw new Exception("This is how an exception message is displayed inside a BFroms grid row");
                }

                //render row details
                var model = _gridRepository.ReadDetails(objId);
                html = this.BsRenderPartialView("Grid/Details/_Index", model);

            }
            catch (Exception ex)
            {
                msg = "<strong>" + Resource.ServerError + "!</strong> " + ex.Message;
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
                    //simulate exception
                    if (id == 3)
                    {
                        throw new Exception("This is how an exception message is displayed when it's triggered on row control");
                    }

                    _gridRepository.Delete(id);
                }
            }
            catch (Exception ex)
            {
                msg = "<strong>" + Resource.ServerError + "!</strong> " + ex.Message;
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
                    //simulate exception
                    if (id == 2)
                    {
                        throw new Exception("This is how an exception message is displayed when it's triggered on row control");
                    }

                    _gridRepository.EnableDisable(id, enable);
                }
            }
            catch (Exception ex)
            {
                msg = "<strong>" + Resource.ServerError + "!</strong> " + ex.Message;
                status = BsResponseStatus.ServerError;
            }

            return new BsJsonResult(null, status, msg);
        }

        public ActionResult ExportExcel(BsGridRepositorySettings<ContributorSearchModel> settings, List<int> ids)
        {
            var items = _gridRepository.GetExcelItems(settings, ids);

            try
            {
                var builder = new BsGridExcelBuilder<ContributorRowExcelModel>("BForms Contributors.xlsx", items);

                builder.ConfigureHeader(header =>
                        {
                            header.Style.Font.Bold = true;
                            header.Style.FillColor = BsGridExcelColor.Ivory;
                            header.For(x => x.StartDate)
                                  .Text("Contributor since")
                                  .Style(style => style.Font.Italic = true);
                        })
                       .ConfigureRows((row, style) =>
                        {
                            if (row.Role == "TeamLeader")
                            {
                                style.Font.Bold = true;
                            }
                            if (row.Role == "Tester")
                            {
                                style.Font.Italic = true;
                            }
                        })
                        .ConfigureColumns(columns =>
                        {
                            columns.For(x => x.Enabled)
                                   .Text(x => x.Enabled ? Resource.Yes : Resource.No)
                                   .Style((row, style) => style.FillColor = row.Enabled ? BsGridExcelColor.LightGreen : BsGridExcelColor.Red);
                            columns.For(x => x.Role)
                                   .Style(style => style.FillColor = BsGridExcelColor.Lavender);
                            columns.For(x => x.StartDate)
                                   .Style(style => style.Font.Italic = true);
                        });

                return new BsExcelResult<ContributorRowExcelModel>("BForms Contributors.xlsx", builder);
            }
            catch (Exception ex)
            {
                var controllerName = (string)Request.RequestContext.RouteData.Values["controller"];
                var actionName = (string)Request.RequestContext.RouteData.Values["action"];

                return View("Error", new HandleErrorInfo(ex, controllerName, actionName));
            }
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
