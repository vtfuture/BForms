using System;
using System.Collections.Generic;
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

namespace BForms.Renderers
{
    public class FormEditorBaseRenderer : BsBaseRenderer<BForms.FormBuilder.FormBuilder>
    {
        public FormEditorBaseRenderer(BForms.FormBuilder.FormBuilder builder)
            : base(builder)
        {

        }

        public override string Render()
        {
            var containerBuilder = new TagBuilder("div");

            var renderingOptions = this.Builder.GetRenderingOptions();

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
                Glyphicon = x.Glyphicon.GetDescription()
            });

            var controlsData = new Dictionary<string, object> {{"controls", availableControls}};

            containerBuilder.MergeAttribute("data-controls", controlsData.ToJsonString());

            return containerBuilder.ToString();
        }

        private string RenderControlsTab(FormEditorRenderingOptions renderingOptions)
        {
            var controlsContainerBuilder = new TagBuilder("div");
            var toolbarBuilder = GetToolbarBuilder("Controls", renderingOptions);

            controlsContainerBuilder.AddCssClass("form_builder-tab form_builder-controls col-lg-3 col-md-3 col-sm-3 border");
            controlsContainerBuilder.InnerHtml = toolbarBuilder.ToString();

            var controlsListBuilder = new TagBuilder("ul");

            controlsListBuilder.AddCssClass("form_builder-controlsList list-group");

            var controls = this.Builder.GetAvailableControls();

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

            controlsContainerBuilder.InnerHtml += controlsListBuilder.ToString();

            return controlsContainerBuilder.ToString();
        }

        private string RenderPropertiesTab(FormEditorRenderingOptions renderingOptions)
        {
            var propertiesContainerBuilder = new TagBuilder("div");
            var toolbarBuilder = GetToolbarBuilder("Properties", renderingOptions);
            var settingsContainerBuilder = new TagBuilder("div");

            settingsContainerBuilder.AddCssClass("form_builder-settingsContainer row");

            var panelBuilder = new BsPanelHtmlBuilder(this.Builder.GetViewContext());
            var panelRenderer = new BsPanelLightRenderer(panelBuilder);

            panelBuilder.InitialEditable().Id("").Name("Settings").Glyphicon(Glyphicon.Cog);

            propertiesContainerBuilder.AddCssClass("form_builder-tab form_builder-properties col-lg-3 col-md-3 col-sm-3 border");
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

            formContainerBuilder.AddCssClass("form_builder-tab form_builder-form col-lg-6 col-md-6 col-sm-6 border");
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
