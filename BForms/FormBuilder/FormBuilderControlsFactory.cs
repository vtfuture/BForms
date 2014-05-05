using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BForms.FormBuilder
{
    public class FormBuilderControlsFactory
    {
        private List<FormBuilderControlViewModel> _controls;
        private FormBuilderControlFactory _controlFactory;
        private int _tabId;

        public FormBuilderControlsFactory(IEnumerable<FormBuilderControlViewModel> controls = null, int wrapperTabId = 0)
        {
            _tabId = wrapperTabId;
            _controlFactory = new FormBuilderControlFactory();

            if (controls != null)
            {
                _controls = controls.ToList();
            }
            else
            {
                _controls = new List<FormBuilderControlViewModel>();
            }
        }

        #region Fluent methdos

        public FormBuilderControlsFactory AddControl(FormBuilderControlViewModel control)
        {
            _controls.Add(control);

            return this;
        }

        public FormBuilderControlsFactory AddControl(FormBuilderControlType controlType)
        {
            var control = _controlFactory.Create(controlType, _tabId);

            return AddControl(control);
        }

        public FormBuilderControlsFactory AddControls(IEnumerable<FormBuilderControlViewModel> controls)
        {
            _controls.AddRange(controls);

            return this;
        }

        public FormBuilderControlsFactory AddControls(IEnumerable<FormBuilderControlType> controlTypes)
        {
            var controls = controlTypes.Select(x => _controlFactory.Create(x, _tabId));

            return AddControls(controls);
        }

        public FormBuilderControlsFactory AddControls(params FormBuilderControlViewModel[] controls)
        {
            return AddControls(controls.ToList());
        }

        public FormBuilderControlsFactory AddControls(params FormBuilderControlType[] controlTypes)
        {
            var ctrls = controlTypes.ToList();

            return AddControls(controlTypes.ToList());
        }

        public FormBuilderControlsFactory Remove(FormBuilderControlViewModel control)
        {
            _controls.Remove(control);

            return this;
        }

        public FormBuilderControlsFactory Remove(IEnumerable<FormBuilderControlViewModel> controls)
        {
            foreach (var control in controls)
            {
                _controls.Remove(control);
            }

            return this;
        }

        public FormBuilderControlsFactory Remove(Predicate<FormBuilderControlViewModel> predicate)
        {
            _controls.RemoveAll(predicate);

            return this;
        }

        public FormBuilderControlsFactory ClearControls()
        {
            _controls.Clear();

            return this;
        }

        public FormBuilderControlsFactory For(Func<FormBuilderControlViewModel, bool> selector, Action<FormBuilderControlViewModel> action)
        {
            var controls = _controls.Where(selector).ToList();

            controls.ForEach(action);
            
            return this;
        }

        public FormBuilderControlViewModel Get(Func<FormBuilderControlViewModel, bool> selector)
        {
            var control = _controls.FirstOrDefault(selector);

            return control;
        }

        #endregion

        #region Helper methods

        public List<FormBuilderControlViewModel> GetAllControls()
        {
            return _controls;
        }

        #endregion
    }
}
