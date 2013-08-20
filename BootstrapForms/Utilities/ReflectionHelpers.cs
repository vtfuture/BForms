using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using BootstrapForms.Models;
using BootstrapForms.Mvc;

namespace BootstrapForms.Utilities
{
    internal static class ReflectionHelpers
    {
        /// <summary>
        /// Retuns BsControlAttribute
        /// </summary>
        internal static bool TryGetControlAttribute(string name, Type modelType, out BsControlAttribute attribute)
        {
            var hasAttribute = false;
            attribute = null;

            PropertyInfo property = null;

            foreach (var prop in name.Split('.'))
            {
                property = modelType.GetProperty(prop);
                modelType = property != null ? property.PropertyType : null;
            }
            if (property != null)
            {
                hasAttribute = Attribute.IsDefined(property, typeof (BsControlAttribute));
                
                if (hasAttribute)
                {
                    attribute = (BsControlAttribute) Attribute.GetCustomAttribute(property, typeof (BsControlAttribute));
                }
            }

            return hasAttribute;
        }

        /// <summary>
        /// Retuns model state value
        /// </summary>
        internal static object GetModelStateValue(this HtmlHelper htmlHelper, string key, Type destinationType)
        {
            ModelState modelState;
            if (htmlHelper.ViewData.ModelState.TryGetValue(key, out modelState))
            {
                if (modelState.Value != null)
                {
                    return modelState.Value.ConvertTo(destinationType, culture: null);
                }
            }
            return null;
        }

        /// <summary>
        /// Returns true if the specified model property has validation errors
        /// </summary>
        internal static bool HasModelStateErros<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression)
        {
            var propertyName = ExpressionHelper.GetExpressionText(expression);
            var name = helper.AttributeEncode(helper.ViewData.TemplateInfo.GetFullHtmlFieldName(propertyName));
            return helper.ViewData.ModelState[name] != null &&
                            helper.ViewData.ModelState[name].Errors != null &&
                            helper.ViewData.ModelState[name].Errors.Count > 0;
        }

        /// <summary>
        /// Appends the specified html attribute to an existing collection
        /// </summary>
        internal static void MergeAttribute(this IDictionary<string, object> htmlAttributes, string key, string val, bool replace = false)
        {
            htmlAttributes = htmlAttributes ?? new Dictionary<string, object>();
            if (htmlAttributes.Any(x => x.Key == key))
            {
                if (replace)
                {
                    htmlAttributes.Remove(key);
                    htmlAttributes.Add(key, val);
                }
                else
                {
                    var attr = htmlAttributes[key].ToString().ToLowerInvariant();
                    if (!attr.Contains(val.ToLowerInvariant()))
                    {
                        htmlAttributes[key] = htmlAttributes[key] + " " + val;
                    }
                }
            }
            else
            {
                htmlAttributes.Add(key, val);
            }
        }

        /// <summary>
        /// Returns the Description attribute value
        /// </summary>
        internal static string GetDescription(this Enum enumValue)
        {
            var attribute = enumValue.GetType()
                        .GetMember(enumValue.ToString())
                        .First()
                        .GetCustomAttributes(false)
                        .OfType<DescriptionAttribute>()
                        .LastOrDefault();

            return attribute == null ? String.Empty : attribute.Description;
        }

        /// <summary>
        /// Retuns HTML5 input type based on DataTypeAttribute
        /// </summary>
        internal static string GetHtml5Type(this string dataType)
        {
            var strongType = (DataType)Enum.Parse(typeof(DataType), dataType);
            string html5Type;

            switch (strongType)
            {
                case DataType.Date:
                    html5Type = "date";
                    break;
                case DataType.DateTime:
                    html5Type = "datetime";
                    break;
                case DataType.EmailAddress:
                    html5Type = "email";
                    break;
                case DataType.ImageUrl:
                    html5Type = "url";
                    break;
                case DataType.Password:
                    html5Type = "password";
                    break;
                case DataType.PhoneNumber:
                    html5Type = "tel";
                    break;
                case DataType.PostalCode:
                    html5Type = "number";
                    break;
                case DataType.Text:
                    html5Type = "text";
                    break;
                case DataType.Time:
                    html5Type = "time";
                    break;
                case DataType.Upload:
                    html5Type = "file";
                    break;
                case DataType.Url:
                    html5Type = "url";
                    break;
                default:
                    html5Type = "text";
                    break;
            }

            return html5Type;
        }

        /// <summary>
        /// Retuns HTML5 input type based on BsControlType
        /// </summary>
        internal static string GetHtml5Type(this BsControlType bsType)
        {
            var html5Type = string.Empty;

            switch (bsType)
            {
                case BsControlType.TextBox:
                    html5Type = "text";
                    break;
                case BsControlType.Password:
                    html5Type = "password";
                    break;
                case BsControlType.Number:
                    html5Type = "number";
                    break;
                case BsControlType.Url:
                    html5Type = "url";
                    break;
                case BsControlType.DatePicker:
                    html5Type = "datetime";
                    break;
                case BsControlType.TimePicker:
                    html5Type = "time";
                    break;
                case BsControlType.Email:
                    html5Type = "email";
                    break;
                case BsControlType.Upload:
                    html5Type = "file";
                    break;
                case BsControlType.CheckBox:
                    html5Type = "checkbox";
                    break;
                case BsControlType.RadioButton:
                    html5Type = "radio";
                    break;
                case BsControlType.ColorPicker:
                    html5Type = "color";
                    break;
            }

            return html5Type;
        }

        /// <summary>
        /// Checks to see if a generic type is a subclass of a raw generic type. eg. List&lt;int&gt; for List&lt;&gt;
        /// </summary>
        /// <param name="generic">The generic without type specifiers, such as List&lt;&gt; or Dictionary&lt;,&gt;</param>
        /// <param name="toCheck">The generic subclass, such as List&lt;&gt; or Dictionary&lt;string,object&gt;</param>
        internal static bool IsSubclassOfRawGeneric(this Type toCheck, Type generic)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }
    }
}
