using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BootstrapForms.Utilities;
using BootstrapForms.Models;

namespace BootstrapForms.Mvc
{
    /// <summary>
    /// MVC ModelState helpers 
    /// </summary>
    public static class ModelStateExtensions
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

        /// <summary>
        /// Removes model state errors except for the specified filed
        /// </summary>
        public static void ClearModelState(this ModelStateDictionary modelState, string propName)
        {
            foreach (var key in modelState.Keys.ToList().Where(key => key.IndexOf(propName, System.StringComparison.Ordinal) == -1))
            {
                modelState.Remove(key);
            }
        }

        public static void AddFormError(this ModelStateDictionary modelState, string prefix, string errorMessage)
        {
            var key = string.IsNullOrEmpty(prefix) ? "BsFormError" : prefix + ".BsFormError";
            modelState.AddModelError(key, errorMessage);
        }

        public static void AddFieldError(this ModelStateDictionary modelState, string prefix, Type filedType, string errorMessage)
        {
            var key = prefix;
            if (filedType.IsSubclassOfRawGeneric(typeof(BsSelectList<>)))
            {
                key += ".SelectedValues";
            }
            if (filedType.IsSubclassOfRawGeneric(typeof(BsRange<>)))
            {
                key += ".TextValue";
            }

            modelState.AddModelError(key, errorMessage);
        }

        /// <summary>
        /// Removes model state errors except for the specified fileds
        /// </summary>
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
