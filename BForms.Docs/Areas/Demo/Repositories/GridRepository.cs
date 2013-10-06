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
            var query = db.Users.AsQueryable();

            return Filter(query);
        }

        public override IOrderedQueryable<User> OrderQuery(IQueryable<User> query)
        {
            return query.OrderBy(x => x.FirstName + " " + x.LastName);
        }

        public override IEnumerable<UsersGridRowModel> MapQuery(IQueryable<User> query)
        {
            return query.Select(MapUser_UserGridRowModel);
        }

        public IQueryable<User> Filter(IQueryable<User> query)
        {
            if (Settings.Search != null)
            {
                #region Register Date
                if (Settings.Search.RegisterDate != null)
                {
                    if (Settings.Search.RegisterDate.From.HasValue)
                    {
                        var fromDate = Settings.Search.RegisterDate.From.Value;

                        query = query.Where(x => x.RegisterDate >= fromDate);
                    }
                    if (Settings.Search.RegisterDate.To.HasValue)
                    {
                        var toDate = Settings.Search.RegisterDate.To.Value;

                        query = query.Where(x => x.RegisterDate >= toDate);
                    }
                }
                #endregion

                #region Name
                if (!string.IsNullOrEmpty(Settings.Search.Name))
                {
                    query = query.Where(x => (x.FirstName + " " + x.LastName).Contains(Settings.Search.Name));
                }
                #endregion

                #region Enabled
                if (Settings.Search.IsEnabled.SelectedValues.HasValue)
                {
                    var enabled = Settings.Search.IsEnabled.SelectedValues.Value == YesNoValueTypes.Yes;

                    query = query.Where(x => x.Enabled == enabled);
                }
                #endregion
            }

            return query;
        } 
    }
}