using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BForms.Docs.Areas.Demo.Mock;
using BForms.Docs.Areas.Demo.Models;
using BForms.Docs.Resources;
using BootstrapForms.Grid;
using BootstrapForms.Models;

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

        public Func<User, UsersDetailsModel> MapUser_UsersDetailsModel = x =>
            new UsersDetailsModel
            {
                Id = x.Id,
                Job = x.Job != null ? x.Job.Name : Resource.Unemployed,
                Enabled = x.Enabled
            };
        #endregion

        #region Filter/Order/Map
        public override IQueryable<User> Query()
        {
            var query = db.Users.AsQueryable();

            return Filter(query);
        }

        public override IOrderedQueryable<User> OrderQuery(IQueryable<User> query)
        {
            this.orderedQueryBuilder.OrderFor(x => x.Name, y => y.FirstName + " " + y.LastName);
            var orderedQuery = this.orderedQueryBuilder.Order(query, x => x.Id, BsOrderType.Descending);
            return orderedQuery;
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
        #endregion

        #region CRUD
        public UsersDetailsModel ReadDetails(int objId)
        {
            return db.Users.Where(x => x.Id == objId).Select(MapUser_UsersDetailsModel).FirstOrDefault();
        }

        public UsersGridRowModel ReadRow(int objId)
        {
            return db.Users.Where(x => x.Id == objId).Select(MapUser_UserGridRowModel).FirstOrDefault();
        }

        public void Delete(int objId)
        {
            var entity = db.Users.FirstOrDefault(x => x.Id == objId);

            if (entity != null)
            {
                db.Users.Remove(entity);
                db.SaveChanges();
            }
        }
        #endregion

        #region Helpers
        public UsersSearchModel GetSearchForm()
        {
            return new UsersSearchModel()
            {
                Jobs = GetJobsDropdown()
            };
        }

        public UsersNewModel GetNewForm()
        {
            return new UsersNewModel()
            {
                Jobs = GetJobsDropdown()
            };
        }

        public void EnableDisable(int objId)
        {
            var entity = db.Users.FirstOrDefault(x => x.Id == objId);

            if (entity != null)
            {
                entity.Enabled = !entity.Enabled;
                db.SaveChanges();
            }
        }

        public BsSelectList<int?> GetJobsDropdown(int? selected = null)
        {
            return new BsSelectList<int?>
            {
                Items = (from d in db.Jobs
                         orderby d.Name
                         select new BsSelectListItem
                         {
                             Text = d.Name,
                             Selected = selected.HasValue && selected.Value == d.Id,
                             Value = d.Id.ToString()
                         }).ToList(),
                SelectedValues = selected
            };
        }
        #endregion
    }
}