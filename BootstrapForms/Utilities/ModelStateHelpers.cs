﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BootstrapForms.Utilities
{
    public static class ModelStateHelpers
    {
        public static Dictionary<string, string> GetErrors(this ModelStateDictionary modelState, string replaceWith = ".")
        {
            var errorDictionary = new Dictionary<string, string>();
            var collection = modelState.Keys.Where(r => modelState[r].Errors.Any());
            foreach (var item in collection)
            {
                errorDictionary.Add(item.Replace(".", replaceWith), modelState[item].Errors.FirstOrDefault().ErrorMessage);
            }
            return errorDictionary;
        }

        public static void ClearModelState(this ModelStateDictionary ms, string propName)
        {
            foreach (var key in ms.Keys.ToList().Where(key => key.IndexOf(propName, System.StringComparison.Ordinal) == -1))
            {
                ms.Remove(key);
            }
        }

        public static void ClearModelState(this ModelStateDictionary ms, List<string> props)
        {
            foreach (var key in ms.Keys.ToList())
            {
                if (props.All(propName => key.IndexOf(propName, StringComparison.Ordinal) == -1))
                {
                    ms.Remove(key);
                }
            }
        }

        public static string GetPropertyName<T, TProp>(this T instance, Expression<Func<T, TProp>> selector)
        {
            return GetPropertyName<T, TProp>(selector);
        }

        public static string GetPropertyName<T, TProp>(Expression<Func<T, TProp>> selector)
        {
            var bodyString = selector.Body.ToString();
            return bodyString.Substring(bodyString.IndexOf('.') + 1);
        }
    }
}
