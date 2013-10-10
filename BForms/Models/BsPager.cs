using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BForms.Models
{
    public class BsPagerModel
    {
        private int pageSize = 5;
        private int currentPage;
        private int totalPages;
        private int totalRecords;

        public int CurrentPage
        {
            get
            {
                return this.currentPage;
            }
        }

        public int TotalPages
        {
            get
            {
                return this.totalPages;
            }
        }

        public int TotalRecords
        {
            get
            {
                return this.totalRecords;
            }
        }

        public int PageSize
        {
            get
            {
                return this.pageSize;
            }
        }

        public int CurrentPageRecords { get; set; }

        public BsPagerModel(int totalRecords, int pageSize = 5, int page = 1)
        {
            this.totalPages = (int)Math.Ceiling((Decimal)totalRecords / (Decimal)pageSize);
            if (page <= 0)
                page = 1;
            else if (page > this.totalPages)
                page = this.totalPages;
            this.currentPage = page;
            this.totalRecords = totalRecords;
            this.pageSize = pageSize;
        }

        public int GetStartPage(int length)
        {
            int num = this.CurrentPage - length / 2;
            if (num < 1 || length > this.TotalPages)
                num = 1;
            else if (this.CurrentPage > this.TotalPages - length / 2)
                num = this.TotalPages - length + 1;
            return num;
        }

        public void TotalRecordsChanged(int totalRecords)
        {
        }

        private void SetProperties()
        {
        }
    }
}