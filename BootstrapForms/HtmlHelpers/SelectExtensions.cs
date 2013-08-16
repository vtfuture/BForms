using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using BootstrapForms.Utilities;
using BootstrapForms.Models;
using BootstrapForms.Attributes;

namespace BootstrapForms.HtmlHelpers
{
    /// <summary>
    /// Represents support for making selections in a list
    /// </summary>
    public static class SelectExtensions
    {
        #region RadioGroup

        /// <summary>
        /// Returns a a list of radio buttons
        /// </summary>
        public static MvcHtmlString BsRadioButtonList(this HtmlHelper htmlHelper, string name,
            IEnumerable<SelectListItem> radioList)
        {
            return BsRadioButtonList(htmlHelper, name, radioList, htmlAttributes: (object)null);
        }

        /// <summary>
        /// Returns a a list of radio buttons
        /// </summary>
        public static MvcHtmlString BsRadioButtonList(this HtmlHelper htmlHelper, string name,
            IEnumerable<SelectListItem> radioList, object htmlAttributes)
        {
            return BsRadioButtonList(htmlHelper, name, radioList,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Returns a a list of radio buttons
        /// </summary>
        public static MvcHtmlString BsRadioButtonList(this HtmlHelper htmlHelper, string name,
            IEnumerable<SelectListItem> radioList, IDictionary<string, object> htmlAttributes)
        {
            var metadata = ModelMetadata.FromStringExpression(name, htmlHelper.ViewData);
            return htmlHelper.BsRadioButtonListInternal(metadata, name, radioList, htmlAttributes, "bs-radio-list");
        }

        /// <summary>
        /// Returns a list of radio buttons
        /// </summary>
        public static MvcHtmlString BsRadioButtonListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> radioList)
        {
            return BsRadioButtonListFor(htmlHelper, expression, radioList, (object)null);
        }

        /// <summary>
        /// Returns a list of radio buttons
        /// </summary>
        public static MvcHtmlString BsRadioButtonListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> radioList, object htmlAttributes)
        {
            return BsRadioButtonListFor(htmlHelper, expression, radioList,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Returns a list of radio buttons
        /// </summary>
        public static MvcHtmlString BsRadioButtonListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> radioList,
            IDictionary<string, object> htmlAttributes)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

            //add bs- control type
            BsControlAttribute bsControl = null;
            string bsCssClass = string.Empty;
            if (ReflectionHelpers.TryGetControlAttribute(ExpressionHelper.GetExpressionText(expression), typeof(TModel), out bsControl))
            {
                bsCssClass = bsControl.ControlType.GetDescription();
            }

            return htmlHelper.BsRadioButtonListInternal(metadata, metadata.PropertyName, radioList, htmlAttributes, bsCssClass);
        }

        private static MvcHtmlString BsRadioButtonListInternal(this HtmlHelper htmlHelper, ModelMetadata metadata,
            string name, IEnumerable<SelectListItem> radioList, IDictionary<string, object> htmlAttributes, string bsCssClass)
        {
            var propertyName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            var html = new StringBuilder();
            var divTag = new TagBuilder("div");
            divTag.MergeAttribute("id", propertyName, true);
            divTag.MergeAttribute("class", "form-control");
            divTag.MergeAttribute("class", bsCssClass);

            if (htmlAttributes != null)
            {
                divTag.MergeAttributes(htmlAttributes);
            }

            // Create a radio button for each item in the list
            foreach (var item in radioList)
            {
                // Generate an id to be given to the radio button field
                var id = string.Format("{0}_{1}", propertyName, item.Value);

                // Create and populate a radio button using the existing html htmlHelpers
                var label = htmlHelper.Label(id, HttpUtility.HtmlEncode(item.Text));

                var radioHtmlAttributes = new Dictionary<string, object> { { "id", id } };

                if (htmlAttributes != null)
                {
                    htmlAttributes.ToList().ForEach(x =>
                    {
                        if (!radioHtmlAttributes.ContainsKey(x.Key))
                        {
                            radioHtmlAttributes.Add(x.Key, x.Value);
                        }
                    });
                }

                var radio = htmlHelper.RadioButton(name, item.Value, radioHtmlAttributes).ToHtmlString();

                // Create the html string
                // e.g. <input data-val="true" data-val-required="You must select an option" id="TestRadio_1" name="TestRadio" type="radio" value="1" /><label for="TestRadio_1">Line1</label>
                var innerDivTag = new TagBuilder("div");
                innerDivTag.AddCssClass("RadioButton");
                html.Append(innerDivTag.ToString(TagRenderMode.StartTag));
                html.Append(radio);
                html.Append(label);
                html.Append(innerDivTag.ToString(TagRenderMode.EndTag));
            }


            divTag.InnerHtml = html.ToString();

            //add info tooltip
            var description = new MvcHtmlString("");
            if (!string.IsNullOrEmpty(metadata.Description))
            {
                description = htmlHelper.BsDescription(name);
            }

            return MvcHtmlString.Create(divTag.ToString() + description);
        }

