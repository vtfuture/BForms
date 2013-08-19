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
            return htmlHelper.BsRadioButtonListInternal(metadata, name, radioList, "", htmlAttributes, "bs-radio-list");
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
            var bsCssClass = string.Empty;
            if (ReflectionHelpers.TryGetControlAttribute(ExpressionHelper.GetExpressionText(expression), typeof(TModel), out bsControl))
            {
                bsCssClass = bsControl.ControlType.GetDescription();
            }

            return htmlHelper.BsRadioButtonListInternal(metadata, metadata.PropertyName, radioList, "", htmlAttributes, bsCssClass);
        }

        private static MvcHtmlString BsRadioButtonListInternal(this HtmlHelper htmlHelper, ModelMetadata metadata,
            string name, IEnumerable<SelectListItem> radioList, string optionLabel, IDictionary<string, object> htmlAttributes, string bsCssClass)
        {
            var propertyName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            var bsList = BsSelectList<List<string>>.FromSelectList(radioList.ToList());

            var htmlList = htmlHelper.BsRadioListInternal<List<string>>(name, bsList, htmlAttributes, false, bsCssClass);

            //add info tooltip
            var description = new MvcHtmlString("");
            if (!string.IsNullOrEmpty(metadata.Description))
            {
                description = htmlHelper.BsDescription(name);
            }

            return MvcHtmlString.Create(htmlList.ToHtmlString() + description);
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
            var name = ExpressionHelper.GetExpressionText(expression);

            //get list object from expression
            var selectList = expression.Compile().Invoke(htmlHelper.ViewData.Model);

            string htmlSelect;
            string optionLabel = null;
            var allowMultiple = false;

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
                        htmlSelect = BsTagListInternal(htmlHelper, name,
                            selectList, optionLabel, htmlAttributes, bsCssClass).ToHtmlString();
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
                    case BsControlType.Autocomplete:
                        allowMultiple = false;
                        htmlSelect = BsSelectInternal(htmlHelper, name, selectList,
                        optionLabel, htmlAttributes, allowMultiple, bsCssClass).ToHtmlString();
                        break;
                    default:
                        throw new Exception(bsControl.ControlType.GetDescription() + " does not match a select element");
                        break;
                        
                }
            }
            else
            {
                throw new Exception("The " + name + " property is not decorated with a BsControlAttribute");
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

        private static MvcHtmlString BsTagListInternal<TKey>(this HtmlHelper htmlHelper, string name,
            BsSelectList<TKey> selectList, string optionLabel,
            IDictionary<string, object> htmlAttributes, string bsCssClass)
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
            bool allowMultiple = true;
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

            //build options
            foreach (var item in selectList.Items)
            {
                listItemBuilder.AppendLine(ListItemToOption(item));
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

        internal static BsSelectList<TKey> GetSelectData<TKey>(this HtmlHelper htmlHelper, string name)
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

        internal static string ListItemToOption(BsSelectListItem item)
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