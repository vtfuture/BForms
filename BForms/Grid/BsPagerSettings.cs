using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BForms.Grid
{
    public class BsPagerSettings
    {
        private int _defaultPageSize = 5;
        private int _size = 5;
        private bool _showPrevNextButtons = true;
        private bool _showFirstLastButton = true;
        private bool _hasPagesText = true;
        private bool _hasPageSizeSelector = true;
        private bool _strippedDown = false;
        private List<int> _pageSizeValues;
        private string _template;
        private bool _noOffset = false;

        public int DefaultPageSize
        {
            get
            {
                return this._defaultPageSize;
            }
            set
            {
                if (!PageSizeValues.Contains(value))
                    throw new ArgumentOutOfRangeException();
                this._defaultPageSize = value;
            }
        }

        public List<int> PageSizeValues
        {
            get
            {
                List<int> list = this._pageSizeValues;
                if (list == null)
                    list = new List<int>()
          {
            5,
            10,
            50,
            100,
            500
          };
                return list;
            }
            set
            {
                this._pageSizeValues = value;
            }
        }

        public int Size
        {
            get
            {
                return this._size;
            }
            set
            {
                this._size = value;
            }
        }

        public bool ShowPrevNextButtons
        {
            get
            {
                return this._showPrevNextButtons;
            }
            set
            {
                this._showPrevNextButtons = value;
            }
        }

        public bool ShowFirstLastButtons
        {
            get
            {
                return this._showFirstLastButton;
            }
            set
            {
                this._showFirstLastButton = value;
            }
        }

        public bool HasPagesText
        {
            get
            {
                return this._hasPagesText;
            }
            set
            {
                this._hasPagesText = value;
            }
        }

        public bool HasPageSizeSelector
        {
            get
            {
                return this._hasPageSizeSelector;
            }
            set
            {
                this._hasPageSizeSelector = value;
            }
        }

        public bool StrippedDown
        {
            get
            {
                return this._strippedDown;
            }
            set
            {
                this._strippedDown = value;
            }
        }

        public string Template
        {
            get
            {
                return this._template;
            }
            set
            {
                this._template = value;
            }
        }

        public bool NoOffset
        {
            get { return this._noOffset; }
            set { this._noOffset = value; }
        }

        private class PageSizeSelectorItem
        {
            public int Value { get; set; }

            public bool Selected { get; set; }
        }
    }
}