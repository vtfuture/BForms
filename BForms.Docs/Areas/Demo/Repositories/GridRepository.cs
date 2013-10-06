using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BForms.Docs.Areas.Demo.Mock;
using BForms.Docs.Areas.Demo.Models;
using BootstrapForms.Grid;

namespace BForms.Docs.Areas.Demo.Repositories
{
    public class GridRepository : BsBaseGridModelBuilder<User, UsersGridRowModel, UsersSearchModel>
    {
        #region Properties and Constructor
        private BFormsContext db;

        public class GridSettings : BsGridRepositorySettings<UsersSearchModel>
        {
            public int? OrganizationId { get; set; }
        }

        public BsGridRepositorySettings<UsersSearchModel> Settings
        {
            get
            {
                return settings;
            }
        }

        public GridRepository(BFormsContext _db)
        {
            db = _db;
        }
        #endregion

        #region Mappers
        public Func<User, UsersGridRowModel> MapUser_UserGridRowModel = x => 
            new UsersGridRowModel
            {
                Id = x.Id,
                Enabled = x.Enabled,
                Name = x.FirstName + " " + x.LastName,
                RegisterDate = x.RegisterDate
            };
        #endregion

        public override IQueryable<User> Query()
        {
            return db.Users.AsQueryable();
        }

        public override IOrderedQueryable<User> OrderQuery(IQueryable<User> query)
        {
            return query.OrderBy(x => x.Id);
        }

        public override IEnumerable<UsersGridRowModel> MapQuery(IQueryable<User> query)
        {
            return query.Select(MapUser_UserGridRowModel);
        }
    }
}