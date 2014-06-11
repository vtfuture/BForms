using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BForms.Html
{
    public class BsControlPanelTabsFactory
    {
        protected List<ControlPanelTab> Tabs;

        public BsControlPanelTabsFactory()
        {
            Tabs = new List<ControlPanelTab>();
        }

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

        public List<ControlPanelTab> GetTabs()
        {
            return Tabs;
        }
    }
    
    #region Helpers

    public class ControlPanelTab
    {
        public string TabId { get; set; }
        public string DisplayName { get; set; }
        public bool HasQuicksearch { get; set; }
        public string Content { get; set; }
        public bool IsActive { get; set; }

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
    }

    #endregion
}
