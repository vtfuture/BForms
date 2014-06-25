using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BForms.Html;
using BForms.FormBuilder;
using BForms.Models;
using BForms.Panels;
using BForms.FormBuilder;
using BForms.Utilities;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.ExcelAc;

namespace BForms.Renderers
{
    public class FormEditorBaseRenderer : BsBaseRenderer<BForms.FormBuilder.FormBuilder>
    {
        public FormEditorBaseRenderer(BForms.FormBuilder.FormBuilder builder, HtmlHelper helper)
            : base(builder)
        {
            FormHtmlRenderer = new BsFormHtmlRenderer(helper);
        }

        protected BsFormHtmlRenderer FormHtmlRenderer;

        public override string Render()
        {
            var containerBuilder = new TagBuilder("div");

            var renderingOptions = this.Builder.GetRenderingOptions();

            this.Builder.ApplyPreRenderingChanges();

            containerBuilder.AddCssClass("form_builder row");

            var controlsTab = RenderControlsTab(renderingOptions);
            var formTab = RenderFormTab(renderingOptions);
            var propertiesTab = RenderPropertiesTab(renderingOptions);

            containerBuilder.InnerHtml = controlsTab + formTab + propertiesTab;
            containerBuilder.MergeAttributes(this.Builder.htmlAttributes);

            var availableControls = this.Builder.GetAvailableControls().Select(x => new
            {
                Type = x.Type,
                Text = x.Text,
                Order = x.Order,
                Glyphicon = x.Glyphicon.GetDescription(),
                TabId = x.TabId,
                ControlName = x.ControlName,
                Actions = x.Actions != null ?  x.Actions.Select(y => y.GetDisplayAttribute().Name).ToList() : new List<string>(),
                CustomActions = x.CustomActions.Select(y => new
                {
                    Name = y.Name,
                    Glyphicon = y.Glyphicon.GetDescription(),
                    Title = y.Title
                })
            });

            var controlsData = new Dictionary<string, object> { { "controls", availableControls } };
            var defaultActionsData = new Dictionary<string, object> { { "actions", this.Builder.GetDefaultActions().Select(x=>x.GetDisplayAttribute().Name) } };
            var customActionsData = new Dictionary<string, object> { { "actions", this.Builder.GetCustomActions().Select(x => new
            {
                Name = x.Name,
                Title = x.Title,
                Glyphicon = x.Glyphicon.GetDescription()
            }) } };

            containerBuilder.MergeAttribute("data-controls", controlsData.ToJsonString());
            containerBuilder.MergeAttribute("data-defaultactions", defaultActionsData.ToJsonString());
            containerBuilder.MergeAttribute("data-customactions", customActionsData.ToJsonString());

            return containerBuilder.ToString();
        }

        private string RenderControlsTab(FormEditorRenderingOptions renderingOptions)
        {
            var controlsContainerBuilder = new TagBuilder("div");
            var toolbarBuilder = GetToolbarBuilder("Controls", renderingOptions);

            controlsContainerBuilder.AddCssClass("form_builder-tab form_builder-controls col-lg-3 col-md-3 col-sm-3");
            controlsContainerBuilder.InnerHtml = toolbarBuilder.ToString();

            var controls = this.Builder.GetAvailableControls();
            var groupedControls = controls.GroupBy(x => x.TabId, (key, group) => new
            {
                TabId = key,
                Controls = group.ToList()
            });

            if (controls != null && controls.Any())
            {
                controls = controls.OrderBy(x => x.Order).ToList();

                var controlOptions = new List<object>();

                foreach (var control in controls)
                {
                    controlOptions.Add(control);
                }

                this.Builder.options.Add("controls", controlOptions);
            }

            var controlsWrapper = new TagBuilder("div");

            controlsWrapper.AddCssClass("form_builder-controlsWrapper");

            var tabs = this.Builder.GetTabs();

            if (tabs.Any(x => !x.IsDefaultTab))
            {
                tabs.Remove(tabs.FirstOrDefault(x => x.IsDefaultTab));
            }

            if (tabs.Any() && tabs.Any(x => !x.IsDefaultTab))
            {
                if (!tabs.Any(x => x.IsOpen && !x.IsDefaultTab))
                {
                    tabs.FirstOrDefault(x => !x.IsDefaultTab).IsOpen = true;
                }

                var controlTabsBuilder = GetControlTabsBuilder(tabs);

                controlsWrapper.InnerHtml = controlTabsBuilder.ToString();
            }

            if (tabs.Count(x => x.IsOpen) > 1)
            {
                var firstOpenTab = tabs.FirstOrDefault(x => x.IsOpen);

                tabs.ForEach(x =>
                {
                    x.IsOpen = false;
                });

                firstOpenTab.IsOpen = true;
            }

            foreach (var tab in tabs)
            {
                var controlsListBuilder = new TagBuilder("ul");

                controlsListBuilder.AddCssClass("form_builder-controlsList list-group");
                controlsListBuilder.Attributes.Add("data-tabid", tab.Id.ToString());

                if (!tab.IsOpen)
                {
                    controlsListBuilder.Attributes.Add("style", "display:none;");
                }

                controlsWrapper.InnerHtml += controlsListBuilder.ToString();
            }

            controlsContainerBuilder.InnerHtml += controlsWrapper.ToString();

            return controlsContainerBuilder.ToString();
        }

