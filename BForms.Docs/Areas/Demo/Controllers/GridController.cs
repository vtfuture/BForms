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
using BootstrapForms.Mvc;

namespace BForms.Docs.Areas.Demo.Controllers
{
    public class GridController : BaseController
    {
        #region Properties and Constructor
        private readonly GridRepository _gridRepository;

        public GridController()
        {
            _gridRepository = new GridRepository(db);
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
                Grid = gridModel
            };

            RequireJsOptions.Add("pagerUrl", Url.Action("Pager"));

            return View(model);
        }
        #endregion

        #region Ajax
        public JsonResult Pager(BsGridRepositorySettings<UsersSearchModel> model)
        {
            var msg = string.Empty;
            var msgToolTip = string.Empty;
            var status = StatusInfo.Success;
            var html = string.Empty;
            var count = 0;

            try
            {
                var gridModel = _gridRepository.ToBsGridViewModel(model);

                count = gridModel.Pager.TotalRecords;

                html = this.BsRenderPartialView("_Grid", new UsersViewModel
                {
                    Grid = gridModel
                });
            }
            catch (Exception ex)
            {
                msg = Resource.ServerError;
                status = StatusInfo.ServerError;
            }

            return Json(new
            {
                Status = status,
                Message = msg,
                MessageToolTip = msgToolTip,
                Data = new
                {
                    Count = count,
                    Html = html
                }
            });
        }
        #endregion
    }
}