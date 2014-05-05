using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using BForms.Models;

namespace BForms.FormBuilder
{
    public class FormBuilderTabBuilder
    {
        #region Members

        public int Id { get; set; }
        public string Text { get; set; }
        public Glyphicon? Glyphicon { get; set; }
        public bool IsDefaultTab { get; set; }
        public bool IsOpen { get; set; }
        public Dictionary<string, object> Attributes { get; set; } 

        public List<FormBuilderControlViewModel> Controls { get; set; }

        private FormBuilderControlFactory _controlFactory = new FormBuilderControlFactory();

        #endregion

        #region Fluent Methods

        public FormBuilderTabBuilder SetText(string text)
        {
            Text = text;

            return this;
        }

        public FormBuilderTabBuilder SetGlyphicon(Glyphicon glyphicon)
        {
            Glyphicon = glyphicon;

            return this;
        }

        public FormBuilderTabBuilder SetId(int id)
        {
            Id = id;

            return this;
        }

        public FormBuilderTabBuilder Open(bool open = true)
        {
            IsOpen = open;

            return this;
        }

        public FormBuilderTabBuilder SetControls(List<FormBuilderControlViewModel> controls)
        {
            Controls = controls;

            return this;
        }

        public FormBuilderTabBuilder AddControl(FormBuilderControlViewModel control)
        {
            Controls.Add(control);
            
            return this;
        }

        public FormBuilderTabBuilder AddControls(IEnumerable<FormBuilderControlViewModel> controls)
        {
            Controls.AddRange(controls);

            return this;
        }

        public FormBuilderTabBuilder AddControls(params FormBuilderControlViewModel[] controls)
        {
            return AddControls(controls.ToList());
        }

        public FormBuilderTabBuilder AddControl(FormBuilderControlType controlType)
        {
            var control = _controlFactory.Create(controlType, Id);

            return AddControl(control);
        }

        public FormBuilderTabBuilder AddControls(IEnumerable<FormBuilderControlType> controlTypes)
        {
            var controls = controlTypes.Select(x => _controlFactory.Create(x, Id));

            return AddControls(controls);
        }

        public FormBuilderTabBuilder AddControls(params FormBuilderControlType[] controlTypes)
        {
            var types = controlTypes.ToList();

            return AddControls(types);
        }

        public FormBuilderTabBuilder HtmlAttributes(Dictionary<string, object> attributes)
        {
            Attributes = attributes;

            return this;
        }

        public FormBuilderTabBuilder ClearControls()
        {
            Controls.Clear();

            return this;
        }

        #endregion
    }

    public class FormBuilderTabsFactory
    {
        private List<FormBuilderTabBuilder> _tabs;

        public FormBuilderTabsFactory()
        {
            _tabs = new List<FormBuilderTabBuilder>();

            _tabs.Add(new FormBuilderTabBuilder
            {
                Id = 0,
                IsDefaultTab = true,
                IsOpen = true,
                Controls = new List<FormBuilderControlViewModel>()
            });
        }

        #region Public methods

        public FormBuilderTabBuilder Add(FormBuilderTabBuilder tab)
        {
            _tabs.Add(tab);

            return _tabs.LastOrDefault();
        }

        public FormBuilderTabBuilder Add(string text)
        {
            var id = GenerateId();

            return Add(new FormBuilderTabBuilder
            {
                Id = id,
                Text = text,
                Controls = new List<FormBuilderControlViewModel>()
            });
        }

        public FormBuilderTabBuilder Add(int id, string text)
        {
            return Add(id, text, null);
        }

        public FormBuilderTabBuilder Add(int id, string text, Glyphicon? glyphicon = null)
        {
            if (_tabs.Select(x => x.Id).Contains(id))
            {
                throw new Exception("A tab with id " + id + " has already been added");
            }

            return Add(new FormBuilderTabBuilder
            {
                Id = id,
                Text = text,
                Glyphicon = glyphicon,
                Controls = new List<FormBuilderControlViewModel>()
            });
        }

        public FormBuilderTabBuilder Add(int id, Glyphicon glyphicon)
        {
            return Add(id, null, glyphicon);
        }

        public FormBuilderTabBuilder Add(Glyphicon glyphicon)
        {
            var id = GenerateId();

            return Add(id, glyphicon);
        }

        public void Remove(int id)
        {
            var tab = _tabs.FirstOrDefault(x => x.Id == id);

            if (tab != null)
            {
                _tabs.Remove(tab);
            }
        }

        public FormBuilderTabBuilder For(Func<FormBuilderTabBuilder, bool> predicate)
        {
            var tab = _tabs.FirstOrDefault(predicate);

            return tab;
        }

        public List<FormBuilderTabBuilder> GetTabs()
        {
            return _tabs;
        }

        public FormBuilderTabsFactory ClearTabs()
        {
            foreach (var tab in _tabs)
            {
                tab.ClearControls();
            }

            _tabs.RemoveAll(x => !x.IsDefaultTab);

            return this;
        }

        #endregion

        #region Private methods

        private int GenerateId()
        {
            if (!_tabs.Any())
            {
                return 1;
            }

            var lastId = _tabs.Select(x => x.Id).LastOrDefault();

            return lastId + 1;
        }

        #endregion
    }
}