        private string RenderPropertiesTab(FormEditorRenderingOptions renderingOptions)
        {
            var propertiesContainerBuilder = new TagBuilder("div");
            var toolbarBuilder = GetToolbarBuilder("Properties", renderingOptions);
            var settingsContainerBuilder = new TagBuilder("div");

            settingsContainerBuilder.AddCssClass("form_builder-settingsContainer row");

            var controls = this.Builder.GetControlsMetadata();

            foreach (var control in controls)
            {
                var metadataBuilder = new TagBuilder("div");
                
                metadataBuilder.AddCssClass("col-lg-12 col-md-12 col-sm-12");

                var propertiesTabs = from prop in control.GetType().GetProperties()
                    let propertiesAttributes = prop.GetCustomAttributes(typeof (FormBuilderPropertiesTab), true)
                    let displayAttributes = prop.GetCustomAttributes(typeof(DisplayAttribute), true)
                    where propertiesAttributes.Length == 1
                    select new
                    {
                        PropertiesAttribute = propertiesAttributes.FirstOrDefault() as FormBuilderPropertiesTab,
                        DisplayAttribute = displayAttributes.Select(x => x as DisplayAttribute).FirstOrDefault(),
                        PropertyInfo = prop
                    };

                propertiesTabs = propertiesTabs.OrderBy(x => x.PropertiesAttribute.Order);

                foreach (var propertiesTab in propertiesTabs)
                {
                    var title = propertiesTab.DisplayAttribute.GetName();

                    var titleBuilder = new TagBuilder("h3");
                    var titleGlyphiconBuilder = new TagBuilder("span");
                    var arrowGlyphiconBuilder = new TagBuilder("span");

                    titleGlyphiconBuilder.AddCssClass("glyphicon " + propertiesTab.PropertiesAttribute.Glyphicon.GetDescription());
                    arrowGlyphiconBuilder.AddCssClass("pull-right glyphicon " + Glyphicon.ChevronUp.GetDescription());

                    titleBuilder.InnerHtml = titleGlyphiconBuilder.ToString() + " " + title +
                                             arrowGlyphiconBuilder.ToString();

                    var formModel = propertiesTab.PropertyInfo.GetValue(control);

                    if (formModel != null)
                    {
                        var propertiesFormString = FormHtmlRenderer.RenderForm(formModel, renderingOptions.Theme, true, propertiesTab.PropertyInfo.Name);

                        metadataBuilder.InnerHtml += titleBuilder.ToString() + propertiesFormString;
                    }
                }

                metadataBuilder.MergeAttribute("data-properties-for", ((int) control.Type).ToString());
                metadataBuilder.MergeAttribute("style", "display:none;");

                settingsContainerBuilder.InnerHtml += metadataBuilder.ToString();
            }

            var panelBuilder = new BsPanelHtmlBuilder(this.Builder.GetViewContext());
            var panelRenderer = new BsPanelLightRenderer(panelBuilder);

            panelBuilder.InitialEditable().Id("").Name("Settings").Glyphicon(Glyphicon.Cog);

            propertiesContainerBuilder.AddCssClass("form_builder-tab form_builder-properties col-lg-3 col-md-3 col-sm-3");
            propertiesContainerBuilder.InnerHtml = toolbarBuilder.ToString() + settingsContainerBuilder.ToString();

            return propertiesContainerBuilder.ToString();
        }

        private string RenderFormTab(FormEditorRenderingOptions renderingOptions)
        {
            var formContainerBuilder = new TagBuilder("div");
            var toolbarBuilder = GetToolbarBuilder("Form", renderingOptions);
            var formWrapperBuilder = new TagBuilder("div");
            var formBuilder = new TagBuilder("form");
            var formInnerContainerBuilder = new TagBuilder("div");

            formWrapperBuilder.AddCssClass("form_builder-formContainer");
            formInnerContainerBuilder.AddCssClass("form_container row " + Utilities.ReflectionHelpers.GetDescription(renderingOptions.Theme));
            formBuilder.AddCssClass("form_builder-form");

            formBuilder.InnerHtml = formInnerContainerBuilder.ToString();
            formWrapperBuilder.InnerHtml = formBuilder.ToString();

            formContainerBuilder.AddCssClass("form_builder-tab form_builder-form col-lg-6 col-md-6 col-sm-6");
            formContainerBuilder.InnerHtml = toolbarBuilder.ToString() + formWrapperBuilder.ToString();

            return formContainerBuilder.ToString();
        }

