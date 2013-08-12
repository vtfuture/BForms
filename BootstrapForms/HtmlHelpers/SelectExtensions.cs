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

namespace BootstrapForms.HtmlHelpers
{
    /// <summary>
    /// Represents support for making selections in a list
    /// </summary>
    public static class SelectExtensions
    {
        /// <summary>
        /// Returns a list of radio buttons
        /// </summary>
        public static MvcHtmlString BsRadioButtonListFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> radioList, IDictionary<string, object> htmlAttributes)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var html = new StringBuilder();
            var propertyName = (helper.ViewData.TemplateInfo.HtmlFieldPrefix != string.Empty ? (helper.ViewData.TemplateInfo.HtmlFieldPrefix + ".") : string.Empty) + metadata.PropertyName;

            var divTag = new TagBuilder("div");
            divTag.MergeAttribute("id", propertyName);
            divTag.MergeAttribute("class", "RadioButtonsContainer form-control");

            if (htmlAttributes != null)
            {
                divTag.MergeAttributes(htmlAttributes);
            }

            // Create a radio button for each item in the list
            foreach (SelectListItem item in radioList)
            {
                // Generate an id to be given to the radio button field
                var id = string.Format("{0}_{1}", propertyName, item.Value);

                // Create and populate a radio button using the existing html helpers
                var label = helper.Label(id, HttpUtility.HtmlEncode(item.Text));

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

                var radio = helper.RadioButtonFor(expression, item.Value, radioHtmlAttributes).ToHtmlString();

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
                description = helper.BsDescriptionFor(expression);
            }

