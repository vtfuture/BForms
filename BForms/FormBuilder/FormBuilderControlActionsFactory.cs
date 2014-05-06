using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BForms.Models;

namespace BForms.FormBuilder
{
    public class FormBuilderControlActionsFactory
    {
        private List<FormBuilderControlActionType> _defaultActions;
        private List<FormBuilderCustomAction> _defaultCustomActions;
        private List<FormBuilderCustomAction> _customActions;
        private List<KeyValuePair<Func<FormBuilderControlViewModel, bool>, List<FormBuilderControlActionType>>> _actionsToApply;
        private List<KeyValuePair<Func<FormBuilderControlViewModel, bool>, List<FormBuilderCustomAction>>> _customActionsToApply;

        public FormBuilderControlActionsFactory()
        {
            _defaultActions = new List<FormBuilderControlActionType> { FormBuilderControlActionType.All };
            _defaultCustomActions = new List<FormBuilderCustomAction>();
            _actionsToApply = new List<KeyValuePair<Func<FormBuilderControlViewModel, bool>, List<FormBuilderControlActionType>>>();
            _customActions = new List<FormBuilderCustomAction>();
            _customActionsToApply = new List<KeyValuePair<Func<FormBuilderControlViewModel, bool>, List<FormBuilderCustomAction>>>();
        }

        #region Fluent methods

        public FormBuilderControlActionsFactory DefaultActions(IEnumerable<FormBuilderControlActionType> defaultActions)
        {
            _defaultActions = defaultActions.ToList();

            return this;
        }

        public FormBuilderControlActionsFactory DefaultActions(params FormBuilderControlActionType[] defaultActions)
        {
            _defaultActions = defaultActions.ToList();

            return this;
        }

        public FormBuilderControlActionsFactory DefaultAction(FormBuilderControlActionType defaultAction)
        {
            _defaultActions = new List<FormBuilderControlActionType> { defaultAction };

            return this;
        }

        public FormBuilderControlActionsFactory ActionsFor(Func<FormBuilderControlViewModel, bool> controlSelector, IEnumerable<FormBuilderControlActionType> actions)
        {
            _actionsToApply.Add(new KeyValuePair<Func<FormBuilderControlViewModel, bool>, List<FormBuilderControlActionType>>(controlSelector, actions.ToList()));

            return this;
        }

        public FormBuilderControlActionsFactory ClearActionsFor(Func<FormBuilderControlViewModel, bool> controlSelector)
        {
            return ActionsFor(controlSelector, new List<FormBuilderControlActionType>());
        }

        public FormBuilderControlActionsFactory ClearDefaultActions()
        {
            _defaultActions.Clear();

            return this;
        }

        #region Custom actions

        public FormBuilderControlActionsFactory RegisterCustomAction(FormBuilderCustomAction customAction)
        {
            _customActions.Add(customAction);

            return this;
        }

        public FormBuilderControlActionsFactory DefaultCustomActions(IEnumerable<FormBuilderCustomAction> customActions)
        {
            _customActions = customActions.ToList();

            return this;
        }

        public FormBuilderControlActionsFactory DefaultCustomActions(IEnumerable<string> customActionNames)
        {
            var customActions = GetCustomActions(customActionNames);

            return DefaultCustomActions(customActions);
        }

        public FormBuilderControlActionsFactory DefaultCustomActions(params FormBuilderCustomAction[] customActions)
        {
            _customActions = customActions.ToList();

            return this;
        }

        public FormBuilderControlActionsFactory DefaultCustomActions(params string[] customActionNames)
        {
            return DefaultCustomActions(customActionNames.ToList());
        }

        public FormBuilderControlActionsFactory DefaultCustomAction(FormBuilderCustomAction customAction)
        {
            _customActions = new List<FormBuilderCustomAction> { customAction };

            return this;
        }

