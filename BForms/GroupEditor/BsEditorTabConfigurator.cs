using BForms.Models;
using BForms.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BForms.GroupEditor
{
    public class BsEditorTabConfigurator : BaseComponent
    {
        #region TabProperties
        private class TabProperties
        {
            internal bool Selected {get; set;}
            internal string Name {get; set;}
            internal string Id { get; set; }
        }
        #endregion

        #region Properties and Constructor
        private List<TabProperties> TabsProperties { get; set; }
        internal Dictionary<string, BaseComponent> Tabs { get; set; }

        public BsEditorTabConfigurator()
        {
            Tabs = new Dictionary<string, BaseComponent>();
            TabsProperties = new List<TabProperties>();
        }
        #endregion

        #region Config
        public BsEditorTabConfigurator AddTab(string name, string id)
        {
            var tabProp = new TabProperties()
            {
                Name = name,
                Id = id
            };

            this.TabsProperties.Add(tabProp);

            return this;
        }

        public BsEditorTabBuilder<TRow> Add<TRow>(BsGroupEditorAttribute attr, BsGridModel<TRow> model) where TRow : new()
        {
            var uid = attr.Id.ToString();
            var tab = new BsEditorTabBuilder<TRow>(attr.Name, uid);

            SetSelectedTab(attr.Name);

            this.Tabs.Add(uid, tab);

            return tab;
        }
        #endregion

        #region Render
        public override string Render()
        {
            return RenderNavigation();
        }

        private string RenderNavigation()
        {
            var nav = new TagBuilder("nav");

            nav.AddCssClass("navbar navbar-default");
            nav.MergeAttribute("role", "navigation");

            var navHeader = new TagBuilder("div");

            navHeader.AddCssClass("navbar-header");

            var navToggle = new TagBuilder("button");
            navToggle.AddCssClass("navbar-toggle");
            navToggle.MergeAttributes(new Dictionary<string, object>
            {
                { "type", "button" }, 
                { "data-toggle", "collapse" },
                { "data-target", ".navbar-ex1-collapse" },
            });

            var span = new TagBuilder("span");
            span.AddCssClass("sr-only");

            navToggle.InnerHtml += span;

            span = new TagBuilder("span");
            span.AddCssClass("icon-bar");

            navToggle.InnerHtml += span;

            navToggle.InnerHtml += span;

            navToggle.InnerHtml += span;

            navHeader.InnerHtml += navToggle;

            var navbar = new TagBuilder("div");
            navbar.AddCssClass("collapse navbar-collapse navbar-ex1-collapse");

            var list = new TagBuilder("ul");

            list.AddCssClass("nav navbar-nav");

            foreach (var tab in this.TabsProperties)
            {
                if (!string.IsNullOrEmpty(tab.Name))
                {
                    var item = new TagBuilder("li");

                    if (tab.Selected)
                    {
                        item.AddCssClass("active");
                    }

                    var anchor = new TagBuilder("a");

                    anchor.MergeAttribute("href", "#");
                    anchor.MergeAttribute("data-toggle", "tab");

                    if (!string.IsNullOrEmpty(tab.Id))
                    {
                        anchor.MergeAttribute("data-id", tab.Id);
                    }
                    else
                    {
                        throw new Exception("Tab property Id is not set for tab " + tab.Name);
                    }

                    anchor.InnerHtml += tab.Name;

                    item.InnerHtml += anchor;

                    list.InnerHtml += item;
                }
                else
                {
                    throw new Exception("Tab property Name is not set in BsGroupEditorAttribute");
                }
            }

            navbar.InnerHtml += list;

            return navbar.ToString(); 
        }
        #endregion

        #region Helpers
        private void SetSelectedTab(string name)
        {
            this.TabsProperties.ForEach(x =>
            {
                if (x.Name == name)
                {
                    x.Selected = true;
                }
            });
        }
        #endregion
    }
}
