using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using BForms.Html;
using BForms.Models;
using BForms.Mvc;
using BForms.Utilities;

namespace BForms.FormBuilder
{
    public class BsFormHtmlRenderer
    {
        private HtmlHelper _helper;

        public BsFormHtmlRenderer(HtmlHelper helper)
        {
            _helper = helper;
        }

        public string RenderForm<TFormModel>(TFormModel formModel, BsTheme theme = BsTheme.Default, bool wrapInForm = true, string propertyName = null) where TFormModel : class
        {
            if (formModel == null)
            {
                throw new Exception("The form model passed as an argument cannot be null");
            }

            var formString = String.Empty;
            var formButtonsString = String.Empty;

            var formBuilder = new TagBuilder("form");
            var formContainerBuilder = new TagBuilder("div");

            formBuilder.MergeAttribute("novalidate", "novalidate");
            formContainerBuilder.AddCssClass("form_container " + theme.GetDescription());

            var properties = formModel.GetType().GetProperties();

            var bsControlProperties = from prop in formModel.GetType().GetProperties()
                                      let bsControl = prop.GetCustomAttributes(typeof(BsControlAttribute), true)
                                      let required = prop.GetCustomAttributes(typeof(RequiredAttribute), true)
                                      let display = prop.GetCustomAttributes(typeof(DisplayAttribute), true)
                                      let formGroup = prop.GetCustomAttributes(typeof(FormGroup), true)
                                      where bsControl.Length == 1
                                      select new FormControlPropertyMetadata
                                      {
                                          PropertyInfo = prop,
                                          FormGroupAttribute = formGroup.FirstOrDefault() as FormGroup,
                                          RequiredAttribute = required.FirstOrDefault() as RequiredAttribute,
                                          DisplayAttribute = display.FirstOrDefault() as DisplayAttribute,
                                          BsControlAttribute = bsControl.FirstOrDefault() as BsControlAttribute
                                      };

            foreach (var property in bsControlProperties)
            {
                formString += RenderFormGroup(property, formModel);
            }

            var buttonsProperties = from prop in formModel.GetType().GetProperties()
                where prop.GetCustomAttributes(typeof (FormButtons), true).Any()
                select prop;

            var buttons = buttonsProperties.Select(prop => prop.GetValue(formModel)).FirstOrDefault();

            if (buttons != null)
            {
                var buttonsList = buttons as IEnumerable<BsButtonModel>;

                if (buttonsList == null)
                {
                    throw new Exception("Properties decorated with the BsButtonModel attribute must be of type IEnumerable<BsButtonModel>");
                }

                formButtonsString = RenderFormButtons(buttonsList);
            }

            formContainerBuilder.InnerHtml = formString + formButtonsString;

            if (wrapInForm)
            {
                formBuilder.InnerHtml = formContainerBuilder.ToString();

                formBuilder.MergeAttribute("data-property-name", propertyName);

                return formBuilder.ToString();
            }

            return formContainerBuilder.ToString();
        }

        public string RenderFormGroup<TFormModel>(FormControlPropertyMetadata propertyMetadata, TFormModel formModel)
        {
            var controlHtml = RenderFormControl(propertyMetadata, formModel);

            var formGroupBuilder = new TagBuilder("div");
            var inputGroupBuilder = new TagBuilder("div");

            var width = propertyMetadata.FormGroupAttribute != null
                ? propertyMetadata.FormGroupAttribute.Width
                : ColumnWidth.Large;

            formGroupBuilder.AddCssClass("form-group " + width.GetDescription());
            inputGroupBuilder.AddCssClass("input-group");

            var glyphiconAddonHtml = String.Empty;

            if (propertyMetadata.FormGroupAttribute != null && propertyMetadata.FormGroupAttribute.GlyphiconAddon != Glyphicon.Custom)
            {
                var glyphicon = propertyMetadata.FormGroupAttribute.GlyphiconAddon;

                var glyphiconAddonbuilder = new TagBuilder("span");

                glyphiconAddonbuilder.AddCssClass("input-group-addon glyphicon " + glyphicon.GetDescription());

                glyphiconAddonHtml = glyphiconAddonbuilder.ToString();
            }

            inputGroupBuilder.InnerHtml = glyphiconAddonHtml + controlHtml;
            formGroupBuilder.InnerHtml = RenderBsLabel(propertyMetadata, formModel) + inputGroupBuilder.ToString();

            return formGroupBuilder.ToString();
        }

