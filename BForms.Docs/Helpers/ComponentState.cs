using BForms.Docs.Areas.Demo.Mock;
using BForms.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BForms.Docs.Helpers
{
    public class ComponentStateHandler : IBsComponentStateHandler<BsComponentStateIdentifier>
    {
        private BFormsContext db;

        public ComponentStateHandler(BFormsContext _db)
        {
            db = _db;
        }

        public int Save(BsComponentState<BsComponentStateIdentifier> state)
        {
            var entity = Get<BsComponentState<BsComponentStateIdentifier>>(state.Identifier);

            if (entity != null)
            {
                entity.PerPage = state.PerPage;
                entity.SearchData = state.SearchData;
                entity.OrderableColumns = state.OrderableColumns;
                entity.OrderColumns = state.OrderColumns;
                entity.QuickSearch = state.QuickSearch;
                db.SaveChanges();

                return entity.Id;
            }
            else
            {
                var lastId = db.ComponentStates.Count() + 1;
                state.Id = lastId;
                db.ComponentStates.Add(state);
                db.SaveChanges();

                return state.Id;
            }
        }

        public BsComponentState<BsComponentStateIdentifier> Get(int id)
        {
            return db.ComponentStates.FirstOrDefault(x => x.Id == id);
        }

        public T Get<T>(BsComponentStateIdentifier identifier) where T : class
        {
            return db.ComponentStates.FirstOrDefault(x => x.Identifier.ComponentId == identifier.ComponentId && x.Identifier.ReferralUrl == identifier.ReferralUrl) as T;
        }
    }
}