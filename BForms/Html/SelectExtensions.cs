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
using System.Web.UI;
using BForms.Models;
using BForms.Mvc;
using BForms.Utilities;
using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Office2010.Drawing.ChartDrawing;

namespace BForms.Html
{
    /// <summary>
    /// Represents support for making selections in a list
    /// </summary>
    public static class SelectExtensions
    {
        /// <summary>
        /// Returns a BForms select element based on BsControlAttribute
        /// </summary>
        public static MvcHtmlString BsSelectFor<TModel, TKey>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, BsSelectList<TKey>>> expression)
        {
            return htmlHelper.BsSelectFor(expression, null, null);
        }

        /// <summary>
        /// Returns a BForms select element based on BsControlAttribute
        /// </summary>
        public static MvcHtmlString BsSelectFor<TModel, TKey>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, BsSelectList<TKey>>> expression, object htmlAttributes)
        {
            return htmlHelper.BsSelectFor(expression,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes),
                null);
        }

        /// <summary>
        /// Returns a BForms select element based on BsControlAttribute
        /// </summary>
        public static MvcHtmlString BsSelectFor<TModel, TKey>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, BsSelectList<TKey>>> expression, IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.BsSelectFor(expression,
                htmlAttributes,
                null);
        }

        /// <summary>
        /// Returns a BForms select element based on BsControlAttribute
        /// </summary>
        public static MvcHtmlString BsSelectFor<TModel, TKey>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, BsSelectList<TKey>>> expression, object htmlAttributes, object dataOptions)
        {
            return htmlHelper.BsSelectFor(expression,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes),
                HtmlHelper.AnonymousObjectToHtmlAttributes(dataOptions));
        }

        /// <summary>
        /// Returns a BForms select element based on BsControlAttribute
        /// </summary>
        public static MvcHtmlString BsSelectFor<TModel, TKey>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, BsSelectList<TKey>>> expression,
            IDictionary<string, object> htmlAttributes,
            IDictionary<string, object> dataOptions)
        {
            if (htmlAttributes == null)
            {
                htmlAttributes = new Dictionary<string, object>();
            }
            if (dataOptions == null)
            {
                dataOptions = new Dictionary<string, object>();
            }

            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var name = ExpressionHelper.GetExpressionText(expression);

            //get list object from expression
            var selectList = expression.Compile().Invoke(htmlHelper.ViewData.Model);

            string htmlSelect;
            string optionLabel = null;
            var allowMultiple = false;

            //copy from BsSelectList.SelectedValues to SelectListItem.Selected
            if (selectList.SelectedValues != null && selectList.Items != null && selectList.Items.Any())
            {
                var selectedValuesStr = new List<string>();
                var bsKeyType = typeof(TKey);
                if (typeof(IEnumerable).IsAssignableFrom(bsKeyType) && bsKeyType != typeof(string))
                {

                    var selectedValues = (IEnumerable)selectList.SelectedValues;
                    selectedValuesStr = (from object selectedValue in selectedValues select ReflectionHelpers.GetNonEnumerableValue(selectedValue)).ToList();
                }
                else
                {
                    selectedValuesStr.Add(ReflectionHelpers.GetNonEnumerableValue(selectList.SelectedValues));
                }


                foreach (var item in selectList.Items)
                {
                    if (selectedValuesStr.Contains(item.Value))
                    {
                        item.Selected = true;
                    }
                }
            }

            //add optionLabel from Watermark 
            if (!string.IsNullOrEmpty(metadata.Watermark))
            {
                optionLabel = metadata.Watermark;
            }

            //set data- options
            if (dataOptions.Any())
            {
                htmlAttributes.MergeAttribute("data-options", dataOptions.ToJsonString());
            }

            //determine the select type
            BsControlAttribute bsControl = null;
            if (ReflectionHelpers.TryGetAttribute(name, typeof(TModel), out bsControl))
            {
                //add bs- control type
                var bsCssClass = bsControl.ControlType.GetDescription();

                if (bsControl.IsReadonly)
                {
                    htmlAttributes.MergeAttribute("readonly", "readonly");
                }

                switch (bsControl.ControlType)
                {
                    case BsControlType.TagList:
                        allowMultiple = true;
                        htmlSelect = BsTagListInternal(htmlHelper, name,
                            selectList, optionLabel, htmlAttributes, bsCssClass, metadata).ToHtmlString();
                        break;
                    case BsControlType.CheckBoxList:
                        allowMultiple = true;
                        htmlSelect = BsRadioListInternal(htmlHelper, name,
                            selectList, htmlAttributes, allowMultiple, bsCssClass, metadata).ToHtmlString();
                        break;
                    case BsControlType.RadioButtonList:
                        allowMultiple = false;
                        htmlSelect = BsRadioListInternal(htmlHelper, name,
                            selectList, htmlAttributes, allowMultiple, bsCssClass, metadata).ToHtmlString();
                        break;
                    case BsControlType.ListBox:
                    case BsControlType.ListBoxGrouped:
                        allowMultiple = true;
                        htmlSelect = BsSelectInternal(htmlHelper, name, selectList,
                            optionLabel, htmlAttributes, allowMultiple, bsCssClass, metadata).ToHtmlString();
                        break;
                    case BsControlType.DropDownList:
                    case BsControlType.DropDownListGrouped:
                    case BsControlType.Autocomplete:
                        allowMultiple = false;
                        htmlSelect = BsSelectInternal(htmlHelper, name, selectList,
                        optionLabel, htmlAttributes, allowMultiple, bsCssClass, metadata).ToHtmlString();
                        break;
                    case BsControlType.ButtonGroupDropdown:
                        allowMultiple = false;
                        htmlSelect =
                            BsButtonGroupDropdownInternal(htmlHelper, name, selectList,
                            optionLabel, htmlAttributes, bsCssClass, metadata).ToHtmlString();
                        break;
                    default:
                        throw new ArgumentException("The " + name + " of type " + bsControl.ControlType.GetDescription() + " does not match a select element");

                }
            }
            else
            {
                throw new InvalidOperationException("The " + name + " property is not decorated with a BsControlAttribute");
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
            IDictionary<string, object> htmlAttributes, bool allowMultiple, string bsCssClass, ModelMetadata metadata = null)
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
            tagBuilder.MergeAttribute("name", name, true);
            tagBuilder.AddCssClass(bsCssClass);
            tagBuilder.AddCssClass("form-control");
            tagBuilder.GenerateId(name);
            if (allowMultiple)
            {
                tagBuilder.MergeAttribute("multiple", "multiple");
            }

            tagBuilder.BsSelectListValidation(htmlHelper, name, metadata);

            return MvcHtmlString.Create(tagBuilder.ToString());
        }

        private static MvcHtmlString BsRadioListInternal<TKey>(this HtmlHelper htmlHelper, string name,
           BsSelectList<TKey> radioList, IDictionary<string, object> htmlAttributes, bool allowMultiple, string bsCssClass, ModelMetadata metadata = null)
        {
            //TODO: refactoring
            //bind the selected values BsSelectList.SelectedValues
            name += ".SelectedValues";

            var fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            var html = new StringBuilder();
            var divTag = new TagBuilder("div");
            divTag.MergeAttribute("id", fullName, true);
            divTag.AddCssClass(bsCssClass);
            divTag.AddCssClass("form-control");


            if (htmlAttributes != null)
            {
                divTag.MergeAttributes(htmlAttributes);
            }

            // Create a radio button temporary to get the common SelectListValidation (hack to preverv them)
            var commonRadioBuilder = new TagBuilder("input");
            if (radioList.Items.Any())
            {
                commonRadioBuilder.BsSelectListValidation(htmlHelper, name, metadata);
            }

            // Create a radio button for each item in the list
            foreach (var item in radioList.Items)
            {
                // Generate an id to be given to the radio button field
                var id = string.Format("{0}_{1}", fullName, item.Value);

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

                // build radio or checkbox input
                var input = string.Empty;
                var radioBuilder = new TagBuilder("input");
                radioBuilder.MergeAttribute("name", fullName, true);
                radioBuilder.MergeAttributes(radioHtmlAttributes);
                if (item.Selected)
                {
                    radioBuilder.MergeAttribute("checked", "checked");
                }

                // merge common atributes
                radioBuilder.MergeAttributes(commonRadioBuilder.Attributes);

                if (allowMultiple)
                {
                    //render checkbox
                    radioBuilder.MergeAttribute("type", "checkbox");
                    radioBuilder.MergeAttribute("data-value", item.Value);

                    var inputItemBuilder = new StringBuilder();
                    inputItemBuilder.Append(radioBuilder.ToString(TagRenderMode.SelfClosing));

                    var hiddenInput = new TagBuilder("input");
                    hiddenInput.MergeAttribute("type", HtmlHelper.GetInputTypeString(InputType.Hidden));
                    hiddenInput.MergeAttribute("name", fullName);
                    hiddenInput.MergeAttribute("value", "false");
                    inputItemBuilder.Append(hiddenInput.ToString(TagRenderMode.SelfClosing));
                    input = inputItemBuilder.ToString();
                }
                else
                {
                    //render radio
                    radioBuilder.MergeAttribute("type", "radio");
                    radioBuilder.MergeAttribute("value", item.Value);
                    input = radioBuilder.ToString(TagRenderMode.SelfClosing);
                }

                // Create the html string
                // e.g. <input data-val="true" data-val-required="You must select an option" id="TestRadio_1" name="TestRadio" type="radio" value="1" /><label for="TestRadio_1">Line1</label>
                var innerDivTag = new TagBuilder("div");
                //innerDivTag.AddCssClass("RadioButton");
                html.Append(innerDivTag.ToString(TagRenderMode.StartTag));
                html.Append(input);
                html.Append(label);
                html.Append(innerDivTag.ToString(TagRenderMode.EndTag));
            }

            var buttonGroupContainer = new TagBuilder("div");

            buttonGroupContainer.AddCssClass("checkbox_replace form-control");
            buttonGroupContainer.MergeAttribute("id", fullName + "_checkBox");
            buttonGroupContainer.MergeAttribute("tabindex", 0.ToString());

            var buttonGroupDiv = new TagBuilder("div");
            buttonGroupDiv.AddCssClass("btn-group-justified");

            foreach (var item in radioList.Items)
            {
                var buttonA = new TagBuilder("a");
                buttonA.AddCssClass("option bs-buttonGroupItem");
                buttonA.MergeAttribute("data-value", item.Value);

                if (item.Selected)
                {
                    buttonA.AddCssClass("selected");
                }

                buttonA.InnerHtml += HttpUtility.HtmlEncode(item.Text);

                buttonGroupDiv.InnerHtml += buttonA;
            }

            buttonGroupContainer.InnerHtml += buttonGroupDiv;
            
            divTag.InnerHtml = html.ToString();
            divTag.MergeAttribute("style", "display:none");

            return MvcHtmlString.Create(divTag.ToString() + buttonGroupContainer.ToString());
        }

        private static MvcHtmlString BsTagListInternal<TKey>(this HtmlHelper htmlHelper, string name,
            BsSelectList<TKey> selectList, string optionLabel,
            IDictionary<string, object> htmlAttributes, string bsCssClass, ModelMetadata metadata = null)
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
            tagBuilder.MergeAttribute("name", name, true);
            tagBuilder.AddCssClass(bsCssClass);
            tagBuilder.AddCssClass("form-control");
            tagBuilder.GenerateId(name);
            if (allowMultiple)
            {
                tagBuilder.MergeAttribute("multiple", "multiple");
            }

            tagBuilder.BsSelectListValidation(htmlHelper, name, metadata);

            return MvcHtmlString.Create(tagBuilder.ToString());
        }

        private static MvcHtmlString BsButtonGroupDropdownInternal<TKey>(this HtmlHelper htmlHelper, string name,
           BsSelectList<TKey> selectList, string optionLabel, IDictionary<string, object> htmlAttributes, string bsCssClass, ModelMetadata metadata = null)
        {
            name += ".SelectedValues";
            name = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);

            bool usedViewData = false;
            object defaultValue = htmlHelper.GetModelStateValue(name, typeof(string));

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
                var defaultValues = new[] { defaultValue };
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
            tagBuilder.MergeAttribute("name", name, true);
            tagBuilder.AddCssClass(bsCssClass);
            tagBuilder.AddCssClass("form-control");
            tagBuilder.GenerateId(name);

            tagBuilder.BsSelectListValidation(htmlHelper, name, metadata);

            tagBuilder.MergeAttribute("style", "display:none");

            #region render button dropdown
            var btnGroup = new TagBuilder("div");
            btnGroup.AddCssClass("btn-group bs-buttonGroupDropdownContainer btn-group-justified");

            var button = new TagBuilder("a");
            button.AddCssClass("btn btn-default dropdown-toggle bs-buttonGroupDropdownToggle");
            button.MergeAttribute("href", "#");
            button.MergeAttribute("data-toggle", "dropdown");
            button.MergeAttribute("data-dropdown-for", tagBuilder.Attributes["id"]);
            button.MergeAttribute("data-placeholder", optionLabel + " ");

            var selectedValue = selectList.Items.FirstOrDefault(x => x.Selected);

            if (selectedValue != null)
            {
                button.InnerHtml += selectedValue.Text + " ";
            }
            else
            {
                button.InnerHtml += optionLabel + " ";
            }

            var carretSpan = new TagBuilder("span");
            carretSpan.AddCssClass("caret");

            button.InnerHtml += carretSpan.ToString();

            btnGroup.InnerHtml += button;

            var dropdownUl = new TagBuilder("ul");
            dropdownUl.AddCssClass("dropdown-menu bs-dropdownList");
            dropdownUl.MergeAttribute("role", "menu");

            var dropdownListItemBuilder = new StringBuilder();

            foreach (var item in selectList.Items)
            {
                dropdownListItemBuilder.AppendLine(ListItemToLi(item));
            }

            dropdownUl.InnerHtml += dropdownListItemBuilder.ToString();

            btnGroup.InnerHtml += dropdownUl;
            #endregion

            return MvcHtmlString.Create(tagBuilder.ToString() + btnGroup.ToString());
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

        internal static string ListItemToLi(BsSelectListItem item)
        {
            var liBuilder = new TagBuilder("li");

            var aBuilder = new TagBuilder("a")
            {
                InnerHtml = HttpUtility.HtmlEncode(item.Text)
            };

            aBuilder.MergeAttribute("href", "#");
            aBuilder.AddCssClass("bs-buttonGroupDropdownOption");

            if (item.Value != null)
            {
                aBuilder.MergeAttribute("data-value", item.Value);
            }

            if (item.Selected)
            {
                aBuilder.MergeAttribute("data-selected", "true");
                aBuilder.AddCssClass("mark");
            }

            if (item.Data != null)
            {
                foreach (var data in item.Data)
                {
                    aBuilder.MergeAttribute("data-" + data.Key, data.Value);
                }
            }

            liBuilder.InnerHtml += aBuilder.ToString();

            return liBuilder.ToString();
        }

        internal static void BsSelectListValidation(this TagBuilder tagBuilder, HtmlHelper htmlHelper, string name, ModelMetadata metadata = null)
        {
            // If there are any errors for a named field, we add the css attribute.
            ModelState modelState;
            if (htmlHelper.ViewData.ModelState.TryGetValue(name, out modelState))
            {
                if (modelState.Errors.Count > 0)
                {
                    tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
                }
            }

            //Validation Hack
            name = name.Replace(".SelectedValues", "");

            var attributes = htmlHelper.GetUnobtrusiveValidationAttributes(name, metadata);

            if (name.Contains(".") && !attributes.Any())
            {
                attributes = htmlHelper.GetUnobtrusiveValidationAttributes(name.Split('.').LastOrDefault());
            }

            tagBuilder.MergeAttributes(attributes);
        }

        public static MvcHtmlString BsSelectList<TKey>(this HtmlHelper helper, BsSelectList<TKey> selectList, BsControlType controlType, string name, bool allowMultiple, Dictionary<string, object> htmlAttributes, ModelMetadata metadata = null)
        {
            return BsSelectInternal(helper, name, selectList, null, htmlAttributes, allowMultiple, controlType.GetDescription(), null);
        }

        public static MvcHtmlString BsTagList<TKey>(this HtmlHelper helper, BsSelectList<TKey> selectList, BsControlType controlType, string name, Dictionary<string, object> htmlAttributes, ModelMetadata metadata = null)
        {
            return BsTagListInternal(helper, name, selectList, null, htmlAttributes, controlType.GetDescription(), null);
        }

        public static MvcHtmlString BsRadioList<TKey>(this HtmlHelper htmlHelper, BsSelectList<TKey> radioList, BsControlType controlType, string name, IDictionary<string, object> htmlAttributes, bool allowMultiple, ModelMetadata metadata = null)
        {
            return BsRadioListInternal(htmlHelper, name, radioList, htmlAttributes, allowMultiple, controlType.GetDescription(), metadata);
        }
    }
}