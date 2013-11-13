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

namespace BForms.GroupEditor
{
    public class BsEditorTabConfigurator<TModel> : BaseComponent
    {
        #region Properties and Constructor
        internal Dictionary<object, BsEditorTabBuilder> Tabs { get; set; }
        private BsEditorNavBuilder navBuilder { get; set; }

        public BsEditorTabConfigurator(ViewContext viewContext)
        {
            this.Tabs = new Dictionary<object, BsEditorTabBuilder>();
            this.navBuilder = new BsEditorNavBuilder();
            this.viewContext = viewContext;
        }
        #endregion

        #region Config
        public BsEditorTabBuilder<TRow, TSearch, TNew> For<TRow, TSearch, TNew>(Expression<Func<TModel, BsGroupEditor<TRow, TSearch, TNew>>> expression) where TRow : new()
        {
            var builder = this.GetTab(expression);

            return builder as BsEditorTabBuilder<TRow, TSearch, TNew>;
        }

        public BsEditorTabBuilder<TRow, TSearch> For<TRow, TSearch>(Expression<Func<TModel, BsGroupEditor<TRow, TSearch>>> expression) where TRow : new()
        {
            var builder = this.GetTab(expression);

            return builder as BsEditorTabBuilder<TRow, TSearch>;
        }

        public BsEditorTabBuilder<TRow> For<TRow>(Expression<Func<TModel, BsGroupEditor<TRow>>> expression) where TRow : new()
        {
            var builder = this.GetTab(expression);

            return builder as BsEditorTabBuilder<TRow>;
        }

        private BsEditorTabBuilder GetTab<TValue>(Expression<Func<TModel, TValue>> expression)
        {
            var prop = expression.GetPropertyInfo<TModel, TValue>();

            BsGroupEditorAttribute attr = null;

            if (ReflectionHelpers.TryGetAttribute(prop, out attr))
            {
                var id = attr.Id;

                return this.Tabs[id];
            }

            throw new Exception("Property " + prop.Name + " has no BsGroupEditorAttribute");
        }
        #endregion

        #region Render
        public override string Render()
        {
            var result = navBuilder.Render();

            foreach (var tab in this.Tabs)
            {
                result += tab.Value.Render();
            }

            return result;
        }
        #endregion

        #region Helpers
        private void InsertTab<TRow>(object id, BsEditorTabBuilder<TRow> tabBuilder) where TRow : new ()
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

        internal void AddTab(BsGroupEditorAttribute attr)
        {
            navBuilder.AddTab(attr);
        }

        private void Add<TRow>(BsGroupEditorAttribute attr, BsGroupEditor<TRow> model) where TRow : new()
        {
            var tab = new BsEditorTabBuilder<TRow>(model.Grid, model.InlineSearch, this.viewContext)
                       .DisplayName(attr.Name)
                       .Id(attr.Id)
                       .Selected(attr.Selected);

            if (attr.Selected)
            {
                navBuilder.Selected(attr.Id);
            }

            InsertTab(attr.Id, tab);
        }

        private void AddWithSearch<TRow, TSearch>(BsGroupEditorAttribute attr, BsGroupEditor<TRow, TSearch> model) where TRow : new()
        {
            var tab = new BsEditorTabBuilder<TRow, TSearch>(model, this.viewContext)
                        .DisplayName(attr.Name)
                        .Id(attr.Id)
                        .Selected(attr.Selected);

            if (attr.Selected)
            {
                navBuilder.Selected(attr.Id);
            }

            InsertTab(attr.Id, tab);
        }

        private void AddWithNew<TRow, TSearch, TNew>(BsGroupEditorAttribute attr, BsGroupEditor<TRow, TSearch, TNew> model) where TRow : new()
        {
            var tab = new BsEditorTabBuilder<TRow, TSearch, TNew>(model, this.viewContext)
                        .DisplayName(attr.Name)
                        .Id(attr.Id)
                        .Selected(attr.Selected);

            if (attr.Selected)
            {
                navBuilder.Selected(attr.Id);
            }

            InsertTab(attr.Id, tab);
        }
        #endregion
    }
}
