using BForms.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BForms.Utilities
{
    [Serializable]
    public class BsComponentStateIdentifier
    {
        public string ComponentId { get; set; }
        public string ReferralUrl { get; set; }
    }

    [Serializable]
    public class BsComponentState<T> where T : BsComponentStateIdentifier
    {
        public int Id { get; set; }
        public T Identifier { get; set; }
        public string SearchData { get; set; }
        public string QuickSearch { get; set; }
        public List<BsColumnOrder> OrderableColumns { get; set; } // order grid by column
        public Dictionary<string, int> OrderColumns { get; set; } // swap columns order
        public int PerPage { get; set; }
        
        public BsGridRepositorySettings<TSearch> ToBsGridRepositorySettings<TSearch>()
        {
            return new BsGridRepositorySettings<TSearch>()
            {
                Search = Newtonsoft.Json.JsonConvert.DeserializeObject<TSearch>(SearchData),
                PageSize = PerPage,
                OrderableColumns = OrderableColumns,
                OrderColumns = OrderColumns,
                QuickSearch = QuickSearch,
                Page = 1
            };
        }
    }

    public interface IBsComponentStateHandler<T> where T : BsComponentStateIdentifier
    {
        int Save(BsComponentState<T> state);
        TEntity Get<TEntity>(T identifier) where TEntity : class;
        BsComponentState<T> Get(int id);
    }
}