        public FormBuilderControlActionsFactory DefaultCustomAction(string customActionName)
        {
            var customAction = GetCustomAction(customActionName);

            return DefaultCustomAction(customAction);
        }

        public FormBuilderControlActionsFactory CustomActionsFor(Func<FormBuilderControlViewModel, bool> controlSelector, IEnumerable<FormBuilderCustomAction> customActions)
        {
            _customActionsToApply.Add(new KeyValuePair<Func<FormBuilderControlViewModel, bool>, List<FormBuilderCustomAction>>(controlSelector, customActions.ToList()));

            return this;
        }
        public FormBuilderControlActionsFactory CustomActionsFor(Func<FormBuilderControlViewModel, bool> controlSelector, IEnumerable<string> customActionNames)
        {
            var customActions = GetCustomActions(customActionNames);

            _customActionsToApply.Add(new KeyValuePair<Func<FormBuilderControlViewModel, bool>, List<FormBuilderCustomAction>>(controlSelector, customActions.ToList()));

            return this;
        }


        public FormBuilderControlActionsFactory ClearCustomActionsFor(Func<FormBuilderControlViewModel, bool> controlSelector)
        {
            return CustomActionsFor(controlSelector, new List<FormBuilderCustomAction>());
        }

        public FormBuilderControlActionsFactory ClearDefaultCustomActions()
        {
            _defaultCustomActions.Clear();

            return this;
        }

        #endregion

        #endregion

        #region Public methods

        public void SetActions(ref List<FormBuilderControlViewModel> controls)
        {
            controls.ForEach(x =>
            {
                x.Actions = _defaultActions;
                x.CustomActions = _defaultCustomActions;
            });

            if (_actionsToApply.Any())
            {
                foreach (var pair in _actionsToApply)
                {
                    var selector = pair.Key;
                    var actions = pair.Value;

                    var controlsToAffect = controls.Where(selector).ToList();

                    controlsToAffect.ForEach(x =>
                    {
                        x.Actions = actions;
                    });
                }
            }

            if (_customActionsToApply.Any())
            {
                foreach (var pair in _customActionsToApply)
                {
                    var selector = pair.Key;
                    var customActions = pair.Value;

                    var controlsToAffect = controls.Where(selector).ToList();

                    controlsToAffect.ForEach(x =>
                    {
                        x.CustomActions = customActions;
                    });
                }
            }
        }

        public List<FormBuilderControlActionType> GetDefaultActions()
        {
            return _defaultActions;
        }

        public List<FormBuilderCustomAction> GetRegisteredCustomActions()
        {
            var customActions = _customActions;

            var otherCustomActions = _customActionsToApply.SelectMany(x => x.Value);

            foreach (var candidateCustomAction in otherCustomActions)
            {
                if (!customActions.Any(x => x.Name == candidateCustomAction.Name))
                {
                    customActions.Add(candidateCustomAction);
                }
            }

            return customActions;
        }

        #endregion

        #region Private methods

        private bool CustomActionRegistered(string customActionName)
        {
            return _customActions.Any(x => x.Name == customActionName);
        }

        private FormBuilderCustomAction GetCustomAction(string customActionName)
        {
            var customAction = _customActions.FirstOrDefault(x => x.Name == customActionName);

            if (customAction == null)
            {
                throw new UnregisteredCustomActionException("No registered custom action named " + customActionName + " was found");
            }

            return customAction;
        }

        private List<FormBuilderCustomAction> GetCustomActions(IEnumerable<string> customActionNames)
        {
            var customActions = customActionNames.Select(GetCustomAction).ToList();

            return customActions;
        }

        #endregion
    }

    public class FormBuilderCustomAction
    {
        public string Name { get; set; }
        public Glyphicon Glyphicon { get; set; }
        public string Title { get; set; }
    }

    public class UnregisteredCustomActionException : Exception
    {
        public UnregisteredCustomActionException(string message)
            : base(message)
        {
            
        }
    }
}
