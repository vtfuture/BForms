using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BForms.Grid;
using BForms.Models;
using BForms.Mvc;
using MyGrid.Models;
using MyGrid.Repositories;
using RequireJS;

namespace MyGrid.Controllers
{
    public class HomeController : BaseController
    {
        #region Properties and Constructor
        private readonly MoviesGridRepository _gridRepository;

        public HomeController()
        {
            _gridRepository = new MoviesGridRepository(Db);
        }
        #endregion

        public ActionResult Index()
        {
            var bsGridSettings = new BsGridBaseRepositorySettings
            {
                Page = 1,
                PageSize = 5
            };

            var savedSettings = GetGridSettings();

            if (savedSettings != null)
            {
                bsGridSettings.OrderableColumns = savedSettings.OrderableColumns;
                bsGridSettings.OrderColumns = savedSettings.OrderColumns;
                bsGridSettings.PageSize = savedSettings.PageSize;
            }

            var gridModel = _gridRepository.ToBsGridViewModel(bsGridSettings);

            var model = new MoviesViewModel
            {
                Grid = gridModel,
                Toolbar = new BsToolbarModel<MoviesSearchModel, MoviesNewModel>
                {
                    Search = _gridRepository.GetSearchForm(),
                    New = _gridRepository.GetNewForm()
                }
            };

            var options = new Dictionary<string, object>
            {
                {"pagerUrl", Url.Action("Pager")},
                {"getRowsUrl", Url.Action("GetRows")},
                {"recommendUnrecommendUrl", Url.Action("RecommendUnrecommend")},
                {"updateUrl", Url.Action("Update")},
                {"deleteUrl", Url.Action("Delete")},
                {"editComponents", RequireJsHtmlHelpers.ToJsonDictionary<EditComponents>()}
            };

            RequireJsOptions.Add("index", options);

            return View(model);
        }

        #region Ajax
        public BsJsonResult Pager(BsGridRepositorySettings<MoviesSearchModel> settings)
        {
            var msg = string.Empty;
            var status = BsResponseStatus.Success;
            var html = string.Empty;
            var count = 0;

            try
            {
                SaveGridSettings(settings);

                var viewModel = _gridRepository.ToBsGridViewModel(settings, out count).Wrap<MoviesViewModel>(x => x.Grid);

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
                msg = "<strong>Server Error!</strong> " + ex.Message;
                status = BsResponseStatus.ServerError;
            }

            return new BsJsonResult(new
            {
                RowsHtml = rowsHtml
            }, status, msg);
        }

        public BsJsonResult New(BsToolbarModel<MoviesSearchModel, MoviesNewModel> model)
        {
            var msg = string.Empty;
            var status = BsResponseStatus.Success;
            var row = string.Empty;

            try
            {
                if (ModelState.IsValid)
                {
                    var rowModel = _gridRepository.Create(model.New);

                    var viewModel = _gridRepository.ToBsGridViewModel(rowModel).Wrap<MoviesViewModel>(x => x.Grid);

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
                msg = "Server Error";
                status = BsResponseStatus.ServerError;
            }

            return new BsJsonResult(new
            {
                Row = row
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
                    _gridRepository.Delete(item.Id);
                }
            }
            catch (Exception ex)
            {
                msg = "<strong>Server Error!</strong> " + ex.Message;
                status = BsResponseStatus.ServerError;
            }

            return new BsJsonResult(null, status, msg);
        }

        public BsJsonResult Update(MovieDetailsModel model, int objId, EditComponents componentId)
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


                    switch (componentId)
                    {
                        case EditComponents.Info:
                            html = this.BsRenderPartialView("Grid/Details/_InfoReadonly", detailsModel);
                            break;
                        case EditComponents.Revenue:
                            html = this.BsRenderPartialView("Grid/Details/_RevenueReadonly", detailsModel);
                            break;
                    }

                    var rowModel = _gridRepository.ReadRow(objId);

                    var viewModel = _gridRepository.ToBsGridViewModel(rowModel, true).Wrap<MoviesViewModel>(x => x.Grid);

                    html = this.BsRenderPartialView("Grid/_Grid", viewModel);
                }
            }
            catch (Exception ex)
            {
                msg = "<strong>Server Error!</strong> " + ex.Message;
                status = BsResponseStatus.ServerError;
            }
            

            return new BsJsonResult(new
            {
                RowsHtml = html
            }, status, msg);
        }

        public BsJsonResult RecommendUnrecommend(List<BsGridRowData<int>> items, bool? recommended)
        {
            var msg = string.Empty;
            var status = BsResponseStatus.Success;
            var rowsHtml = string.Empty;

            try
            {
                foreach (var item in items)
                {
                    _gridRepository.RecommendUnrecommend(item.Id, recommended);
                }

                rowsHtml = GetRowsHtml(items);
            }
            catch (Exception ex)
            {
                msg = "<strong>Server Error!</strong> " + ex.Message;
                status = BsResponseStatus.ServerError;
            }

            return new BsJsonResult(new
            {
                RowsHtml = rowsHtml
            }, status, msg);
        }



        [NonAction]
        private string GetRowsHtml(List<BsGridRowData<int>> items)
        {
            var ids = items.Select(x => x.Id).ToList();
            var rowsModel = _gridRepository.ReadRows(ids);
            var viewModel = _gridRepository.ToBsGridViewModel(rowsModel, row => row.Id, items).Wrap<MoviesViewModel>(x => x.Grid);

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
            switch (componentId)
            {
                case EditComponents.Info:
                    ms.ClearModelState(new List<string>() { "Title", "Poster", "Rating", "GenresList" });
                    break;
                case EditComponents.Revenue:
                    ms.ClearModelState(new List<string>() { "GrossRevenue", "WeekendRevenue", "ReleaseDate" });
                    break;
            }
        }

        [NonAction]
        public void SaveGridSettings(BsGridRepositorySettings<MoviesSearchModel> settings)
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

            public List<BsColumnOrder> OrderableColumns { get; set; }

            public Dictionary<string, int> OrderColumns { get; set; }
        }

        #endregion

    }
}
