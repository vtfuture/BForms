using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BForms.Grid;
using BForms.Models;
using BForms.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BForms.Html
{ 
    public class BsControlPanelTabsFactory
    {
        protected List<ControlPanelTab> Tabs;
        public HtmlHelper Helper;

        public BsControlPanelTabsFactory(HtmlHelper helper)
        {
            Tabs = new List<ControlPanelTab>();
            Helper = helper;
        }

        #region Fluent API

        public BsControlPanelTabsFactory Add(ControlPanelTab tab)
        {
            Tabs.Add(tab);

            return this;
        }

        public ControlPanelTab For(Func<ControlPanelTab, bool> tabSelector)
        {
            var tab = Tabs.FirstOrDefault(tabSelector);

            return tab;
        }

        #endregion

        #region Public methods

        public List<ControlPanelTab> GetTabs()
        {
            return Tabs;
        }

        #endregion
    }
    
    #region Helpers

    public class ControlPanelTab
    {
        public string TabId { get; set; }
        public string DisplayName { get; set; }
        public bool HasQuicksearch { get; set; }
        public string Content { get; set; }
        public bool IsActive { get; set; }
        public BsControlPanelTabContentType ContentType { get; set; }

        public ControlPanelTab SetTitle(string title)
        {
            DisplayName = title;

            return this;
        }

        public ControlPanelTab ShowQuicksearch(bool showQuicksearch = true)
        {
            HasQuicksearch = showQuicksearch;

            return this;
        }

        public ControlPanelTab SetTabId(string tabId)
        {
            TabId = tabId;

            return this;
        }

        public ControlPanelTab SetContent(string content)
        {
            Content = content;

            return this;
        }

        public ControlPanelTab Selected(bool selected = true)
        {
            IsActive = selected;

            return this;
        }

        public ControlPanelTab SetContentType(BsControlPanelTabContentType type)
        {
            ContentType = type;

            return this;
        }
    }

    #endregion

    #region Enums

    public enum BsControlPanelTabContentType
    {
        [Description("default")]
        Default = 1,
        [Description("grid")]
        Grid = 2,
        [Description("form")]
        Form = 3
    }

    #endregion
}
