using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocumentFormat.OpenXml.Bibliography;
using BForms.Utilities;

namespace BForms.Grid
{
    public class BsGridRepositorySettings<TSearch> : BsGridBaseRepositorySettings
    {
        public TSearch Search { get; set; }

        public bool FromReset { get; set; }

        public BsComponentState<BsComponentStateIdentifier> ToBsComponentState(string componentId, string referralUrl)
        {
            return new BsComponentState<BsComponentStateIdentifier>()
            {
                Identifier = new BsComponentStateIdentifier()
                {
                    ComponentId = componentId,
                    ReferralUrl = referralUrl
                },
                OrderableColumns = OrderableColumns,
                OrderColumns = OrderColumns,
                PerPage = PageSize,
                QuickSearch = QuickSearch,
                SearchData = Newtonsoft.Json.JsonConvert.SerializeObject(Search)
            };
        }
    }
    
    public class BsGridBaseRepositorySettings : BsBaseRepositorySettings
    {
        public string QuickSearch { get; set; }

        public BsDirectionType? GoTo { get; set; }

        public object UniqueID { get; set; }

        public List<BsColumnOrder> OrderableColumns { get; set; } // order grid by column

        public Dictionary<string, int> OrderColumns { get; set; } // swap columns order

        public int DetailsStartIndex { get; set; }

        public int DetailsCount { get; set; }

        public bool DetailsAll { get; set; }

        public BsGridBaseRepositorySettings GetBase()
        {
            return new BsGridBaseRepositorySettings
            {
                OrderColumns = this.OrderColumns,
                OrderableColumns = this.OrderableColumns,
                Page = this.Page,
                PageSize = this.PageSize,
                DetailsAll = DetailsAll,
                DetailsCount = DetailsCount,
                DetailsStartIndex = DetailsStartIndex
            };
        }

        public BsGridBaseRepositorySettings()
        {
            this.Page = 1;
            this.PageSize = 5;
            this.OrderableColumns= new List<BsColumnOrder>();
        }

        public void SetDetailsInterval(int count)
        {
            SetDetailsInterval(0, count);
        }

        public void SetDetailsInterval(int startIndex, int count)
        {
            this.DetailsStartIndex = startIndex;
            this.DetailsCount = count;
        }

        public bool HasDetails(int index)
        {
            return this.DetailsAll || index >= DetailsStartIndex && index <= DetailsStartIndex + DetailsCount - 1;
        }
    }

    public class BsBaseRepositorySettings
    {
        public BsBaseRepositorySettings()
        {
            this.Page = 1;
            this.PageSize = 5;
        }

        public int Page { get; set; }

        public int PageSize { get; set; }
    }

    public enum BsDirectionType
    {
        First = 1,
        Prev = 2,
        Next = 3,
        Last = 4
    }
}