            return MvcHtmlString.Create(divTag.ToString() + description);
        }
        public static MvcHtmlString BsRadioButtonListFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> radioList)
        {
            return BsRadioButtonListFor(helper, expression, radioList, (object)null);
        }
        public static MvcHtmlString BsRadioButtonListFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> radioList, object htmlAttributes)
        {
            return BsRadioButtonListFor(helper, expression, radioList, new RouteValueDictionary(htmlAttributes));
        }

        /// <summary>
        /// Returns a list of tags with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsTagListFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);

            //merge custom css classes with bootstrap
            htmlAttributes.BsMergeAttribute("class", "form-control");
            htmlAttributes.BsMergeAttribute("data-stringifyTags", "true");

            //add placeholder
            if (!string.IsNullOrEmpty(metadata.Watermark) && !htmlAttributes.ContainsKey("placeholder"))
            {
                htmlAttributes.BsMergeAttribute("placeholder", metadata.Watermark);
            }

            //add info tooltip
            var description = new MvcHtmlString("");
            if (!string.IsNullOrEmpty(metadata.Description))
            {
                description = helper.BsDescriptionFor(expression);
            }

            return MvcHtmlString.Create(helper.HiddenFor(expression, htmlAttributes).ToHtmlString() + description.ToHtmlString());
        }
        public static MvcHtmlString BsTagListFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression)
        {
            return BsTagListFor(helper, expression, (object)null);
        }
        public static MvcHtmlString BsTagListFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return BsTagListFor(helper, expression, new RouteValueDictionary(htmlAttributes));
        }

        /// <summary>
        /// Returns a DropDownList element with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel, IDictionary<string, object> htmlAttributes)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);

            //merge custom css classes with bootstrap
            htmlAttributes.BsMergeAttribute("class", "form-control");

            //add info tooltip
            var description = new MvcHtmlString("");
            if (!string.IsNullOrEmpty(metadata.Description))
            {
                description = helper.BsDescriptionFor(expression);
            }

            return MvcHtmlString.Create(helper.DropDownListFor(expression, selectList, optionLabel, htmlAttributes).ToHtmlString() + description.ToHtmlString());
        }
        public static MvcHtmlString BsDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel)
        {
            return BsDropDownListFor(helper, expression, selectList, optionLabel, (object)null);
        }
        public static MvcHtmlString BsDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel, object htmlAttributes)
        {
            return BsDropDownListFor(helper, expression, selectList, optionLabel, new RouteValueDictionary(htmlAttributes));
        }
        public static MvcHtmlString BsDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, IDictionary<string, object> htmlAttributes)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            string htmlSelect;

            //merge custom css classes with bootstrap
            htmlAttributes.BsMergeAttribute("class", "form-control");

            //add optionLabel from Disply Watermark
            if (!string.IsNullOrEmpty(metadata.Watermark))
            {
                htmlSelect = helper.DropDownListFor(expression, selectList, metadata.Watermark, htmlAttributes).ToHtmlString();
            }
            else
            {
                htmlSelect = helper.DropDownListFor(expression, selectList, htmlAttributes).ToHtmlString();
            }

            //add info tooltip
            var description = new MvcHtmlString("");
            if (!string.IsNullOrEmpty(metadata.Description))
            {
                description = helper.BsDescriptionFor(expression);
            }

            return MvcHtmlString.Create(htmlSelect + description.ToHtmlString());
        }
        public static MvcHtmlString BsDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList)
        {
            return BsDropDownListFor(helper, expression, selectList, (object)null);
        }
        public static MvcHtmlString BsDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, object htmlAttributes)
        {
            return BsDropDownListFor(helper, expression, selectList, new RouteValueDictionary(htmlAttributes));
        }

        /// <summary>
        /// Returns a DropDown grouped list element with placeholder and info tooltip
        /// </summary>
        public static MvcHtmlString BsDropDownGroupedListFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, IEnumerable<BsGroupedSelectListItem> selectList, string optionLabel, IDictionary<string, object> htmlAttributes, bool allowMultiple = false)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            string htmlSelect;

            //copy watermark to optionLabel
            if (!string.IsNullOrEmpty(metadata.Watermark))
            {
                htmlSelect = BsDropDownListHelper(helper, ExpressionHelper.GetExpressionText(expression), selectList, metadata.Watermark, htmlAttributes, allowMultiple).ToHtmlString();
            }
            else
            {
                htmlSelect = BsDropDownListHelper(helper, ExpressionHelper.GetExpressionText(expression), selectList, optionLabel, htmlAttributes, allowMultiple).ToHtmlString();
            }

            //add info tooltip
            var description = new MvcHtmlString("");
            if (!string.IsNullOrEmpty(metadata.Description))
            {
                description = helper.BsDescriptionFor(expression);
            }

            return MvcHtmlString.Create(htmlSelect + description.ToHtmlString());
        }
        public static MvcHtmlString BsDropDownGroupedListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<BsGroupedSelectListItem> selectList, bool allowMultiple = false)
        {
            return BsDropDownGroupedListFor(htmlHelper, expression, selectList, null /* optionLabel */, null /* htmlAttributes */, allowMultiple);
        }
        public static MvcHtmlString BsDropDownGroupedListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<BsGroupedSelectListItem> selectList, object htmlAttributes, bool allowMultiple = false)
        {
            return BsDropDownGroupedListFor(htmlHelper, expression, selectList, null /* optionLabel */, new RouteValueDictionary(htmlAttributes), allowMultiple);
        }
        public static MvcHtmlString BsDropDownGroupedListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<BsGroupedSelectListItem> selectList, IDictionary<string, object> htmlAttributes, bool allowMultiple = false)
        {
            return BsDropDownGroupedListFor(htmlHelper, expression, selectList, null /* optionLabel */, htmlAttributes, allowMultiple);
        }
        public static MvcHtmlString BsDropDownGroupedListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<BsGroupedSelectListItem> selectList, string optionLabel, bool allowMultiple = false)
        {
            return BsDropDownGroupedListFor(htmlHelper, expression, selectList, optionLabel, null /* htmlAttributes */, allowMultiple);
        }
        public static MvcHtmlString BsDropDownGroupedListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<BsGroupedSelectListItem> selectList, string optionLabel, object htmlAttributes, bool allowMultiple = false)
        {
            return BsDropDownGroupedListFor(htmlHelper, expression, selectList, optionLabel, new RouteValueDictionary(htmlAttributes), allowMultiple);
        }

        #region DropdownGrouped Helpers
        private static MvcHtmlString BsDropDownListHelper(HtmlHelper htmlHelper, string expression, IEnumerable<BsGroupedSelectListItem> selectList, string optionLabel, IDictionary<string, object> htmlAttributes, bool allowMultiple)
        {
            return SelectInternal(htmlHelper, optionLabel, expression, selectList, allowMultiple, htmlAttributes);
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

        internal static string ListItemToOption(BsGroupedSelectListItem item)
        {
            TagBuilder builder = new TagBuilder("option")
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

        private static MvcHtmlString SelectInternal(this HtmlHelper htmlHelper, string optionLabel, string name, IEnumerable<BsGroupedSelectListItem> selectList, bool allowMultiple, IDictionary<string, object> htmlAttributes)
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

            object defaultValue = (allowMultiple) ? htmlHelper.GetModelStateValue(name, typeof(string[])) : htmlHelper.GetModelStateValue(name, typeof(string));

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
                IEnumerable defaultValues = (allowMultiple) ? defaultValue as IEnumerable : new[] { defaultValue };
                IEnumerable<string> values = from object value in defaultValues select Convert.ToString(value, CultureInfo.CurrentCulture);
                HashSet<string> selectedValues = new HashSet<string>(values, StringComparer.OrdinalIgnoreCase);
                List<BsGroupedSelectListItem> newSelectList = new List<BsGroupedSelectListItem>();

                foreach (BsGroupedSelectListItem item in selectList)
                {
                    item.Selected = (item.Value != null) ? selectedValues.Contains(item.Value) : selectedValues.Contains(item.Text);
                    newSelectList.Add(item);
                }
                selectList = newSelectList;
            }

            // Convert each ListItem to an <option> tag
            StringBuilder listItemBuilder = new StringBuilder();

            // Make optionLabel the first item that gets rendered.
            if (optionLabel != null)
            {
                listItemBuilder.AppendLine(ListItemToOption(new BsGroupedSelectListItem() { Text = optionLabel, Value = String.Empty, Selected = false }));
            }

            foreach (var group in selectList.GroupBy(i => i.GroupKey))
            {
                string groupName = selectList.Where(i => i.GroupKey == group.Key).Select(it => it.GroupName).FirstOrDefault();
                listItemBuilder.AppendLine(string.Format("<optgroup label=\"{0}\" value=\"{1}\">", groupName, group.Key));
                foreach (BsGroupedSelectListItem item in group)
                {
                    listItemBuilder.AppendLine(ListItemToOption(item));
                }
                listItemBuilder.AppendLine("</optgroup>");
            }

            TagBuilder tagBuilder = new TagBuilder("select")
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

        internal static object GetModelStateValue(this HtmlHelper helper, string key, Type destinationType)
        {
            ModelState modelState;
            if (helper.ViewData.ModelState.TryGetValue(key, out modelState))
            {
                if (modelState.Value != null)
                {
                    return modelState.Value.ConvertTo(destinationType, null /* culture */);
                }
            }
            return null;
        }
        #endregion
    }

    public class BsGroupedSelectListItem : SelectListItem
    {
        public string GroupKey { get; set; }
        public string GroupName { get; set; }
    }

    public class BsChosenItem
    {
        public int? Id { get; set; }
        public string Val { get; set; }
        public string Group { get; set; }
    }
    public class BsChosenTags
    {
        public List<string> Choices { get; set; }
    }
}