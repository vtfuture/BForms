using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BootstrapForms.Models
{
    public class BsSelectList<T>
    {
        private T id;

        public T Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        public static implicit operator T(BsSelectList<T> value)
        {
            return value.Id;
        }

        public static implicit operator BsSelectList<T>(T value)
        {
            return new BsSelectList<T> { Id = value };
        }

        public List<BsSelectListItem> Items { get; set; }

        public BsSelectList()
        {
            Items = new List<BsSelectListItem>();
        }

        public List<SelectListItem> ToSelectList()
        {
            List<SelectListItem> list = null;
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

    }

    public class BsSelectListItem : SelectListItem
    {
        public string GroupKey { get; set; }
        public string GroupName { get; set; }
        public Dictionary<string, string> Data { get; set; }
    }

    /// <summary>
    /// SelectListItem for grouped DropDownList and ListBox
    /// </summary>
    public class BsGroupedSelectListItem : SelectListItem
    {
        /// <summary>
        /// Group unique id
        /// </summary>
        public string GroupKey { get; set; }
        /// <summary>
        /// Group display name
        /// </summary>
        public string GroupName { get; set; }
    }
}
