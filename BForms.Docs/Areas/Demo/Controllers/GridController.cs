using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BForms.Docs.Areas.Demo.Models;
using BForms.Docs.Areas.Demo.Repositories;
using BForms.Docs.Controllers;
using BootstrapForms.Grid;

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

            return View(model);
        }
        #endregion

        #region Ajax

        #endregion
    }
}