        public string RenderFormGroup(FormBuilderControl control, string controlHtml)
        {
            var formGroupBuilder = new TagBuilder("div");
            var inputGroupBuilder = new TagBuilder("div");

            var width = control.DefaultProperties != null && control.DefaultProperties.Width != null
                ? control.DefaultProperties.Width.SelectedValues
                : ColumnWidth.Large;

            formGroupBuilder.AddCssClass("form-group " + width.GetDescription());
            inputGroupBuilder.AddCssClass("input-group");

            var glyphiconAddonHtml = String.Empty;

            if (control.DefaultProperties != null && control.DefaultProperties.GlyphiconAddon != null)
            {
                var glyphicon = control.DefaultProperties.GlyphiconAddon.SelectedValues;

                if(glyphicon != null)
                {
                    var glyphiconAddonbuilder = new TagBuilder("span");

                    glyphiconAddonbuilder.AddCssClass("input-group-addon glyphicon " + glyphicon.GetDescription());

                    glyphiconAddonHtml = glyphiconAddonbuilder.ToString();
                }
            }

            inputGroupBuilder.InnerHtml = glyphiconAddonHtml + controlHtml;
            formGroupBuilder.InnerHtml = RenderBsLabel(control) + inputGroupBuilder.ToString();

            return formGroupBuilder.ToString();
        }

