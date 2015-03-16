using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using BForms.Models;
using BForms.Mvc;

using DocumentFormat.OpenXml.EMMA;
using Newtonsoft.Json;

namespace BForms.Utilities
{
    /// <summary>
    /// Reflection helper class 
    /// </summary>
    public static class ReflectionHelpers
    {
        /// <summary>
        /// Returns the Name value of Description attribute
        /// </summary>
        public static string GetDescription(this Enum enumValue)
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
        /// Returns the text value of Display attribute
        /// </summary>
        public static string EnumDisplayName(Type myEnum, Enum val)
        {
            var enumType = myEnum;
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("myEnum is not of type enum", "myEnum");
            }

            var name = Enum.GetName(enumType, val);

            var text = enumType.GetMember(name)
                    .First()
                    .GetCustomAttributes(false)
                    .OfType<DisplayAttribute>()
                    .LastOrDefault();

            var textValue = text == null ? name : text.GetName();

            return textValue;
        }

        /// <summary>
        /// Returns the Name value of Description attribute
        /// </summary>
        public static string EnumDescription(Type myEnum, Enum val)
        {
            var enumType = myEnum;
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("myEnum is not of type enum", "myEnum");
            }

            var name = Enum.GetName(enumType, val);

            var text = enumType.GetMember(name)
                    .First()
                    .GetCustomAttributes(false)
                    .OfType<DescriptionAttribute>()
                    .LastOrDefault();

            var textValue = text == null ? name : text.Description;

