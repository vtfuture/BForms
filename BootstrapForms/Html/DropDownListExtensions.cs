using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using BootstrapForms.Models;
using BootstrapForms.Utilities;

namespace BootstrapForms.Html
{
    /// <summary>
    /// Represents bootstrap v3 support for password input control
    /// </summary>
    public static class DropDownListExtensions
    {
        #region DropDownList
        /// <summary>
        ///  Returns a single-selection select element with placeholder
        /// </summary>
        public static MvcHtmlString BsDropDownList(this HtmlHelper htmlHelper, string name)
        {
            return BsDropDownList(htmlHelper, name, null /* selectList */, null /* optionLabel */, htmlAttributes: null);
        }

        /// <summary>
        ///  Returns a single-selection select element with placeholder
        /// </summary>
        public static MvcHtmlString BsDropDownList(this HtmlHelper htmlHelper, string name, string optionLabel)
        {
            return BsDropDownList(htmlHelper, name, null /* selectList */, optionLabel, htmlAttributes: null);
        }

        /// <summary>
        ///  Returns a single-selection select element with placeholder
        /// </summary>
        public static MvcHtmlString BsDropDownList(this HtmlHelper htmlHelper, string name,
            IEnumerable<SelectListItem> selectList)
        {
            return BsDropDownList(htmlHelper, name, selectList, null /* optionLabel */, htmlAttributes: null);
        }

        /// <summary>
        ///  Returns a single-selection select element with placeholder
        /// </summary>
        public static MvcHtmlString BsDropDownList(this HtmlHelper htmlHelper, string name,
            IEnumerable<SelectListItem> selectList, object htmlAttributes)
        {
            return BsDropDownList(htmlHelper, name, selectList, null /* optionLabel */,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        ///  Returns a single-selection select element with placeholder
        /// </summary>
        public static MvcHtmlString BsDropDownList(this HtmlHelper htmlHelper, string name,
            IEnumerable<SelectListItem> selectList, IDictionary<string, object> htmlAttributes)
        {
            return BsDropDownList(htmlHelper, name, selectList, null /* optionLabel */, htmlAttributes);
        }

        /// <summary>
        ///  Returns a single-selection select element with placeholder
        /// </summary>
        public static MvcHtmlString BsDropDownList(this HtmlHelper htmlHelper, string name,
            IEnumerable<SelectListItem> selectList, string optionLabel)
        {
            return BsDropDownList(htmlHelper, name, selectList, optionLabel, htmlAttributes: null);
        }

        /// <summary>
        ///  Returns a single-selection select element with placeholder
        /// </summary>
        public static MvcHtmlString BsDropDownList(this HtmlHelper htmlHelper, string name,
            IEnumerable<SelectListItem> selectList, string optionLabel, object htmlAttributes)
        {
            return BsDropDownList(htmlHelper, name, selectList, optionLabel,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        ///  Returns a single-selection select element with placeholder
        /// </summary>
        public static MvcHtmlString BsDropDownList(this HtmlHelper htmlHelper, string name,
            IEnumerable<SelectListItem> selectList, string optionLabel, IDictionary<string, object> htmlAttributes)
        {
            //add bootstrap attributes
            htmlAttributes.ApplyDropDownListAttributes();

            return htmlHelper.DropDownListInternal(name, selectList, optionLabel, htmlAttributes);
        }
        #endregion

        #region DropDownListFor
        /// <summary>
        ///  Returns a single-selection select element with placeholder
        /// </summary>
        public static MvcHtmlString BsDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList)
        {
            return BsDropDownListFor(htmlHelper, expression, selectList, (object)null);
        }

        /// <summary>
        ///  Returns a single-selection select element with placeholder
        /// </summary>
        public static MvcHtmlString BsDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel)
        {
            return BsDropDownListFor(htmlHelper, expression, selectList, optionLabel, (object)null);
        }

        /// <summary>
        ///  Returns a single-selection select element with placeholder
        /// </summary>
        public static MvcHtmlString BsDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList,
            object htmlAttributes)
        {
            return BsDropDownListFor(htmlHelper, expression, selectList, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        ///  Returns a single-selection select element with placeholder
        /// </summary>
        public static MvcHtmlString BsDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList,
            IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.BsDropDownListFor(expression, selectList, null, htmlAttributes);
        }

        /// <summary>
        ///  Returns a single-selection select element with placeholder
        /// </summary>
        public static MvcHtmlString BsDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel,
            object htmlAttributes)
        {
            return BsDropDownListFor(htmlHelper, expression, selectList, optionLabel,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Returns a single-selection select element with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel,
            IDictionary<string, object> htmlAttributes)
        {

            htmlAttributes.ApplyDropDownListAttributes();

            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

            //add optionLabel from Watermark 
            if (!string.IsNullOrEmpty(metadata.Watermark) && string.IsNullOrEmpty(optionLabel))
            {
                optionLabel = metadata.Watermark;
            }

            //add info tooltip
            var description = new MvcHtmlString("");
            if (!string.IsNullOrEmpty(metadata.Description))
            {
                description = htmlHelper.BsDescriptionFor(expression);
            }

            return
                MvcHtmlString.Create(
                    htmlHelper.DropDownListForInternal(expression, selectList, optionLabel, htmlAttributes).ToHtmlString() +
                    description.ToHtmlString());
        }
        #endregion

        #region Helpers
        internal static MvcHtmlString DropDownListInternal(this HtmlHelper htmlHelper, string name,
            IEnumerable<SelectListItem> selectList, string optionLabel, IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.DropDownList(name, selectList, optionLabel, htmlAttributes);
        }

        internal static MvcHtmlString DropDownListForInternal<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel,
            IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.DropDownListFor(expression, selectList, optionLabel, htmlAttributes);
        }

        internal static void ApplyDropDownListAttributes(this IDictionary<string, object> htmlAttributes)
        {
            //add bootstrap css
            htmlAttributes.MergeAttribute("class", "form-control");
            htmlAttributes.MergeAttribute("class", BsControlType.DropDownList.GetDescription());
        }
        #endregion
    }
}
