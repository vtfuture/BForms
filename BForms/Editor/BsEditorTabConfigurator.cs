using BForms.Models;
using BForms.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BForms.Utilities;

namespace BForms.Editor
{
    public class BsEditorTabConfigurator<TModel> : BsBaseConfigurator
    {
        #region Properties and Constructor
        protected Dictionary<object, BsEditorTabBuilder> tabs { get; set; }
        protected BsEditorNavBuilder navBuilder { get; set; }

        internal Dictionary<object, BsEditorTabBuilder> Tabs
        {
            get
            {
                return this.tabs;
            }
        }

        internal BsEditorNavBuilder NavigationBuilder
        {
            get
            {
                return this.navBuilder;
            }
        }

        public BsEditorTabConfigurator(ViewContext viewContext) : base(viewContext)
        {
            this.tabs = new Dictionary<object, BsEditorTabBuilder>();
            this.navBuilder = new BsEditorNavBuilder(viewContext);
            this.viewContext = viewContext;
        }

        /// <summary>
        /// Tabs header
        /// </summary>
        public string Title { get; set; }
        #endregion

        #region Public Methods
        public BsEditorTabBuilder<TEditor> For<TEditor>(Expression<Func<TModel, TEditor>> expression)
            where TEditor : IBsEditorTabModel
        {
            var builder = this.GetTab(expression);

            return builder as BsEditorTabBuilder<TEditor>;
        }
        #endregion

        #region Helpers
        private void InsertTab<TEditor, TRow>(object id, BsEditorTabBuilder<TEditor> tabBuilder)
            where TEditor : IBsEditorTabModel
            where TRow : BsItemModel
        {
            if (tabBuilder.IsSelected())
            {
                foreach (var tab in this.Tabs)
                {
                    if (tab.Key == id)
                    {
                        throw new Exception("Duplicate tab id " + id.ToString());
                    }

                    if (tab.Value.IsSelected())
                    {
                        tab.Value.SetSelected(false);
                    }
                }
            }
            this.Tabs.Add(id, tabBuilder);
        }

        private BsEditorTabBuilder GetTab<TValue>(Expression<Func<TModel, TValue>> expression)
        {
            var prop = expression.GetPropertyInfo<TModel, TValue>();

            BsEditorTabAttribute attr = null;

            if (ReflectionHelpers.TryGetAttribute(prop, out attr))
            {
                var id = attr.Id;

                return this.Tabs[id];
            }

            throw new Exception("Property " + prop.Name + " has no BsGroupEditorAttribute");
        }

        internal void AddNavTab(BsEditorTabAttribute attr)
        {
            navBuilder.AddTab(attr);
        }

        private void Add<TEditor, TRow>(BsEditorTabAttribute attr, BsEditorTabModel<TRow> model, List<TabGroupConnection> connections, object[] groupIds)
            where TEditor : IBsEditorTabModel
            where TRow : BsItemModel
        {
            var tab = new BsEditorTabBuilder<TEditor>(model, this.viewContext, connections)
                       .ConnectsWith(groupIds)
                       .DisplayName(attr.Name)
                       .Id(attr.Id)
                       .Selected(attr.Selected);

            if (attr.Editable)
            {
                tab.Editable();
            }

            tab.InitRenderer<TRow>();

            if (attr.Selected)
            {
                navBuilder.Selected(attr.Id);
            }

            InsertTab<TEditor, TRow>(attr.Id, tab);
        }

        internal object[] GetEditableTabIds()
        {
            return this.Tabs.Where(x => x.Value.editable).Select(x => x.Value).ToArray();
        }
        #endregion
    }
}