            return textValue;
        }

        /// <summary>
        /// Retuns Attribute
        /// </summary>
        internal static bool TryGetAttribute<T>(string name, Type modelType, out T attribute) where T : Attribute
        {
            var hasAttribute = false;
            attribute = default(T);

            PropertyInfo property = null;

            foreach (var prop in name.Split('.'))
            {
                if (modelType == null)
                {
                    break;
                }
                var splitByIndex = prop.Split('[')[0];
                property = modelType.GetProperty(splitByIndex);
                if (property == null)
                {
                    modelType = null;
                }
                else
                {
                    modelType = property.PropertyType;
                    if (modelType.InheritsOrImplements(typeof(IEnumerable<>)) && modelType.GenericTypeArguments.Length > 0)
                    {
                        modelType = modelType.GenericTypeArguments[0];    
                    }
                }
            }
            if (property != null)
            {
                hasAttribute = Attribute.IsDefined(property, typeof(T));

                if (hasAttribute)
                {
                    attribute = (T)Attribute.GetCustomAttribute(property, typeof(T));
                }
            }

            return hasAttribute;
        }

        /// <summary>
        /// Retuns Attribute
        /// </summary>
        internal static bool TryGetAttribute<T>(PropertyInfo property, out T attribute) where T : Attribute
        {
            var hasAttribute = false;
            attribute = default(T);

            if (property != null)
            {
                hasAttribute = Attribute.IsDefined(property, typeof(T));

                if (hasAttribute)
                {
                    attribute = (T)Attribute.GetCustomAttribute(property, typeof(T));
                }
            }

            return hasAttribute;
        }

        /// <summary>
        /// Gets propertyInfo from expression
        /// </summary>
        internal static PropertyInfo GetPropertyInfo<TSource, TProperty>(this Expression<Func<TSource, TProperty>> propertyLambda)
        {
            Type type = typeof(TSource);
            MemberExpression memberExpression = propertyLambda.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException(string.Format("Expression '{0}' refers to a method, not a property.", (object)propertyLambda.ToString()));
            PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == (PropertyInfo)null)
                throw new ArgumentException(string.Format("Expression '{0}' refers to a field, not a property.", (object)propertyLambda.ToString()));
            if (type != propertyInfo.ReflectedType && !type.IsSubclassOf(propertyInfo.ReflectedType))
                throw new ArgumentException(string.Format("Expresion '{0}' refers to a property that is not from type {1}.", (object)propertyLambda.ToString(), (object)type));
            else
                return propertyInfo;
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
            var html5Type = String.Empty;

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

        internal static string GetPropertyTypeName(Type propertyType)
        {
            if (propertyType.IsGenericType &&
                    propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                propertyType = propertyType.GetGenericArguments()[0];
            }

            return propertyType.Name;
        }

        /// <summary>
        /// Checks to see if a generic type is a subclass of a raw generic type. eg. List&lt;int&gt; for List&lt;&gt;
        /// </summary>
        internal static bool InheritsOrImplements(this Type child, Type parent)
        {
            parent = ResolveGenericTypeDefinition(parent);

            var currentChild = child.IsGenericType
                                   ? child.GetGenericTypeDefinition()
                                   : child;

            while (currentChild != typeof(object))
            {
                if (parent == currentChild || HasAnyInterfaces(parent, currentChild))
                    return true;

                currentChild = currentChild.BaseType != null
                               && currentChild.BaseType.IsGenericType
                                   ? currentChild.BaseType.GetGenericTypeDefinition()
                                   : currentChild.BaseType;

                if (currentChild == null)
                    return false;
            }
            return false;
        }

        private static bool HasAnyInterfaces(Type parent, Type child)
        {
            return child.GetInterfaces()
                .Any(childInterface =>
                {
                    var currentInterface = childInterface.IsGenericType
                        ? childInterface.GetGenericTypeDefinition()
                        : childInterface;

                    return currentInterface == parent;
                });
        }

        private static Type ResolveGenericTypeDefinition(Type parent)
        {
            if (parent.IsGenericType && parent.GetGenericTypeDefinition() != parent)
            {
                parent = parent.GetGenericTypeDefinition();
            }
            return parent;
        }

        internal static Type ResolveNullableType(this Type t)
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var genericArgs = t.GetGenericArguments();
                if (genericArgs.Length > 0)
                {
                    return genericArgs[0];
                }
            }

            return t;
        }

        internal static string GetNonEnumerableValue(object obj)
        {
            var bsKeyType = obj.GetType();
            var result = String.Empty;

            if (bsKeyType.IsEnum)
            {
                result =
                    Convert.ChangeType(obj, Enum.GetUnderlyingType(bsKeyType)).ToString();

            }
            else if (bsKeyType.InheritsOrImplements(typeof(Nullable<>)) && bsKeyType.GenericTypeArguments[0].IsEnum)
            {
                result =
                    Convert.ChangeType(obj, Enum.GetUnderlyingType(bsKeyType.GenericTypeArguments[0])).ToString();
            }
            else
            {
                result = obj.ToString();
            }
            return result;
        }

        internal static string ToJsonString(this IDictionary<string, object> options)
        {
            var config = new StringBuilder();
            config.Append("{");
            foreach (var item in options)
            {
                config.AppendFormat(" \"{0}\": {1}{2} ", item.Key, JsonConvert.SerializeObject(item.Value), options.Last().Equals(item) ? "" : ",");
            }
            config.Append("}");
            return config.ToString();
        }

        internal static Type GetDeclaringType<T>(T instance, Expression<Func<T, object>> selector)
        {
            var instanceType = typeof(T);
            var key = instance.GetPropertyName(selector);
            var result = instanceType.GetProperty(key).PropertyType;
            return result;
        }

        internal static string GetPropertyName<T, TProp>(this T instance, Expression<Func<T, TProp>> selector)
        {
            return GetPropertyName(selector);
        }

        internal static string GetPropertyName<T, TProp>(Expression<Func<T, TProp>> selector)
        {
            var member = selector.Body as MemberExpression;
            var unary = selector.Body as UnaryExpression;
            var memberInfo = member ?? (unary != null ? unary.Operand as MemberExpression : null);
            if (memberInfo == null)
            {
                throw new Exception("Could not get selector from specified expression.");
            }
            return memberInfo.Member.Name;
        }
    }
}
