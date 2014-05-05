using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BForms.Mvc;
using BForms.Renderers;
using BForms.Models;
using BForms.FormBuilder;

namespace BForms.FormBuilder
{
    public static class FormBuilderExtensions
    {
        public static FormBuilder FormBuilder(this HtmlHelper helper)
        {
            return new FormBuilder(helper.ViewContext);
        }
    }

    public class FormBuilder : BsBaseComponent<FormBuilder>
    {
        #region Properties and constructors

        protected BsTheme EditorTheme;
        protected List<FormBuilderControlViewModel> AvailableControls;
        protected List<FormBuilderControl> SelectedControls;
        protected FormBuilderTabsFactory TabsFactory;
        protected FormBuilderControlsFactory ControlsFactory;
        protected ViewContext ViewContext;
        protected int DefaultTabId;
        protected List<FormBuilderControlActionType> DefaultActions;

        public FormBuilder(ViewContext viewContext)
            : base(viewContext)
        {
            ViewContext = viewContext;

            renderer = new FormEditorBaseRenderer(this);

            EditorTheme = BsTheme.Default;
            AvailableControls = GetDefaultControls();
            TabsFactory = new FormBuilderTabsFactory();
            ControlsFactory = new FormBuilderControlsFactory(AvailableControls, DefaultTabId);
            DefaultActions = new List<FormBuilderControlActionType> { FormBuilderControlActionType.All };
        }

        #endregion

        #region Private methods

        private List<FormBuilderControlViewModel> GetDefaultControls()
        {
            var defaultControls = new List<FormBuilderControlViewModel>();

            defaultControls.Add(new FormBuilderControlViewModel
            {
                Type = FormBuilderControlType.Textbox,
                Glyphicon = Glyphicon.Pencil,
                Text = "Textbox",
                Order = 1
            });

            defaultControls.Add(new FormBuilderControlViewModel
            {
                Type = FormBuilderControlType.Textarea,
                Glyphicon = Glyphicon.Font,
                Text = "Textarea",
                Order = 2
            });

            defaultControls.Add(new FormBuilderControlViewModel
            {
                Type = FormBuilderControlType.NumberPicker,
                Glyphicon = Glyphicon.PlusSign,
                Text = "Number picker",
                Order = 3
            });

            defaultControls.Add(new FormBuilderControlViewModel
            {
                Type = FormBuilderControlType.NumberPickerRange,
                Glyphicon = Glyphicon.PlusSign,
                Text = "Number picker range",
                Order = 4
            });

            defaultControls.Add(new FormBuilderControlViewModel
            {
                Type = FormBuilderControlType.DatePicker,
                Glyphicon = Glyphicon.Calendar,
                Text = "Date picker",
                Order = 5
            });

            defaultControls.Add(new FormBuilderControlViewModel
            {
                Type = FormBuilderControlType.DatePickerRange,
                Glyphicon = Glyphicon.Calendar,
                Text = "Date picker range",
                Order = 6
            });

            defaultControls.Add(new FormBuilderControlViewModel
            {
                Type = FormBuilderControlType.RadioButtonList,
                Glyphicon = Glyphicon.ListAlt,
                Text = "Radio button list",
                Order = 7
            });

            //defaultControls.Add(new FormBuilderControlViewModel
            //{
            //    Type = FormBuilderControlType.Checkbox,
            //    Glyphicon = Glyphicon.Check,
            //    Text = "Checkbox",
            //    Order = 8
            //});

            defaultControls.Add(new FormBuilderControlViewModel
            {
                Type = FormBuilderControlType.CheckboxList,
                Glyphicon = Glyphicon.Check,
                Text = "Checkbox list",
                Order = 9
            });

            defaultControls.Add(new FormBuilderControlViewModel
            {
                Type = FormBuilderControlType.SingleSelect,
                Glyphicon = Glyphicon.List,
                Text = "Select list",
                Order = 10
            });

            defaultControls.Add(new FormBuilderControlViewModel
            {
                Type = FormBuilderControlType.ListBox,
                Glyphicon = Glyphicon.Tag,
                Text = "List box",
                Order = 11
            });

            defaultControls.Add(new FormBuilderControlViewModel
            {
                Type = FormBuilderControlType.TagList,
                Glyphicon = Glyphicon.Tags,
                Text = "Tag list",
                Order = 12
            });

            defaultControls.Add(new FormBuilderControlViewModel
            {
                Type = FormBuilderControlType.Title,
                Glyphicon = Glyphicon.TextWidth,
                Text = "Title",
                Order = 13
            });

            defaultControls.Add(new FormBuilderControlViewModel
            {
                Type = FormBuilderControlType.Pagebreak,
                Glyphicon = Glyphicon.LogIn,
                Text = "Pagebreak",
                Order = 14
            });

            return defaultControls;
        }

        #endregion

        #region Public methods

        public FormEditorRenderingOptions GetRenderingOptions()
        {
            return new FormEditorRenderingOptions
            {
                Theme = EditorTheme
            };
        }

        public List<FormBuilderControlViewModel> GetAvailableControls()
        {
            return AvailableControls;
        }

        public List<FormBuilderControl> GetSelectedControls()
        {
            return SelectedControls;
        }

        public ViewContext GetViewContext()
        {
            return ViewContext;
        }

        public List<FormBuilderTabBuilder> GetTabs()
        {
            return TabsFactory.GetTabs();
        }

        public void ApplyPreRenderingChanges()
        {
            var tabGroups = TabsFactory.GetTabs().ToDictionary(x => x.Id, x => x.Controls);

            AvailableControls = ControlsFactory.GetAllControls();

            if (tabGroups.Any() && tabGroups.Any(x => x.Value.Any()))
            {
                AvailableControls.RemoveAll(x => true);

                foreach (var tabGroup in tabGroups)
                {
                    var controls = tabGroup.Value;

                    controls.ForEach(x =>
                    {
                        x.TabId = tabGroup.Key;
                    });

                    AvailableControls.AddRange(controls);
                }
            }
        }

        #endregion

        #region Fluent methods

        public FormBuilder ConfigureControls(Action<FormBuilderControlsFactory> action)
        {
            TabsFactory.ClearTabs();

            action.Invoke(ControlsFactory);

            return this;
        }

        public FormBuilder ConfigureTabs(Action<FormBuilderTabsFactory> action)
        {
            ControlsFactory.ClearControls();

            action.Invoke(TabsFactory);

            return this;
        }

        public FormBuilder DeafultControlActions(IEnumerable<FormBuilderControlActionType> actions)
        {
            DefaultActions = actions.ToList();

            return this;
        }

        public FormBuilder DeafultControlActions(FormBuilderControlActionType action)
        {
            DefaultActions = new List<FormBuilderControlActionType> { action };

            return this;
        }

        #endregion
    }

    #region Helper classes and enums

    public class FormEditorRenderingOptions
    {
        public BsTheme Theme { get; set; }
    }

    #endregion
}
