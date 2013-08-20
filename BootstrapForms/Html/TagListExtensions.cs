using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using BootstrapForms.Models;
using BootstrapForms.Utilities;

namespace BootstrapForms.Html
{
    /// <summary>
    /// Represents bootstrap v3 support for tag list input control
    /// </summary>
    public static class TagListExtensions
    {
        #region TagList
        /// <summary>
        ///  Returns a single-selection select element with placeholder
        /// </summary>
        public static MvcHtmlString BsTagList(this HtmlHelper htmlHelper, string name)
        {
            return BsTagList(htmlHelper, name, null /* selectList */, null /* optionLabel */, htmlAttributes: null);
        }

        /// <summary>
        ///  Returns a single-selection select element with placeholder
        /// </summary>
        public static MvcHtmlString BsTagList(this HtmlHelper htmlHelper, string name, string optionLabel)
        {
            return BsTagList(htmlHelper, name, null /* selectList */, optionLabel, htmlAttributes: null);
        }

        /// <summary>
        ///  Returns a single-selection select element with placeholder
        /// </summary>
        public static MvcHtmlString BsTagList(this HtmlHelper htmlHelper, string name,
            IEnumerable<SelectListItem> selectList)
        {
            return BsTagList(htmlHelper, name, selectList, null /* optionLabel */, htmlAttributes: null);
        }

        /// <summary>
        ///  Returns a single-selection select element with placeholder
        /// </summary>
        public static MvcHtmlString BsTagList(this HtmlHelper htmlHelper, string name,
            IEnumerable<SelectListItem> selectList, object htmlAttributes)
        {
            return BsTagList(htmlHelper, name, selectList, null /* optionLabel */,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        ///  Returns a single-selection select element with placeholder
        /// </summary>
        public static MvcHtmlString BsTagList(this HtmlHelper htmlHelper, string name,
            IEnumerable<SelectListItem> selectList, IDictionary<string, object> htmlAttributes)
        {
            return BsTagList(htmlHelper, name, selectList, null /* optionLabel */, htmlAttributes);
        }

        /// <summary>
        ///  Returns a single-selection select element with placeholder
        /// </summary>
        public static MvcHtmlString BsTagList(this HtmlHelper htmlHelper, string name,
            IEnumerable<SelectListItem> selectList, string optionLabel)
        {
            return BsTagList(htmlHelper, name, selectList, optionLabel, htmlAttributes: null);
        }

        /// <summary>
        ///  Returns a single-selection select element with placeholder
        /// </summary>
        public static MvcHtmlString BsTagList(this HtmlHelper htmlHelper, string name,
            IEnumerable<SelectListItem> selectList, string optionLabel, object htmlAttributes)
        {
            return BsTagList(htmlHelper, name, selectList, optionLabel,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        ///  Returns a single-selection select element with placeholder
        /// </summary>
        public static MvcHtmlString BsTagList(this HtmlHelper htmlHelper, string name,
            IEnumerable<SelectListItem> selectList, string optionLabel, IDictionary<string, object> htmlAttributes)
        {
            //add bootstrap attributes
            htmlAttributes.ApplyTagListAttributes();

            return htmlHelper.TagListInternal(name, selectList, optionLabel, htmlAttributes);
        }
        #endregion

        #region TagListFor
        /// <summary>
        ///  Returns a single-selection select element with placeholder
        /// </summary>
        public static MvcHtmlString BsTagListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList)
        {
            return BsTagListFor(htmlHelper, expression, selectList, (object)null);
        }

        /// <summary>
        ///  Returns a single-selection select element with placeholder
        /// </summary>
        public static MvcHtmlString BsTagListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel)
        {
            return BsTagListFor(htmlHelper, expression, selectList, optionLabel, (object)null);
        }

        /// <summary>
        ///  Returns a single-selection select element with placeholder
        /// </summary>
        public static MvcHtmlString BsTagListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList,
            object htmlAttributes)
        {
            return BsTagListFor(htmlHelper, expression, selectList, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        ///  Returns a single-selection select element with placeholder
        /// </summary>
        public static MvcHtmlString BsTagListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList,
            IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.BsTagListFor(expression, selectList, null, htmlAttributes);
        }

        /// <summary>
        ///  Returns a single-selection select element with placeholder
        /// </summary>
        public static MvcHtmlString BsTagListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel,
            object htmlAttributes)
        {
            return BsTagListFor(htmlHelper, expression, selectList, optionLabel,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Returns a single-selection select element with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsTagListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel,
            IDictionary<string, object> htmlAttributes)
        {

            htmlAttributes.ApplyTagListAttributes();

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
                    htmlHelper.TagListForInternal(expression, selectList, optionLabel, htmlAttributes).ToHtmlString() +
                    description.ToHtmlString());
        }
        #endregion

        #region Helpers
        internal static MvcHtmlString TagListInternal(this HtmlHelper htmlHelper, string name,
            IEnumerable<SelectListItem> selectList, string optionLabel, IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.DropDownList(name, selectList, optionLabel, htmlAttributes);
        }

        internal static MvcHtmlString TagListForInternal<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel,
            IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.DropDownListFor(expression, selectList, optionLabel, htmlAttributes);
        }

        internal static void ApplyTagListAttributes(this IDictionary<string, object> htmlAttributes)
        {
            //add bootstrap css
            htmlAttributes.MergeAttribute("class", "form-control");
            htmlAttributes.MergeAttribute("class", BsControlType.TagList.GetDescription());
        }
        #endregion
    }
}
