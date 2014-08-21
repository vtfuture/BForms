using System.Net.Mime;
using BForms.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace BForms.Models
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

        /// <summary>
        /// Returns all select list items as IEnumerable
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Filla BsSelectList.Items with enum vals
        /// </summary>
        public void ItemsFromEnum(Type myEnum, params Enum[] excludedVals)
        {
            var enumType = myEnum;
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("myEnum is not of type enum", "myEnum");
            }

            var excludedList = excludedVals.Select(val => ReflectionHelpers.EnumDisplayName(myEnum, val)).ToList();

            foreach (var item in Enum.GetValues(enumType))
            {
                //get Display Name from resources
                var textValue = ReflectionHelpers.EnumDisplayName(myEnum, item as Enum);

                if (excludedList.All(x => x != textValue))
                {
                    this.Items.Add(new BsSelectListItem
                    {
                        Selected = false,
                        Text = textValue,
                        Value = Convert.ChangeType(item, Enum.GetUnderlyingType(myEnum)).ToString()
                    });
                }
            }
        }

        /// <summary>
        /// Returns a BsSelectList from 
        /// </summary>
        public static BsSelectList<T> FromSelectList(IEnumerable<SelectListItem> list)
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

        /// <summary>
        /// Returns a BsSelectList from enum using the DescriptionAttribute
        /// </summary>
        public static BsSelectList<T> FromEnum(Type myEnum)
        {
            var enumType = myEnum;
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("E is not of type enum", "myEnum");
            }

            var bsList = new BsSelectList<T>();
            foreach (var item in Enum.GetValues(enumType))
            {
                //get Display Name from resources
                var textValue = ReflectionHelpers.EnumDisplayName(myEnum, item as Enum);

                bsList.Items.Add(new BsSelectListItem
                                 {
                                     Selected = false,
                                     Text = textValue,
                                     Value = Convert.ChangeType(item, Enum.GetUnderlyingType(myEnum)).ToString()
                                 });
            }

            return bsList;
        }

    }
}