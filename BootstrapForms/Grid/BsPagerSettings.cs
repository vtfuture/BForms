using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BootstrapForms.Grid
{
    public class BsPagerSettings
    {
        private int defaultPageSize = 5;
        private int size = 5;
        private bool showPrevNextButtons = true;
        private bool showFirstLastButton = true;
        private bool hasPagesText = true;
        private bool hasPageSizeSelector = true;
        private List<int> pageSizeValues;
        private string template;

        public int DefaultPageSize
        {
            get
            {
                return this.defaultPageSize;
            }
            set
            {
                if (!this.pageSizeValues.Contains(value))
                    throw new ArgumentOutOfRangeException();
                this.defaultPageSize = value;
            }
        }

        public List<int> PageSizeValues
        {
            get
            {
                List<int> list = this.pageSizeValues;
                if (list == null)
                    list = new List<int>()
          {
            5,
            10,
            50,
            100
          };
                return list;
            }
            set
            {
                this.pageSizeValues = value;
            }
        }

        public int Size
        {
            get
            {
                return this.size;
            }
            set
            {
                this.size = value;
            }
        }

        public bool ShowPrevNextButtons
        {
            get
            {
                return this.showPrevNextButtons;
            }
            set
            {
                this.showPrevNextButtons = value;
            }
        }

        public bool ShowFirstLastButtons
        {
            get
            {
                return this.showFirstLastButton;
            }
            set
            {
                this.showFirstLastButton = value;
            }
        }

        public bool HasPagesText
        {
            get
            {
                return this.hasPagesText;
            }
            set
            {
                this.hasPagesText = true;
            }
        }

        public bool HasPageSizeSelector
        {
            get
            {
                return this.hasPageSizeSelector;
            }
            set
            {
                this.hasPageSizeSelector = value;
            }
        }

        public string Template
        {
            get
            {
                return this.template;
            }
            set
            {
                this.template = value;
            }
        }

        private class PageSizeSelectorItem
        {
            public int Value { get; set; }

            public bool Selected { get; set; }
        }
    }
}