        public string RenderBsLabel(FormBuilderControl control)
        {
            var labelBuilder = new TagBuilder("label");
            var name = control.DefaultProperties.Name;
            var displayName = control.DefaultProperties.Label;

            switch (control.Type)
            {
                case FormBuilderControlType.SingleSelect:
                case FormBuilderControlType.ListBox:
                case FormBuilderControlType.RadioButtonList:
                case FormBuilderControlType.TagList:
                    {
                        name = name + ".SelectedValues";

                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            var id = GetIdAttributeFromName(name);

            var required = control.DefaultProperties.Required != null && control.DefaultProperties.Required.SelectedValues == YesNoValues.Yes;

            labelBuilder.AddCssClass("control-label " + (required ? "required" : ""));
            labelBuilder.MergeAttribute("for", id);

            labelBuilder.InnerHtml = displayName;

            return labelBuilder.ToString();
        }

        public string RenderFormControl<TFormModel, TProperty>(Expression<Func<TFormModel, TProperty>> propertySelector, TFormModel formModel)
        {
            return String.Empty;
        }

        internal string RenderFormControl<TFormModel>(FormControlPropertyMetadata propertyMetadata, TFormModel formModel)
        {
            var controlString = String.Empty;

            var bsControlType = propertyMetadata.BsControlAttribute.ControlType;

            var controlName = propertyMetadata.PropertyInfo.Name;
            var controlId = GetIdAttributeFromName(controlName);
            var controlValue = propertyMetadata.PropertyInfo.GetValue(formModel);
            var isRequired = propertyMetadata.RequiredAttribute != null;
            var requiredMessage = isRequired ? propertyMetadata.RequiredAttribute.ErrorMessage : String.Empty;

            switch (bsControlType)
            {
                case BsControlType.TextBox:
                    {
                        controlString = _helper.BsTextBox(controlName, controlValue).ToString();

                        break;
                    }
                case BsControlType.TextArea:
                    {
                        controlString = _helper.BsTextArea(controlName, controlValue).ToString();

                        break;
                    }
                case BsControlType.DropDownList:
                    {
                        controlString = RenderBsDropdown(propertyMetadata, formModel);

                        break;
                    }
                case BsControlType.ListBox:
                    {
                        controlString = RenderBsListBox(propertyMetadata, formModel);

                        break;
                    }
                case BsControlType.TagList:
                    {
                        controlString = RenderBsTagList(propertyMetadata, formModel);

                        break;
                    }
                case BsControlType.RadioButtonList:
                    {
                        controlString = RenderRadioButtonList(propertyMetadata, formModel);

                        break;
                    }
                case BsControlType.NumberInline:
                    {
                        controlString = RenderBsNumberPickerInline(propertyMetadata, formModel);

                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            return controlString;
        }

        internal string RenderFormButtons(IEnumerable<BsButtonModel> buttons)
        {
            var buttonsString = String.Empty;

            if (buttons != null)
            {
                foreach (var button in buttons)
                {
                    buttonsString += button.ToString();
                }
            }

            var buttonsContainerBuilder = new TagBuilder("div");

            buttonsContainerBuilder.AddCssClass("col-lg-12 col-md-12 col-sm-12 form-group");

            buttonsContainerBuilder.InnerHtml = buttonsString;

            return buttonsContainerBuilder.ToString();
        }

        #region Individual rendering methods

        public string RenderBsLabel<TFormModel>(FormControlPropertyMetadata propertyMetadata, TFormModel formModel)
        {
            var labelBuilder = new TagBuilder("label");
            var name = propertyMetadata.PropertyInfo.Name;
            var displayName = propertyMetadata.DisplayAttribute.Name;

            switch (propertyMetadata.BsControlAttribute.ControlType)
            {
                case BsControlType.DropDownList:
                case BsControlType.ListBox:
                case BsControlType.TagList:
                    {
                        name = name + ".SelectedValues";

                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            var id = GetIdAttributeFromName(name);

            labelBuilder.AddCssClass("control-label " + (propertyMetadata.RequiredAttribute != null ? "required" : ""));
            labelBuilder.MergeAttribute("for", id);

            labelBuilder.InnerHtml = displayName;

            return labelBuilder.ToString();
        }

        public string RenderBsDropdown<TFormModel>(FormControlPropertyMetadata propertyMetadata, TFormModel formModel, object value = null)
        {
            var controlValue = value ?? propertyMetadata.PropertyInfo.GetValue(formModel);

            var bsSelectlistItems = controlValue.GetType().GetProperty("Items").GetValue(controlValue, null) as List<BsSelectListItem>;

            var selectedValues = controlValue.GetType().GetProperty("SelectedValues").GetValue(controlValue, null) as string;

            var bsSelectList = new BsSelectList<string>
            {
                Items = bsSelectlistItems,
                SelectedValues = selectedValues
            };

            return _helper.BsSelectList<string>(bsSelectList, propertyMetadata.BsControlAttribute.ControlType, propertyMetadata.PropertyInfo.Name, false, null).ToString();
        }

        public string RenderBsListBox<TFormModel>(FormControlPropertyMetadata propertyMetadata, TFormModel formModel, object value = null)
        {
            var controlValue = value ?? propertyMetadata.PropertyInfo.GetValue(formModel);

            var bsSelectlistItems = controlValue.GetType().GetProperty("Items").GetValue(controlValue, null) as List<BsSelectListItem>;

            var selectedValues = controlValue.GetType().GetProperty("SelectedValues").GetValue(controlValue, null) as List<string>;

            var bsSelectList = new BsSelectList<List<string>>
            {
                Items = bsSelectlistItems,
                SelectedValues = selectedValues
            };

            return _helper.BsSelectList<List<string>>(bsSelectList, propertyMetadata.BsControlAttribute.ControlType, propertyMetadata.PropertyInfo.Name, true, null).ToString();
        }

        public string RenderBsTagList<TFormModel>(FormControlPropertyMetadata propertyMetadata, TFormModel formModel, object value = null)
        {
            var controlValue = value ?? propertyMetadata.PropertyInfo.GetValue(formModel);

            var bsSelectlistItems = controlValue.GetType().GetProperty("Items").GetValue(controlValue, null) as List<BsSelectListItem>;

            var selectedValues = controlValue.GetType().GetProperty("SelectedValues").GetValue(controlValue, null) as List<string>;

            var bsSelectList = new BsSelectList<List<string>>
            {
                Items = bsSelectlistItems,
                SelectedValues = selectedValues
            };

            return _helper.BsTagList<List<string>>(bsSelectList, propertyMetadata.BsControlAttribute.ControlType, propertyMetadata.PropertyInfo.Name, null).ToString();
        }

        public string RenderRadioButtonList<TFormModel>(FormControlPropertyMetadata propertyMetadata, TFormModel formModel, object value = null)
        {
            var controlValue = value ?? propertyMetadata.PropertyInfo.GetValue(formModel);

            var bsSelectlistItems = controlValue.GetType().GetProperty("Items").GetValue(controlValue, null) as List<BsSelectListItem>;

            var selectedValues = controlValue.GetType().GetProperty("SelectedValues").GetValue(controlValue, null) as string;

            var bsSelectList = new BsSelectList<string>
            {
                Items = bsSelectlistItems,
                SelectedValues = selectedValues
            };

            return _helper.BsRadioList<string>(bsSelectList, propertyMetadata.BsControlAttribute.ControlType, propertyMetadata.PropertyInfo.Name, null, false).ToString();
        }

        public string RenderBsNumberPickerInline<TFormModel>(FormControlPropertyMetadata propertyMetadata, TFormModel formModel)
        {
            var controlName = propertyMetadata.PropertyInfo.Name;
            var controlId = GetIdAttributeFromName(controlName);
            var controlValue = propertyMetadata.PropertyInfo.GetValue(formModel);

            var textInputBuilder = new TagBuilder("input");
            var numberInputBuilder = new TagBuilder("input");

            textInputBuilder.AddCssClass("form-control bs-number-inline bs-number-single_range_inline");
            textInputBuilder.MergeAttribute("name", controlName + ".TextValue");
            textInputBuilder.MergeAttribute("id", controlId + "_TextValue");
            textInputBuilder.MergeAttribute("type", "text");
            textInputBuilder.MergeAttribute("value", controlValue.ToString());

            numberInputBuilder.AddCssClass("bs-number-value");
            numberInputBuilder.MergeAttribute("data-for", controlName + ".TextValue");
            numberInputBuilder.MergeAttribute("name", controlName + ".ItemValue");
            numberInputBuilder.MergeAttribute("id", controlId + "_ItemValue");
            numberInputBuilder.MergeAttribute("type", "hidden");
            numberInputBuilder.MergeAttribute("value", controlValue.ToString());

            return textInputBuilder.ToString() + numberInputBuilder.ToString();
        }

        #endregion

        #region Render form iteratively

        public string RenderControls(IEnumerable<FormBuilderControl> controls)
        {
            var controlsString = String.Empty;

            foreach(var control in controls)
            {
                var controlString = String.Empty;

                switch(control.Type)
                {
                    case FormBuilderControlType.Textbox:
                        {
                            var inputControl = control as InputControlModel;

                            controlString = _helper.BsTextBox(inputControl.Name, inputControl.Properties.InitialValue).ToString();

                            break;
                        }
                    case FormBuilderControlType.Textarea:
                        {
                            var textareaControl = control as TextAreaControlModel;

                            controlString = _helper.BsTextBox(textareaControl.Name, String.Empty).ToString();

                            break;
                        }
                    case FormBuilderControlType.SingleSelect:
                        {
                            var selectControl = control as SingleSelectControlModel;

                            var bsSelectList = new BsSelectList<string>
                            {
                                Items = selectControl.Properties.Items.SelectedValues.Select(x => new BsSelectListItem
                                {
                                    Text = x,
                                    Value = x
                                }).ToList()
                            };                                

                            controlString = _helper.BsSelectList<string>(bsSelectList, BsControlType.DropDownList, selectControl.Name, false, null).ToString();

                            break;
                        }
                    case FormBuilderControlType.ListBox:
                        {
                            var selectControl = control as ListBoxControlModel;

                            var bsSelectList = new BsSelectList<string>
                            {
                                Items = selectControl.Properties.Items.SelectedValues.Select(x => new BsSelectListItem
                                {
                                    Text = x,
                                    Value = x
                                }).ToList()
                            };                                

                            controlString = _helper.BsSelectList<string>(bsSelectList, BsControlType.ListBox, selectControl.Name, true, null).ToString();

                            break;
                        }
                    case FormBuilderControlType.TagList:
                        {
                            var selectControl = control as TagListControlModel;

                            var bsSelectList = new BsSelectList<string>
                            {
                                Items = selectControl.Properties.Items.SelectedValues.Select(x => new BsSelectListItem
                                {
                                    Text = x,
                                    Value = x
                                }).ToList()
                            };                                

                            controlString = _helper.BsTagList<string>(bsSelectList, BsControlType.TagList, selectControl.Name, null).ToString();


                            break;
                        }
                    case FormBuilderControlType.RadioButtonList:
                        {
                            var selectControl = control as RadioButtonListControlModel;

                            var bsSelectList = new BsSelectList<string>
                            {
                                Items = selectControl.Properties.Items.SelectedValues.Select(x => new BsSelectListItem
                                {
                                    Text = x,
                                    Value = x
                                }).ToList()
                            };     

                            controlString = _helper.BsRadioList<string>(bsSelectList, BsControlType.RadioButtonList,selectControl.Name, null, false).ToString();

                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

                var formGroupString = RenderFormGroup(control, controlString);

                controlsString += formGroupString;
            }

            var buttons = new List<BsButtonModel>
            {
                new BsButtonModel("Save", BsComponentStatus.Default),
                new BsButtonModel("Reset", BsComponentStatus.Add)
            };

            var buttonsString = RenderFormButtons(buttons);

            controlsString += buttonsString;

            return controlsString;
        }

        #endregion

        #region Helpers

        internal string GetIdAttributeFromName(string name)
        {
            var regex = new Regex("\\.");

            return regex.Replace(name, "_");
        }

        private List<BsSelectListItem> GetBsSelectListItems<TFormModel>(FormControlPropertyMetadata propertyMetadata, TFormModel formModel)
        {
            var controlValue = propertyMetadata.PropertyInfo.GetValue(formModel);

            var bsSelectlistItems = controlValue.GetType().GetProperty("Items").GetValue(controlValue, null) as List<BsSelectListItem>;

            var selectedValues = controlValue.GetType().GetProperty("SelectedValues").GetValue(controlValue, null) as List<object>;

            var selectedValuesStrings = selectedValues != null
                ? selectedValues.Select(x => x.ToString())
                : new List<string>();

            var items = bsSelectlistItems != null
                ? bsSelectlistItems.Select(x => new BsSelectListItem
                {
                    Text = x.Text,
                    Value = x.Value,
                    Selected = selectedValuesStrings.Contains(x.Value)
                })
                : new List<BsSelectListItem>();

            return items.ToList();
        }

        public TagBuilder GetBsSelectTagBuilder<TFormModel>(FormControlPropertyMetadata propertyMetadata,
            TFormModel formModel)
        {
            var bsSelectListItems = GetBsSelectListItems(propertyMetadata, formModel);

            var selectBuilder = new TagBuilder("select");

            selectBuilder.AddCssClass("form-control bs-hasBformsSelect");

            foreach (var bsSelectListItem in bsSelectListItems)
            {
                var optionBuilder = new TagBuilder("option");

                optionBuilder.MergeAttribute("value", bsSelectListItem.Value);
                optionBuilder.InnerHtml = bsSelectListItem.Text;

                selectBuilder.InnerHtml += optionBuilder.ToString();
            }

            return selectBuilder;
        }

        #endregion
    }

    public class FormControlPropertyMetadata
    {
        public PropertyInfo PropertyInfo { get; set; }
        public BsControlAttribute BsControlAttribute { get; set; }
        public RequiredAttribute RequiredAttribute { get; set; }
        public DisplayAttribute DisplayAttribute { get; set; }
        public FormGroup FormGroupAttribute { get; set; }
    }
}