        private TagBuilder GetToolbarBuilder(string title, FormEditorRenderingOptions renderingOptions)
        {
            var toolbarBuilder = new TagBuilder("div");
            var toolbarHeaderBuilder = new TagBuilder("div");
            var toolbarTitleBuilder = new TagBuilder("h1");

            toolbarBuilder.AddCssClass("grid_toolbar row");
            toolbarBuilder.AddCssClass(Utilities.ReflectionHelpers.GetDescription(renderingOptions.Theme));
            toolbarHeaderBuilder.AddCssClass("grid_toolbar_header");

            toolbarTitleBuilder.InnerHtml = title;
            toolbarHeaderBuilder.InnerHtml = toolbarTitleBuilder.ToString();
            toolbarBuilder.InnerHtml = toolbarHeaderBuilder.ToString();

            return toolbarBuilder;
        }

        private TagBuilder GetControlTabsBuilder(IEnumerable<FormBuilderTabBuilder> tabs)
        {
            var btnGroupBuilder = new TagBuilder("div");

            btnGroupBuilder.AddCssClass("btn-group btn-group-justifird form_builder-tabControl");

            foreach (var tab in tabs)
            {
                if (!tab.IsDefaultTab)
                {
                    var tabBuilder = new TagBuilder("a");
                    var glyphiconHtml = String.Empty;

                    if (tab.Glyphicon != null)
                    {
                        var tabGlyphiconBuilder = new TagBuilder("span");
                        var glyphicon = (Glyphicon) tab.Glyphicon;

                        tabGlyphiconBuilder.AddCssClass("glyphicon " + glyphicon.GetDescription());

                        glyphiconHtml = tabGlyphiconBuilder.ToString();
                    }

                    tabBuilder.AddCssClass("btn btn-default form_builder-tabBtn");
                    tabBuilder.Attributes.Add("role", "button");
                    tabBuilder.Attributes.Add("data-tabid", tab.Id.ToString());
                    tabBuilder.MergeAttributes(tab.Attributes);
                    tabBuilder.InnerHtml = glyphiconHtml + (tab.Text ?? String.Empty);

                    if (tab.IsOpen)
                    {
                        tabBuilder.AddCssClass("selected");
                    }

                    btnGroupBuilder.InnerHtml += tabBuilder.ToString();
                }
            }

            return btnGroupBuilder;
        }

        private TagBuilder GetControlItemBuilder(FormBuilderControlViewModel model)
        {
            var controlItemContainerBuilder = new TagBuilder("li");
            var controlItemBuilder = new TagBuilder("div");
            var glyphiconBuilder = new TagBuilder("span");
            var glyphiconWrapperBuilder = new TagBuilder("a");
            var labelBuilder = new TagBuilder("h1");
            var labelWrapperBuilder = new TagBuilder("a");
            var addButtonBuilder = new TagBuilder("a");
            var addButtonGlyphiconBuilder = new TagBuilder("span");

            controlItemContainerBuilder.AddCssClass("border form_builder-controlItem");
            controlItemContainerBuilder.Attributes.Add("data-controlType", ((int)model.Type).ToString());
            controlItemBuilder.AddCssClass("col-lg-12 col-md-12 col-sm-12");

            glyphiconBuilder.AddCssClass("glyphicon");
            glyphiconBuilder.AddCssClass(Utilities.ReflectionHelpers.GetDescription(model.Glyphicon));

            glyphiconWrapperBuilder.AddCssClass("btn btn-white pull-left disabled form_builder-controlItem-label");
            glyphiconWrapperBuilder.MergeAttribute("href", "#");
            glyphiconWrapperBuilder.InnerHtml = glyphiconBuilder.ToString();

            labelBuilder.InnerHtml = model.Text;
            //labelBuilder.AddCssClass("hidden-sm hidden-xs");

            labelWrapperBuilder.MergeAttribute("href", "#");
            // labelWrapperBuilder.AddCssClass("hidden-sm hidden-xs");
            labelWrapperBuilder.InnerHtml = labelBuilder.ToString();

            addButtonGlyphiconBuilder.AddCssClass("glyphicon glyphicon-plus");

            addButtonBuilder.AddCssClass("btn btn-white pull-right form_builder-controlItem-add");
            addButtonBuilder.Attributes.Add("href", "#");
            addButtonBuilder.InnerHtml = addButtonGlyphiconBuilder.ToString();

            controlItemBuilder.InnerHtml = glyphiconWrapperBuilder.ToString() + labelWrapperBuilder.ToString() + addButtonBuilder.ToString();
            controlItemContainerBuilder.InnerHtml = controlItemBuilder.ToString();

            controlItemContainerBuilder.Attributes.Add("data-position", model.Order.ToString());
            controlItemContainerBuilder.Attributes.Add("data-name", model.Text);
            controlItemContainerBuilder.Attributes.Add("data-glyphicon", Utilities.ReflectionHelpers.GetDescription(model.Glyphicon));

            return controlItemContainerBuilder;
        }
    }
}
