using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BForms.Models;

namespace BForms.Html
{
    public class BsControlPanelActionsFactory
    {
        private List<ControlPanelAction> _actions;

        public BsControlPanelActionsFactory()
        {
            _actions = new List<ControlPanelAction>();

            Add(ControlPanelActionType.Toggle);
            Add(ControlPanelActionType.Remove);
        }

        #region Fluent API

        public BsControlPanelActionsFactory Add(ControlPanelAction action)
        {
            _actions.Add(action);

            return this;
        }

        public BsControlPanelActionsFactory Add(ControlPanelActionType actionType)
        {
            return Add(actionType.GetAction());
        }

        public BsControlPanelActionsFactory Remove(ControlPanelActionType actionType)
        {
            return Remove(x => x.ActionName == actionType.GetAction().ActionName);
        }

        public BsControlPanelActionsFactory Remove(Predicate<ControlPanelAction> selector)
        {
            _actions.RemoveAll(selector);

            return this;
        }

        #endregion

        #region Public methods

        public List<ControlPanelAction> GetActions()
        {
            return _actions;
        }

        #endregion
    }

    #region Enums

    public enum ControlPanelActionType
    {
        [ControlPanelActionSpecification(Glyphicon = Glyphicon.ChevronUp, ActionName = "toggle")]
        Toggle = 1,
        [ControlPanelActionSpecification(Glyphicon = Glyphicon.Remove, ActionName = "remove")]
        Remove = 2
    }
    
    #endregion 

    #region Helpers

    public class ControlPanelAction
    {
        public Glyphicon Glyphicon { get; set; }
        public string ActionName { get; set; }
    }

    public class ControlPanelActionSpecification : Attribute
    {
        public Glyphicon Glyphicon { get; set; }
        public string ActionName { get; set; }
    }

    #endregion

    #region Extensions

    public static class ControlPanelActionsExtensions
    {
        public static ControlPanelAction GetAction(this ControlPanelActionType type)
        {
            var attribute =
                type.GetType().GetField(type.ToString()).GetCustomAttribute<ControlPanelActionSpecification>();

            if (attribute != null)
            {
                return new ControlPanelAction
                {
                    Glyphicon = attribute.Glyphicon,
                    ActionName = attribute.ActionName
                };
            }

            return null;
        }
    }

    #endregion
}
