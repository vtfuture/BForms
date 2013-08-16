using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BootstrapForms.Models
{
    /// <summary>
    /// Represents a list of items for Dropdown, ListBox, RadioList, CheckboxList binding
    /// </summary>
    public class BsSelectList<T>
    {
        private T selectedValues;

        /// <summary>
        /// Selected values
        /// </summary>
        public T SelectedValues
        {
            get
            {
                return selectedValues;
            }
            set
            {
                selectedValues = value;
            }
        }

        public static implicit operator T(BsSelectList<T> value)
        {
            return value.SelectedValues;
        }

        public static implicit operator BsSelectList<T>(T value)
        {
            return new BsSelectList<T> { SelectedValues = value };
        }

        public List<BsSelectListItem> Items { get; set; }

        public BsSelectList()
        {
            Items = new List<BsSelectListItem>();
        }

        public IEnumerable<SelectListItem> ToSelectList()
        {
            var list = new List<SelectListItem>();
            
            foreach (var item in Items)
            {
                list.Add(new SelectListItem
                {
                    Selected = item.Selected,
                    Text = item.Text,
                    Value = item.Value
                });
            }
            return list;
        }

        public static BsSelectList<T> FromSelectList(List<SelectListItem> list)
        {
            var bsList = new BsSelectList<T>();
            foreach (var item in list)
            {
                bsList.Items.Add(new BsSelectListItem
                {
                    Selected = item.Selected,
                    Text = item.Text,
                    Value = item.Value
                });
            }
            return bsList;
        }

    }

    /// <summary>
    /// Represents the selected item in an instance of the BsSelectList
    /// </summary>
    public class BsSelectListItem : SelectListItem
    {
        /// <summary>
        /// Option group unique ID
        /// </summary>
        public string GroupKey { get; set; }

        /// <summary>
        /// Option group display name
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// The dictionary items are serialized in html as data- attributes
        /// </summary>
        public Dictionary<string, string> Data { get; set; }
    }
}
