using System;
using System.Collections.Generic;
using System.Data.Common.EntitySql;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using BForms.Docs.Areas.Demo.Models;
using BForms.Docs.Areas.Demo.Repositories;
using BForms.Docs.Controllers;
using BForms.Docs.Resources;
using BForms.Grid;
using BForms.Models;
using BForms.Mvc;
using RequireJS;
using BForms.Docs.Areas.Demo.Helpers;

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
            var savedSettings = GetGridSettings();
            
            var bsGridSettings = new BsGridRepositorySettings<ContributorSearchModel>
            {
                Page = 1,
                PageSize = 5
            };

            if (savedSettings != null)
            {
                bsGridSettings.OrderableColumns = savedSettings.OrderableColumns;
                bsGridSettings.OrderColumns = savedSettings.OrderColumns;
                bsGridSettings.PageSize = savedSettings.PageSize;
            }

            var gridModel = _gridRepository.ToBsGridViewModel(bsGridSettings);

            var model = new ContributorsViewModel
            {
                Grid = gridModel,
                Toolbar = new BsToolbarModel<ContributorSearchModel, ContributorNewModel, List<ContributorOrderModel>>
                {
                    Search = _gridRepository.GetSearchForm(),
                    New = _gridRepository.GetNewForm(),
                    Order = _gridRepository.GetOrderForm(true)
                }
            };

            var options = new Dictionary<string, object>
            {
                {"pagerUrl", Url.Action("Pager")},
                {"getRowsUrl", Url.Action("GetRows")},
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
        public BsJsonResult Pager(BsGridRepositorySettings<ContributorSearchModel> settings)
        {
            var msg = string.Empty;
            var status = BsResponseStatus.Success;
            var html = string.Empty;
            var count = 0;

            try
            {
                //simulate exception
                if (settings.Page == 3)
                {
                    throw new Exception("This is how an exception message is displayed in grid header");
                }

                SaveGridSettings(settings);

                var viewModel = _gridRepository.ToBsGridViewModel(settings, out count).Wrap<ContributorsViewModel>(x => x.Grid);

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

                    var viewModel = _gridRepository.ToBsGridViewModel(rowModel).Wrap<ContributorsViewModel>(x => x.Grid);

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

        public BsJsonResult GetEditable(int objId, EditComponents componentId)
        {
            var html = string.Empty;
            var model = _gridRepository.ReadEditable(objId, componentId);

            switch (componentId)
            {
                case EditComponents.Identity:
                    html = this.BsRenderPartialView("Grid/Details/_IdentityEditable", model.Identity, "x" + objId + "." + model.GetPropertyName(x => x.Identity));
                    break;
                case EditComponents.ProjectRelated:
                    html = this.BsRenderPartialView("Grid/Details/_ProjectRelatedEditable", model.ProjectRelated, "x" + objId + "." + model.GetPropertyName(x => x.ProjectRelated));
                    break;
            }

            return new BsJsonResult(new
            {
                Html = html
            });
        }

        public BsJsonResult Update(ContributorDetailsEditable model, int objId, EditComponents componentId)
        {
            var msg = string.Empty;
            var status = BsResponseStatus.Success;
            var html = string.Empty;
            var rowsHtml = string.Empty;

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
                            html = this.BsRenderPartialView("Grid/Details/_IdentityReadonly", detailsModel.Identity);
                            break;
                        case EditComponents.ProjectRelated:
                            html = this.BsRenderPartialView("Grid/Details/_ProjectRelatedReadonly", detailsModel.ProjectRelated);
                            break;
                    }

                    var rowModel = _gridRepository.ReadRow(objId);

                    var viewModel = _gridRepository.ToBsGridViewModel(rowModel, true).Wrap<ContributorsViewModel>(x => x.Grid);

                    rowsHtml = this.BsRenderPartialView("Grid/_Grid", viewModel);
                }
            }
            catch (Exception ex)
            {
                msg = "<strong>" + Resource.ServerError + "!</strong> " + ex.Message;
                status = BsResponseStatus.ServerError;
            }

            return new BsJsonResult(new
            {
                Html = html,
                RowsHtml = rowsHtml
            }, status, msg);
        }

        public BsJsonResult Reorder(List<ContributorOrderModel> model)
        {
            var status = BsResponseStatus.Success;
            var msg = string.Empty;

            try
            {
                _gridRepository.Reorder(model);

                var contributors = Db.Contributors.ToList();
            }
            catch (Exception ex)
            {
                msg = "<strong>" + Resource.ServerError + "!</strong> " + ex.Message;
                status = BsResponseStatus.ServerError;
            }

            return new BsJsonResult(null, status, msg);
        }

        public BsJsonResult GetRows(List<BsGridRowData<int>> items)
        {
            var msg = string.Empty;
            var status = BsResponseStatus.Success;
            var rowsHtml = string.Empty;

            try
            {
                rowsHtml = GetRowsHtml(items);
            }
            catch (Exception ex)
            {
                msg = "<strong>" + Resource.ServerError + "!</strong> " + ex.Message;
                status = BsResponseStatus.ServerError;
            }

            return new BsJsonResult(new
            {
                RowsHtml = rowsHtml
            }, status, msg);
        }

        public BsJsonResult Delete(List<BsGridRowData<int>> items)
        {
            var msg = string.Empty;
            var status = BsResponseStatus.Success;

            try
            {
                foreach (var item in items)
                {
                    //simulate exception
                    if (item.Id == 3)
                    {
                        throw new Exception("This is how an exception message is displayed when it's triggered on row control");
                    }

                    _gridRepository.Delete(item.Id);
                }
            }
            catch (Exception ex)
            {
                msg = "<strong>" + Resource.ServerError + "!</strong> " + ex.Message;
                status = BsResponseStatus.ServerError;
            }

            return new BsJsonResult(null, status, msg);
        }

        public BsJsonResult EnableDisable(List<BsGridRowData<int>> items, bool? enable)
        {
            Thread.Sleep(1000);
            var msg = string.Empty;
            var status = BsResponseStatus.Success;
            var rowsHtml = string.Empty;

            try
            {
                foreach (var item in items)
                {
                    //simulate exception
                    if (item.Id == 2)
                    {
                        throw new Exception("This is how an exception message is displayed when it's triggered on row control");
                    }

                    _gridRepository.EnableDisable(item.Id, enable);
                }

                rowsHtml = GetRowsHtml(items);
            }
            catch (Exception ex)
            {
                msg = "<strong>" + Resource.ServerError + "!</strong> " + ex.Message;
                status = BsResponseStatus.ServerError;
            }

            return new BsJsonResult(new
            {
                RowsHtml = rowsHtml
            }, status, msg);
        }

        public ActionResult ExportExcel(BsGridRepositorySettings<ContributorSearchModel> settings, List<BsGridRowData<int>> items)
        {
            var rows = _gridRepository.GetItems(settings, items.Select(x => x.Id).ToList());

            try
            {
                var builder = new BsGridExcelBuilder<ContributorRowModel>("BForms Contributors.xlsx", rows);

                builder.ConfigureHeader(header =>
                        {
                            header.Order(settings.OrderColumns);
                            header.Style.Font.Bold = true;
                            header.Style.FillColor = BsGridExcelColor.Ivory;
                            header.For(x => x.StartDate)
                                  .Text("Contributor since")
                                  .Style(style => style.Font.Italic = true);
                        })
                       .ConfigureRows((row, style) =>
                        {
                            if (row.Role == ProjectRole.TeamLeader)
                            {
                                style.Font.Bold = true;
                            }
                            if (row.Role == ProjectRole.Tester)
                            {
                                style.Font.Italic = true;
                            }
                        })
                        .ConfigureColumns(columns =>
                        {
                            columns.For(x => x.Name)
                                   .Order(1);
                            columns.For(x => x.Enabled)
                                   .Text(x => x.Enabled ? Resource.Yes : Resource.No)
                                   .Style((row, style) => style.FillColor = row.Enabled ? BsGridExcelColor.LightGreen : BsGridExcelColor.Red);
                            columns.For(x => x.Role)
                                   .Order(2)
                                   .Text(x => x.Role.ToString())
                                   .Style(style => style.FillColor = BsGridExcelColor.Lavender);
                            columns.For(x => x.StartDate)
                                   .Text(x => x.StartDate.ToMonthNameDate())
                                   .Style(style => style.Font.Italic = true);
                        });

                return new BsExcelResult<ContributorRowModel>("BForms Contributors.xlsx", builder);
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
        private string GetRowsHtml(List<BsGridRowData<int>> items)
        {
            var ids = items.Select(x => x.Id).ToList();
            var rowsModel = _gridRepository.ReadRows(items.Select(x => x.Id).ToList());
            var viewModel = _gridRepository.ToBsGridViewModel(rowsModel, row => row.Id, items, x => ids.Contains(x.Id))
                    .Wrap<ContributorsViewModel>(x => x.Grid);

            var savedSettings = GetGridSettings();

            if (savedSettings != null)
            {
                viewModel.Grid.BaseSettings.OrderableColumns = savedSettings.OrderableColumns;
                viewModel.Grid.BaseSettings.OrderColumns = savedSettings.OrderColumns;
                viewModel.Grid.BaseSettings.PageSize = savedSettings.PageSize;
            }

            return this.BsRenderPartialView("Grid/_Grid", viewModel);
        }

        [NonAction]
        public void ClearModelState(ModelStateDictionary ms, EditComponents componentId)
        {
            var model = new ContributorDetailsEditable();

            switch (componentId)
            {
                case EditComponents.Identity:
                    ms.ClearModelState(model.GetPropertyName(m => m.Identity) + ".");
                    break;

                case EditComponents.ProjectRelated:
                    ms.ClearModelState(model.GetPropertyName(m => m.ProjectRelated) + ".");
                    break;
            }
        }

        [NonAction]
        public void SaveGridSettings(BsGridRepositorySettings<ContributorSearchModel> settings)
        {
            if (settings.OrderColumns != null)
            {
                Session["GridSettings"] = new BsGridSavedSettings
                {
                    PageSize = settings.PageSize,
                    OrderableColumns = settings.OrderableColumns,
                    OrderColumns = settings.OrderColumns
                };
            }
        }

        [NonAction]
        public BsGridSavedSettings GetGridSettings()
        {
            return Session["GridSettings"] as BsGridSavedSettings;
        }

        [Serializable]
        public class BsGridSavedSettings
        {
            public int PageSize { get; set; }

            public List<BsColumnOrder> OrderableColumns { get; set; } // order grid by column

            public Dictionary<string, int>  OrderColumns{ get; set; } // swap columns order
        }
        #endregion
    }
}