        #endregion

        #region TagGroup

        /// <summary>
        /// Returns a list of tags with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsTagListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression)
        {
            return BsTagListFor(htmlHelper, expression, (object)null);
        }

        /// <summary>
        /// Returns a list of tags with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsTagListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return BsTagListFor(htmlHelper, expression, new RouteValueDictionary(htmlAttributes));
        }

        /// <summary>
        /// Returns a list of tags with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsTagListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

            //add bs- control type
            BsControlAttribute bsControl = null;
            if (ReflectionHelpers.TryGetControlAttribute(ExpressionHelper.GetExpressionText(expression), typeof(TModel), out bsControl))
            {
                htmlAttributes.MergeAttribute("class", bsControl.ControlType.GetDescription());
            }

            //merge custom css classes with bootstrap
            htmlAttributes.MergeAttribute("class", "form-control");
            htmlAttributes.MergeAttribute("data-stringifyTags", "true");

            //add placeholder
            if (!string.IsNullOrEmpty(metadata.Watermark) && !htmlAttributes.ContainsKey("placeholder"))
            {
                htmlAttributes.MergeAttribute("placeholder", metadata.Watermark);
            }

            //add info tooltip
            var description = new MvcHtmlString("");
            if (!string.IsNullOrEmpty(metadata.Description))
            {
                description = htmlHelper.BsDescriptionFor(expression);
            }

