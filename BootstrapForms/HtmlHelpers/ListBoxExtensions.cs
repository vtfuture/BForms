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

namespace BootstrapForms.HtmlHelpers
{
    /// <summary>
    /// Represents bootstrap v3 support for password input control
    /// </summary>
    public static class ListBoxExtensions
    {
        #region ListBox
        /// <summary>
        ///  Returns a single-selection select element with placeholder
        /// </summary>
        public static MvcHtmlString BsListBox(this HtmlHelper htmlHelper, string name)
        {
            return BsListBox(htmlHelper, name, null /* selectList */, htmlAttributes: null);
        }

        /// <summary>
        ///  Returns a single-selection select element with placeholder
        /// </summary>
        public static MvcHtmlString BsListBox(this HtmlHelper htmlHelper, string name,
            IEnumerable<SelectListItem> selectList)
        {
            return BsListBox(htmlHelper, name, selectList, htmlAttributes: null);
        }

        /// <summary>
        ///  Returns a single-selection select element with placeholder
        /// </summary>
        public static MvcHtmlString BsListBox(this HtmlHelper htmlHelper, string name,
            IEnumerable<SelectListItem> selectList, object htmlAttributes)
        {
            return BsListBox(htmlHelper, name, selectList,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }


        /// <summary>
        ///  Returns a single-selection select element with placeholder
        /// </summary>
        public static MvcHtmlString BsListBox(this HtmlHelper htmlHelper, string name,
            IEnumerable<SelectListItem> selectList, IDictionary<string, object> htmlAttributes)
        {
            //add bootstrap attributes
            htmlAttributes.ApplyListBoxAttributes();

            return htmlHelper.ListBoxInternal(name, selectList, htmlAttributes);
        }
        #endregion

        #region ListBoxFor
        /// <summary>
        ///  Returns a single-selection select element with placeholder
        /// </summary>
        public static MvcHtmlString BsListBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList)
        {
            return BsListBoxFor(htmlHelper, expression, selectList, (object)null);
        }

        /// <summary>
        ///  Returns a single-selection select element with placeholder
        /// </summary>
        public static MvcHtmlString BsListBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList,
            object htmlAttributes)
        {
            return BsListBoxFor(htmlHelper, expression, selectList, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Returns a single-selection select element with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsListBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList,
            IDictionary<string, object> htmlAttributes)
        {

            htmlAttributes.ApplyListBoxAttributes();

            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

            //add info tooltip
            var description = new MvcHtmlString("");
            if (!string.IsNullOrEmpty(metadata.Description))
            {
                description = htmlHelper.BsDescriptionFor(expression);
            }

            return
                MvcHtmlString.Create(
                    htmlHelper.ListBoxForInternal(expression, selectList, null, htmlAttributes).ToHtmlString() +
                    description.ToHtmlString());
        }
        #endregion

        #region Helpers
        internal static MvcHtmlString ListBoxInternal(this HtmlHelper htmlHelper, string name,
            IEnumerable<SelectListItem> selectList, IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.ListBox(name, selectList, htmlAttributes);
        }

        internal static MvcHtmlString ListBoxForInternal<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel,
            IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.ListBoxFor(expression, selectList, htmlAttributes);
        }

        internal static void ApplyListBoxAttributes(this IDictionary<string, object> htmlAttributes)
        {
            //add bootstrap css
            htmlAttributes.MergeAttribute("class", "form-control");
            htmlAttributes.MergeAttribute("class", BsControlType.ListBox.GetDescription());
        }
        #endregion
    }
}
