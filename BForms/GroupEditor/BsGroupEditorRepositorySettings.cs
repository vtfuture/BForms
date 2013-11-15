using BForms.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BForms.GroupEditor
{
    public class BsGroupEditorRepositorySettings<TId> : BsBaseRepositorySettings
    {
        public TId TabId { get; set; }

        public object Search { get; set; }

        public string QuickSearch { get; set; }

        public BsGridBaseRepositorySettings ToBaseGridRepositorySettings()
        {
            return new BsGridBaseRepositorySettings
            {
                Page = this.Page,
                PageSize = this.PageSize,
            };
        }

        public BsGridRepositorySettings<T> ToGridRepositorySettings<T>()
        {
            return new BsGridRepositorySettings<T>
            {
                Search = (T)this.Search,
                Page = this.Page,
                PageSize = this.PageSize,
                QuickSearch = this.QuickSearch
            };
        }
    }
}