            return
                MvcHtmlString.Create(htmlHelper.HiddenFor(expression, htmlAttributes).ToHtmlString() +
                                     description.ToHtmlString());
        }

        #endregion

        #region DropDown

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
            //merge custom css classes with bootstrap
            htmlAttributes.MergeAttribute("class", "form-control");

            return htmlHelper.DropDownList(name, selectList, optionLabel, htmlAttributes);
        }

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
            return BsDropDownListFor(htmlHelper, expression, selectList, new RouteValueDictionary(htmlAttributes));
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
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

            //add bs- control type
            BsControlAttribute bsControl = null;
            if (ReflectionHelpers.TryGetControlAttribute(ExpressionHelper.GetExpressionText(expression), typeof(TModel), out bsControl))
            {
                htmlAttributes.MergeAttribute("class", bsControl.ControlType.GetDescription());
            }

            //merge custom css classes with bootstrap
            htmlAttributes.MergeAttribute("class", "form-control");

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
                    htmlHelper.DropDownListFor(expression, selectList, optionLabel, htmlAttributes).ToHtmlString() +
                    description.ToHtmlString());
        }

        #endregion

        #region DropDown grouped

        /// <summary>
        /// Returns a grouped drop-down list element with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString DropDownListGroupedFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<BsGroupedSelectListItem> selectList)
        {
            return DropDownListGroupedFor(htmlHelper, expression, selectList, null /* optionLabel */,
                htmlAttributes: null);
        }

        /// <summary>
        /// Returns a grouped drop-down list element with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString DropDownListGroupedFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<BsGroupedSelectListItem> selectList,
            object htmlAttributes)
        {
            return DropDownListGroupedFor(htmlHelper, expression, selectList, null /* optionLabel */,
                new RouteValueDictionary(htmlAttributes));
        }

        /// <summary>
        /// Returns a grouped drop-down list element with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString DropDownListGroupedFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<BsGroupedSelectListItem> selectList,
            IDictionary<string, object> htmlAttributes)
        {
            return DropDownListGroupedFor(htmlHelper, expression, selectList, null /* optionLabel */, htmlAttributes);
        }

        /// <summary>
        /// Returns a grouped drop-down list element with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString DropDownListGroupedFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<BsGroupedSelectListItem> selectList,
            string optionLabel)
        {
            return DropDownListGroupedFor(htmlHelper, expression, selectList, optionLabel, htmlAttributes: null);
        }

        /// <summary>
        /// Returns a grouped drop-down list element with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString DropDownListGroupedFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<BsGroupedSelectListItem> selectList,
            string optionLabel, object htmlAttributes)
        {
            return DropDownListGroupedFor(htmlHelper, expression, selectList, optionLabel,
                new RouteValueDictionary(htmlAttributes));
        }

        /// <summary>
        /// Returns a grouped drop-down list element with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString DropDownListGroupedFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<BsGroupedSelectListItem> selectList,
            string optionLabel, IDictionary<string, object> htmlAttributes)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            string htmlSelect;

            //add bs- control type
            BsControlAttribute bsControl = null;
            if (ReflectionHelpers.TryGetControlAttribute(ExpressionHelper.GetExpressionText(expression), typeof(TModel), out bsControl))
            {
                htmlAttributes.MergeAttribute("class", bsControl.ControlType.GetDescription());
            }

            //add optionLabel from Watermark 
            if (!string.IsNullOrEmpty(metadata.Watermark) && string.IsNullOrEmpty(optionLabel))
            {
                optionLabel = metadata.Watermark;
            }

            htmlSelect =
                BsGroupedListHelper(htmlHelper, ExpressionHelper.GetExpressionText(expression), selectList,
                    optionLabel, htmlAttributes, false).ToHtmlString();


            //add info tooltip
            var description = new MvcHtmlString("");
            if (!string.IsNullOrEmpty(metadata.Description))
            {
                description = htmlHelper.BsDescriptionFor(expression);
            }

            return MvcHtmlString.Create(htmlSelect + description.ToHtmlString());
        }

        #endregion

        #region ListBox

        /// <summary>
        ///  Returns a ListBox element with placeholder
        /// </summary>
        public static MvcHtmlString BsListBox(this HtmlHelper htmlHelper, string name)
        {
            return BsListBox(htmlHelper, name, null /* selectList */, null /* optionLabel */, htmlAttributes: null);
        }

        /// <summary>
        ///  Returns a ListBox element with placeholder
        /// </summary>
        public static MvcHtmlString BsListBox(this HtmlHelper htmlHelper, string name, string optionLabel)
        {
            return BsListBox(htmlHelper, name, null /* selectList */, optionLabel, htmlAttributes: null);
        }

        /// <summary>
        ///  Returns a ListBox element with placeholder
        /// </summary>
        public static MvcHtmlString BsListBox(this HtmlHelper htmlHelper, string name,
            IEnumerable<SelectListItem> selectList)
        {
            return BsListBox(htmlHelper, name, selectList, null /* optionLabel */, htmlAttributes: null);
        }

        /// <summary>
        ///  Returns a ListBox element with placeholder
        /// </summary>
        public static MvcHtmlString BsListBox(this HtmlHelper htmlHelper, string name,
            IEnumerable<SelectListItem> selectList, object htmlAttributes)
        {
            return BsListBox(htmlHelper, name, selectList, null /* optionLabel */,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        ///  Returns a ListBox element with placeholder
        /// </summary>
        public static MvcHtmlString BsListBox(this HtmlHelper htmlHelper, string name,
            IEnumerable<SelectListItem> selectList, IDictionary<string, object> htmlAttributes)
        {
            return BsListBox(htmlHelper, name, selectList, null /* optionLabel */, htmlAttributes);
        }

        /// <summary>
        ///  Returns a ListBox element with placeholder
        /// </summary>
        public static MvcHtmlString BsListBox(this HtmlHelper htmlHelper, string name,
            IEnumerable<SelectListItem> selectList, string optionLabel)
        {
            return BsListBox(htmlHelper, name, selectList, optionLabel, htmlAttributes: null);
        }

        /// <summary>
        ///  Returns a ListBox element with placeholder
        /// </summary>
        public static MvcHtmlString BsListBox(this HtmlHelper htmlHelper, string name,
            IEnumerable<SelectListItem> selectList, string optionLabel, object htmlAttributes)
        {
            return BsListBox(htmlHelper, name, selectList, optionLabel,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        ///  Returns a ListBox element with placeholder
        /// </summary>
        public static MvcHtmlString BsListBox(this HtmlHelper htmlHelper, string name,
            IEnumerable<SelectListItem> selectList, string optionLabel, IDictionary<string, object> htmlAttributes)
        {
            //merge custom css classes with bootstrap
            htmlAttributes.MergeAttribute("class", "form-control");
            htmlAttributes.MergeAttribute("multiple", "multiple");

            return htmlHelper.DropDownList(name, selectList, optionLabel, htmlAttributes);
        }

        /// <summary>
        ///  Returns a ListBox element with placeholder
        /// </summary>
        public static MvcHtmlString BsListBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList)
        {
            return BsListBoxFor(htmlHelper, expression, selectList, (object)null);
        }

        /// <summary>
        ///  Returns a ListBox element with placeholder
        /// </summary>
        public static MvcHtmlString BsListBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel)
        {
            return BsListBoxFor(htmlHelper, expression, selectList, optionLabel, (object)null);
        }

        /// <summary>
        ///  Returns a ListBox element with placeholder
        /// </summary>
        public static MvcHtmlString BsListBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList,
            object htmlAttributes)
        {
            return BsListBoxFor(htmlHelper, expression, selectList, new RouteValueDictionary(htmlAttributes));
        }

        /// <summary>
        ///  Returns a ListBox element with placeholder
        /// </summary>
        public static MvcHtmlString BsListBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList,
            IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.BsListBoxFor(expression, selectList, null, htmlAttributes);
        }

        /// <summary>
        ///  Returns a ListBox element with placeholder
        /// </summary>
        public static MvcHtmlString BsListBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel,
            object htmlAttributes)
        {
            return BsListBoxFor(htmlHelper, expression, selectList, optionLabel,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Returns a ListBox element with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsListBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel,
            IDictionary<string, object> htmlAttributes)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

            //add bs- control type
            BsControlAttribute bsControl = null;
            if (ReflectionHelpers.TryGetControlAttribute(ExpressionHelper.GetExpressionText(expression), typeof(TModel), out bsControl))
            {
                htmlAttributes.MergeAttribute("class", bsControl.ControlType.GetDescription());
            }

            //merge custom css classes with bootstrap
            htmlAttributes.MergeAttribute("class", "form-control");
            htmlAttributes.MergeAttribute("multiple", "multiple");

            //add info tooltip
            var description = new MvcHtmlString("");
            if (!string.IsNullOrEmpty(metadata.Description))
            {
                description = htmlHelper.BsDescriptionFor(expression);
            }

            return
                MvcHtmlString.Create(
                    htmlHelper.DropDownListFor(expression, selectList, optionLabel, htmlAttributes).ToHtmlString() +
                    description.ToHtmlString());
        }

        #endregion

        #region ListBox grouped

        /// <summary>
        /// Returns a grouped ListBox list element with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsListBoxGroupedFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<BsGroupedSelectListItem> selectList)
        {
            return BsListBoxGroupedFor(htmlHelper, expression, selectList, null /* optionLabel */, htmlAttributes: null);
        }

        /// <summary>
        /// Returns a grouped ListBox list element with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsListBoxGroupedFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<BsGroupedSelectListItem> selectList,
            object htmlAttributes)
        {
            return BsListBoxGroupedFor(htmlHelper, expression, selectList, null /* optionLabel */,
                new RouteValueDictionary(htmlAttributes));
        }

        /// <summary>
        /// Returns a grouped ListBox list element with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsListBoxGroupedFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<BsGroupedSelectListItem> selectList,
            IDictionary<string, object> htmlAttributes)
        {
            return BsListBoxGroupedFor(htmlHelper, expression, selectList, null /* optionLabel */, htmlAttributes);
        }

        /// <summary>
        /// Returns a grouped ListBox list element with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsListBoxGroupedFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<BsGroupedSelectListItem> selectList,
            string optionLabel)
        {
            return BsListBoxGroupedFor(htmlHelper, expression, selectList, optionLabel, htmlAttributes: null);
        }

        /// <summary>
        /// Returns a grouped ListBox list element with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsListBoxGroupedFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<BsGroupedSelectListItem> selectList,
            string optionLabel, object htmlAttributes)
        {
            return BsListBoxGroupedFor(htmlHelper, expression, selectList, optionLabel,
                new RouteValueDictionary(htmlAttributes));
        }

        /// <summary>
        /// Returns a grouped ListBox list element with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsListBoxGroupedFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<BsGroupedSelectListItem> selectList,
            string optionLabel, IDictionary<string, object> htmlAttributes)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            string htmlSelect;

            //add bs- control type
            BsControlAttribute bsControl = null;
            if (ReflectionHelpers.TryGetControlAttribute(ExpressionHelper.GetExpressionText(expression), typeof(TModel), out bsControl))
            {
                htmlAttributes.MergeAttribute("class", bsControl.ControlType.GetDescription());
            }

            //copy watermark to optionLabel
            if (!string.IsNullOrEmpty(metadata.Watermark))
            {
                htmlSelect =
                    BsGroupedListHelper(htmlHelper, ExpressionHelper.GetExpressionText(expression), selectList,
                        metadata.Watermark, htmlAttributes, true).ToHtmlString();
            }
            else
            {
                htmlSelect =
                    BsGroupedListHelper(htmlHelper, ExpressionHelper.GetExpressionText(expression), selectList,
                        optionLabel, htmlAttributes, true).ToHtmlString();
            }

            //add info tooltip
            var description = new MvcHtmlString("");
            if (!string.IsNullOrEmpty(metadata.Description))
            {
                description = htmlHelper.BsDescriptionFor(expression);
            }

            return MvcHtmlString.Create(htmlSelect + description.ToHtmlString());
        }

        #endregion

        #region Grouped Select Helpers

        private static MvcHtmlString BsGroupedListHelper(HtmlHelper htmlHelper, string expression,
            IEnumerable<BsGroupedSelectListItem> selectList, string optionLabel,
            IDictionary<string, object> htmlAttributes, bool allowMultiple)
        {
            return GroupedSelectInternal(htmlHelper, optionLabel, expression, selectList, allowMultiple, htmlAttributes);
        }

        private static MvcHtmlString GroupedSelectInternal(this HtmlHelper htmlHelper, string optionLabel, string name,
            IEnumerable<BsGroupedSelectListItem> selectList, bool allowMultiple,
            IDictionary<string, object> htmlAttributes)
        {
            name = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Null Or Empty", "name");
            }

            bool usedViewData = false;

            // If we got a null selectList, try to use ViewData to get the list of items.
            if (selectList == null)
            {
                selectList = htmlHelper.GetSelectData(name);
                usedViewData = true;
            }

            object defaultValue = (allowMultiple)
                ? htmlHelper.GetModelStateValue(name, typeof(string[]))
                : htmlHelper.GetModelStateValue(name, typeof(string));

            // If we haven't already used ViewData to get the entire list of items then we need to
            // use the ViewData-supplied value before using the parameter-supplied value.
            if (!usedViewData)
            {
                if (defaultValue == null)
                {
                    defaultValue = htmlHelper.ViewData.Eval(name);
                }
            }

            if (defaultValue != null)
            {
                var defaultValues = (allowMultiple) ? defaultValue as IEnumerable : new[] { defaultValue };
                var values = from object value in defaultValues
                             select Convert.ToString(value, CultureInfo.CurrentCulture);
                var selectedValues = new HashSet<string>(values, StringComparer.OrdinalIgnoreCase);
                var newSelectList = new List<BsGroupedSelectListItem>();

                foreach (var item in selectList)
                {
                    item.Selected = (item.Value != null)
                        ? selectedValues.Contains(item.Value)
                        : selectedValues.Contains(item.Text);
                    newSelectList.Add(item);
                }
                selectList = newSelectList;
            }

            // Convert each ListItem to an <option> tag
            var listItemBuilder = new StringBuilder();

            // Make optionLabel the first item that gets rendered.
            if (optionLabel != null)
            {
                listItemBuilder.AppendLine(
                    ListItemToOption(new BsGroupedSelectListItem()
                    {
                        Text = optionLabel,
                        Value = String.Empty,
                        Selected = false
                    }));
            }

            foreach (var group in selectList.GroupBy(i => i.GroupKey))
            {
                string groupName =
                    selectList.Where(i => i.GroupKey == group.Key).Select(it => it.GroupName).FirstOrDefault();
                listItemBuilder.AppendLine(string.Format("<optgroup label=\"{0}\" value=\"{1}\">", groupName, group.Key));
                foreach (BsGroupedSelectListItem item in group)
                {
                    listItemBuilder.AppendLine(ListItemToOption(item));
                }
                listItemBuilder.AppendLine("</optgroup>");
            }

            var tagBuilder = new TagBuilder("select")
            {
                InnerHtml = listItemBuilder.ToString()
            };
            tagBuilder.MergeAttributes(htmlAttributes);
            tagBuilder.MergeAttribute("name", name, true /* replaceExisting */);
            tagBuilder.MergeAttribute("class", "form-control");
            tagBuilder.GenerateId(name);
            if (allowMultiple)
            {
                tagBuilder.MergeAttribute("multiple", "multiple");
            }

            // If there are any errors for a named field, we add the css attribute.
            ModelState modelState;
            if (htmlHelper.ViewData.ModelState.TryGetValue(name, out modelState))
            {
                if (modelState.Errors.Count > 0)
                {
                    tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
                }
            }

            var attributes = htmlHelper.GetUnobtrusiveValidationAttributes(name);

            if (name.Contains(".") && !attributes.Any())
            {
                attributes = htmlHelper.GetUnobtrusiveValidationAttributes(name.Split('.').LastOrDefault());
            }
            tagBuilder.MergeAttributes<string, object>(attributes);

            return MvcHtmlString.Create(tagBuilder.ToString());
        }

        private static IEnumerable<BsGroupedSelectListItem> GetSelectData(this HtmlHelper htmlHelper, string name)
        {
            object o = null;
            if (htmlHelper.ViewData != null)
            {
                o = htmlHelper.ViewData.Eval(name);
            }
            if (o == null)
            {
                throw new InvalidOperationException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        "Missing Select Data"));
            }
            var selectList = o as IEnumerable<BsGroupedSelectListItem>;
            if (selectList == null)
            {
                throw new InvalidOperationException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        "Wrong Select DataType"));
            }
            return selectList;
        }

        private static string ListItemToOption(BsGroupedSelectListItem item)
        {
            var builder = new TagBuilder("option")
            {
                InnerHtml = HttpUtility.HtmlEncode(item.Text)
            };
            if (item.Value != null)
            {
                builder.Attributes["value"] = item.Value;
            }
            if (item.Selected)
            {
                builder.Attributes["selected"] = "selected";
            }
            return builder.ToString(TagRenderMode.Normal);
        }

        #endregion

        #region BsSelectList
        /// <summary>
        /// Returns a BForms select element based on BsControlAttribute
        /// </summary>
        public static MvcHtmlString BsSelectFor<TModel, TKey>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, BsSelectList<TKey>>> expression)
        {
            return htmlHelper.BsSelectFor(expression, null);
        }

        /// <summary>
        /// Returns a BForms select element based on BsControlAttribute
        /// </summary>
        public static MvcHtmlString BsSelectFor<TModel, TKey>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, BsSelectList<TKey>>> expression, IDictionary<string, object> htmlAttributes)
        {

            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            string name = ExpressionHelper.GetExpressionText(expression);

            //get list object from expression
            var selectList = expression.Compile().Invoke(htmlHelper.ViewData.Model);

            string htmlSelect;
            string optionLabel = null;
            bool allowMultiple = false;

            //add optionLabel from Watermark 
            if (!string.IsNullOrEmpty(metadata.Watermark))
            {
                optionLabel = metadata.Watermark;
            }

            //determine the select type
            BsControlAttribute bsControl = null;
            if (ReflectionHelpers.TryGetControlAttribute(name, typeof(TModel), out bsControl))
            {
                //add bs- control type
                var bsCssClass = bsControl.ControlType.GetDescription();

                switch (bsControl.ControlType)
                {
                    case BsControlType.TagList:
                        allowMultiple = true;
                        //TODO: Rewrite TagList
                        htmlSelect = null;
                        break;
                    case BsControlType.CheckBoxList:
                        allowMultiple = true;
                        //TODO: Implement CheckBoxList
                        htmlSelect = BsRadioListInternal(htmlHelper, name,
                            selectList, htmlAttributes, allowMultiple, bsCssClass).ToHtmlString();
                        break;
                    case BsControlType.RadioButtonList:
                        allowMultiple = false;
                        htmlSelect = BsRadioListInternal(htmlHelper, name,
                            selectList, htmlAttributes, allowMultiple, bsCssClass).ToHtmlString();
                        break;
                    case BsControlType.ListBox:
                    case BsControlType.ListBoxGrouped:
                        allowMultiple = true;
                        htmlSelect = BsSelectInternal(htmlHelper, name, selectList,
                            optionLabel, htmlAttributes, allowMultiple, bsCssClass).ToHtmlString();
                        break;
                    case BsControlType.DropDownList:
                    case BsControlType.DropDownListGrouped:
                    default:
                        allowMultiple = false;
                        htmlSelect = BsSelectInternal(htmlHelper, name, selectList,
                        optionLabel, htmlAttributes, allowMultiple, bsCssClass).ToHtmlString();
                        break;
                }
            }
            else
            {
                htmlSelect = BsSelectInternal(htmlHelper, name, selectList,
                    optionLabel, htmlAttributes, allowMultiple, "bs-dropdown").ToHtmlString();
            }


            //add info tooltip
            var description = new MvcHtmlString("");
            if (!string.IsNullOrEmpty(metadata.Description))
            {
                description = htmlHelper.BsDescription(name);
            }

            return MvcHtmlString.Create(htmlSelect + description.ToHtmlString());
        }

        private static MvcHtmlString BsSelectInternal<TKey>(this HtmlHelper htmlHelper, string name,
            BsSelectList<TKey> selectList, string optionLabel,
            IDictionary<string, object> htmlAttributes, bool allowMultiple, string bsCssClass)
        {
            //TODO: refactoring
            //bind the selected values BsSelectList.SelectedValues
            name += ".SelectedValues";

            name = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);

            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Null Or Empty", "name");
            }

            bool usedViewData = false;

            // If we got a null selectList, try to use ViewData to get the list of items.
            if (selectList == null)
            {
                selectList = htmlHelper.GetSelectData<TKey>(name);
                usedViewData = true;
            }

            object defaultValue = (allowMultiple)
                ? htmlHelper.GetModelStateValue(name, typeof(string[]))
                : htmlHelper.GetModelStateValue(name, typeof(string));

            // If we haven't already used ViewData to get the entire list of items then we need to
            // use the ViewData-supplied value before using the parameter-supplied value.
            if (!usedViewData)
            {
                if (defaultValue == null)
                {
                    defaultValue = htmlHelper.ViewData.Eval(name);
                }
            }

            if (defaultValue != null)
            {
                var defaultValues = (allowMultiple) ? defaultValue as IEnumerable : new[] { defaultValue };
                var values = from object value in defaultValues
                             select Convert.ToString(value, CultureInfo.CurrentCulture);
                var selectedValues = new HashSet<string>(values, StringComparer.OrdinalIgnoreCase);
                var newSelectList = new BsSelectList<TKey> { SelectedValues = selectList.SelectedValues };

                foreach (var item in selectList.Items)
                {
                    item.Selected = (item.Value != null)
                        ? selectedValues.Contains(item.Value)
                        : selectedValues.Contains(item.Text);
                    newSelectList.Items.Add(item);
                }
                selectList = newSelectList;
            }

            // Convert each ListItem to an <option> tag
            var listItemBuilder = new StringBuilder();

            // Make optionLabel the first item that gets rendered.
            if (optionLabel != null)
            {
                listItemBuilder.AppendLine(
                    ListItemToOption(new BsSelectListItem
                    {
                        Text = optionLabel,
                        Value = String.Empty,
                        Selected = false
                    }));
            }

            if (selectList.Items.Any(x => string.IsNullOrEmpty(x.GroupKey)))
            {
                //build options
                foreach (var item in selectList.Items)
                {
                    listItemBuilder.AppendLine(ListItemToOption(item));
                }
            }
            else
            {
                //build options with optgroup
                foreach (var group in selectList.Items.GroupBy(i => i.GroupKey))
                {
                    string groupName =
                        selectList.Items.Where(i => i.GroupKey == group.Key).Select(it => it.GroupName).FirstOrDefault();
                    listItemBuilder.AppendLine(string.Format("<optgroup label=\"{0}\" value=\"{1}\">", groupName, group.Key));
                    foreach (var item in group)
                    {
                        listItemBuilder.AppendLine(ListItemToOption(item));
                    }
                    listItemBuilder.AppendLine("</optgroup>");
                }
            }

            var tagBuilder = new TagBuilder("select")
            {
                InnerHtml = listItemBuilder.ToString()
            };
            tagBuilder.MergeAttributes(htmlAttributes);
            tagBuilder.MergeAttribute("name", name, true /* replaceExisting */);
            tagBuilder.AddCssClass(bsCssClass);
            tagBuilder.AddCssClass("form-control");
            tagBuilder.GenerateId(name);
            if (allowMultiple)
            {
                tagBuilder.MergeAttribute("multiple", "multiple");
            }

            // If there are any errors for a named field, we add the css attribute.
            ModelState modelState;
            if (htmlHelper.ViewData.ModelState.TryGetValue(name, out modelState))
            {
                if (modelState.Errors.Count > 0)
                {
                    tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
                }
            }

            var attributes = htmlHelper.GetUnobtrusiveValidationAttributes(name);

            if (name.Contains(".") && !attributes.Any())
            {
                attributes = htmlHelper.GetUnobtrusiveValidationAttributes(name.Split('.').LastOrDefault());
            }
            tagBuilder.MergeAttributes<string, object>(attributes);

            return MvcHtmlString.Create(tagBuilder.ToString());
        }

        private static MvcHtmlString BsRadioListInternal<TKey>(this HtmlHelper htmlHelper, string name,
            BsSelectList<TKey> radioList, IDictionary<string, object> htmlAttributes, bool allowMultiple, string bsCssClass)
        {
            //TODO: refactoring
            //bind the selected values BsSelectList.SelectedValues
            name += ".SelectedValues";

            var propertyName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            var html = new StringBuilder();
            var divTag = new TagBuilder("div");
            divTag.MergeAttribute("id", propertyName, true);
            divTag.AddCssClass(bsCssClass);
            divTag.AddCssClass("form-control");
            

            if (htmlAttributes != null)
            {
                divTag.MergeAttributes(htmlAttributes);
            }

            // Create a radio button for each item in the list
            foreach (var item in radioList.Items)
            {
                // Generate an id to be given to the radio button field
                var id = string.Format("{0}_{1}", propertyName, item.Value);

                // Create and populate a radio button using the existing html htmlHelpers
                var label = htmlHelper.Label(id, HttpUtility.HtmlEncode(item.Text));

                var radioHtmlAttributes = new Dictionary<string, object> { { "id", id } };

                if (htmlAttributes != null)
                {
                    htmlAttributes.ToList().ForEach(x =>
                    {
                        if (!radioHtmlAttributes.ContainsKey(x.Key))
                        {
                            radioHtmlAttributes.Add(x.Key, x.Value);
                        }
                    });
                }

                var radio = htmlHelper.RadioButton(name, item.Value, radioHtmlAttributes).ToHtmlString();

                // Create the html string
                // e.g. <input data-val="true" data-val-required="You must select an option" id="TestRadio_1" name="TestRadio" type="radio" value="1" /><label for="TestRadio_1">Line1</label>
                var innerDivTag = new TagBuilder("div");
                innerDivTag.AddCssClass("RadioButton");
                html.Append(innerDivTag.ToString(TagRenderMode.StartTag));
                html.Append(radio);
                html.Append(label);
                html.Append(innerDivTag.ToString(TagRenderMode.EndTag));
            }


            divTag.InnerHtml = html.ToString();

            return MvcHtmlString.Create(divTag.ToString());
        }

        private static BsSelectList<TKey> GetSelectData<TKey>(this HtmlHelper htmlHelper, string name)
        {
            object o = null;
            if (htmlHelper.ViewData != null)
            {
                o = htmlHelper.ViewData.Eval(name);
            }
            if (o == null)
            {
                throw new InvalidOperationException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        "Missing Select Data"));
            }
            var selectList = o as BsSelectList<TKey>;
            if (selectList == null)
            {
                throw new InvalidOperationException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        "Wrong Select DataType"));
            }
            return selectList;
        }

        private static string ListItemToOption(BsSelectListItem item)
        {
            var builder = new TagBuilder("option")
            {
                InnerHtml = HttpUtility.HtmlEncode(item.Text)
            };
            if (item.Value != null)
            {
                builder.Attributes["value"] = item.Value;
            }
            if (item.Selected)
            {
                builder.Attributes["selected"] = "selected";
            }

            if (item.Data != null)
                foreach (var data in item.Data)
                {
                    builder.Attributes["data-" + data.Key] = data.Value;
                }

            return builder.ToString(TagRenderMode.Normal);
        }

        #endregion
    }
}