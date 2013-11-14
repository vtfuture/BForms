<<<<<<< ours
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Editor;
using System.Web.Routing;
using System.Xml.Linq;
using BForms.Models;
using BForms.Mvc;
using BForms.Utilities;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Wordprocessing;

namespace BForms.Html
{
    public static class SortableListExtensions
    {

        public static BsSortableListHtmlBuilder<TOrderModel> BsSortableListFor<TOrderModel>(this HtmlHelper helper,
            IEnumerable<TOrderModel> model,
            Expression<Func<TOrderModel, BsSortableListConfiguration>> config,
            Expression<Func<TOrderModel, HtmlProperties>> listProperties,
            Expression<Func<TOrderModel, HtmlProperties>> itemProperties,
            Expression<Func<TOrderModel, HtmlProperties>> labelProperties,
            Expression<Func<TOrderModel, HtmlProperties>> badgeProperties)
        {
            if (config == null)
            {
                throw new Exception("Configuration expression cannot be null");
            }

            return new BsSortableListHtmlBuilder<TOrderModel>(model, helper.ViewContext, config, listProperties, itemProperties, labelProperties, badgeProperties);
        }

        public static BsSortableListHtmlBuilder<TOrderModel> BsSortableListFor<TOrderModel>(this HtmlHelper helper,
            IEnumerable<TOrderModel> model,
            Expression<Func<TOrderModel, BsSortableListConfiguration>> config)
        {
            return BsSortableListFor(helper, model, config, null, null, null, null);
        }

        public static BsSortableListHtmlBuilder<TOrderModel> BsSortableListFor<TOrderModel>(this HtmlHelper helper,
            IEnumerable<TOrderModel> model,
            Expression<Func<TOrderModel, BsSortableListConfiguration>> config,
            Expression<Func<TOrderModel, HtmlProperties>> listProperties)
        {
            return BsSortableListFor(helper, model, config, listProperties, null, null, null);
        }

        public static BsSortableListHtmlBuilder<TOrderModel> BsSortableListFor<TOrderModel>(this HtmlHelper helper,
            IEnumerable<TOrderModel> model,
            Expression<Func<TOrderModel, BsSortableListConfiguration>> config,
            Expression<Func<TOrderModel, HtmlProperties>> listProperties,
            Expression<Func<TOrderModel, HtmlProperties>> itemProperties)
        {
            return BsSortableListFor(helper, model, config, listProperties, itemProperties, null, null);
        }

    }

    public class SortableListItemWrapper
    {
        public TagBuilder LabelTag { get; set; }
        public TagBuilder ItemTag { get; set; }
        public TagBuilder BadgeTag { get; set; }

        /// <summary>
        /// @RootTag represents the tag which will wrap around any nested list
        /// </summary>
        public TagBuilder RootTag { get; set; }

        public List<SortableListItemWrapper> Children { get; set; }

        public SortableListItemWrapper(List<SortableListItemWrapper> children)
        {
            RootTag = new TagBuilder("ol");
            ItemTag = new TagBuilder("li");
            LabelTag = new TagBuilder("p");
            BadgeTag = new TagBuilder("span");

            Children = children;
        }

        public SortableListItemWrapper(TagBuilder root = null,
                                       TagBuilder item = null,
                                       TagBuilder label = null,
                                       TagBuilder badge = null,
                                       List<SortableListItemWrapper> children = null)
        {
            RootTag = root ?? new TagBuilder("ol");
            ItemTag = item ?? new TagBuilder("li");
            LabelTag = label ?? new TagBuilder("p");
            BadgeTag = badge ?? new TagBuilder("span");

            Children = children;
        }
    }

    public class HtmlProperties
    {
        public string Text { get; set; }
        public IDictionary<string, object> HtmlAttributes { get; set; }
        public IDictionary<string, object> DataAttributes { get; set; }

        public HtmlProperties()
        {
            Text = String.Empty;
            HtmlAttributes = new Dictionary<string, object>();
            DataAttributes = new Dictionary<string, object>();
        }

        public HtmlProperties(string text,
            IDictionary<string, object> htmlAttributes,
            IDictionary<string, object> dataAttributes)
        {
            Text = text;
            HtmlAttributes = htmlAttributes ?? new Dictionary<string, object>();
            DataAttributes = dataAttributes ?? new Dictionary<string, object>();
        }
    }

    public class UnwrappedHtmlProperties
    {
        string Text { get; set; }
        public object HtmlAttributes { get; set; }
        public object DataAttributes { get; set; }
    }

    public class BsSortableListConfiguration
    {
        public string Id { get; set; }
        public string Order { get; set; }
        public string Text { get; set; }
    }


    public class BsSortableListHtmlBuilder<TModel> : BaseComponent
    {
        public SortableListItemWrapper List { get; set; }
        public IEnumerable<TModel> Model { get; set; }

        // Each of these expressions represents the last given configuration for a specific property
        // Storing them prevents erasing allready configured properties after each @List rebuilding
        private Expression<Func<TModel, BsSortableListConfiguration>> _config;
        private Expression<Func<TModel, HtmlProperties>> _globalProperties;
        private Expression<Func<TModel, HtmlProperties>> _itemProperties;
        private Expression<Func<TModel, HtmlProperties>> _labelProperties;
        private Expression<Func<TModel, HtmlProperties>> _badgeProperties;


        public BsSortableListHtmlBuilder(IEnumerable<TModel> model,
            ViewContext viewContext,
            Expression<Func<TModel, BsSortableListConfiguration>> config,
            Expression<Func<TModel, HtmlProperties>> globalProperties,
            Expression<Func<TModel, HtmlProperties>> itemProperties,
            Expression<Func<TModel, HtmlProperties>> labelProperties,
            Expression<Func<TModel, HtmlProperties>> badgeProperties) :
            base(viewContext)
        {
            Model = model;

            this.List = Build(model, config, globalProperties, itemProperties, labelProperties, badgeProperties);

            _config = config;
            _globalProperties = globalProperties;
            _itemProperties = itemProperties;
            _labelProperties = labelProperties;
            _badgeProperties = badgeProperties;
        }

        #region Build & Render

        private SortableListItemWrapper Build(IEnumerable<TModel> modelList,
            Expression<Func<TModel, BsSortableListConfiguration>> config,
            Expression<Func<TModel, HtmlProperties>> globalProperties,
            Expression<Func<TModel, HtmlProperties>> itemProperties,
            Expression<Func<TModel, HtmlProperties>> labelProperties,
            Expression<Func<TModel, HtmlProperties>> badgeProperties)
        {
            var sortableListItemWrapper = new SortableListItemWrapper();

            if (modelList != null && modelList.Any())
            {
                sortableListItemWrapper.Children = new List<SortableListItemWrapper>();

                foreach (var item in modelList)
                {
                    sortableListItemWrapper.Children.Add(Build(item, config, globalProperties, itemProperties, labelProperties, badgeProperties));
                }
            }

            return sortableListItemWrapper;
        }

        private SortableListItemWrapper Build(TModel model,
            Expression<Func<TModel, BsSortableListConfiguration>> config,
            Expression<Func<TModel, HtmlProperties>> globalProperties,
            Expression<Func<TModel, HtmlProperties>> itemProperties,
            Expression<Func<TModel, HtmlProperties>> labelProperties,
            Expression<Func<TModel, HtmlProperties>> badgeProperties)
        {
            var sortableListItemWrapper = new SortableListItemWrapper();

            #region Apply attributes

            var itemProps = itemProperties != null ? itemProperties.Compile().Invoke(model) : new HtmlProperties(null, null, null);
            var labelProps = labelProperties != null ? labelProperties.Compile().Invoke(model) : new HtmlProperties(null, null, null);
            var badgeProps = badgeProperties != null ? badgeProperties.Compile().Invoke(model) : new HtmlProperties(null, null, null);
            var globalProps = globalProperties != null ? globalProperties.Compile().Invoke(model) : new HtmlProperties(null, null, null);
            var configProps = config != null ? config.Compile().Invoke(model) : null;

            #region Root

            sortableListItemWrapper.RootTag.MergeAttributes(globalProps.HtmlAttributes ?? new Dictionary<string, object>());
            sortableListItemWrapper.RootTag.MergeAttributes(globalProps.DataAttributes != null ? globalProps.DataAttributes.ToDictionary(x => "data-" + x.Key, y => y.Value) : new Dictionary<string, object>());

            #endregion

            #region Item

            if (configProps != null)
            {
                itemProps.DataAttributes.Add("id", configProps.Id);
                itemProps.DataAttributes.Add("order", configProps.Order);
            }

            sortableListItemWrapper.ItemTag.MergeAttributes(itemProps.HtmlAttributes ?? new Dictionary<string, object>());
            sortableListItemWrapper.ItemTag.MergeAttributes(itemProps.DataAttributes != null ? itemProps.DataAttributes.ToDictionary(x => "data-" + x.Key, y => y.Value) : new Dictionary<string, object>());
            sortableListItemWrapper.LabelTag.InnerHtml += configProps != null ? configProps.Text : String.Empty; //itemProps.Text;

            #endregion

            #region Label


            sortableListItemWrapper.LabelTag.MergeAttributes(labelProps.HtmlAttributes ?? new Dictionary<string, object>());
            sortableListItemWrapper.LabelTag.MergeAttributes(labelProps.DataAttributes != null ? labelProps.DataAttributes.ToDictionary(x => "data-" + x.Key, y => y.Value) : new Dictionary<string, object>());
            sortableListItemWrapper.LabelTag.InnerHtml += labelProps.Text;


            #endregion

            #region Badge


            sortableListItemWrapper.BadgeTag.MergeAttributes(badgeProps.HtmlAttributes ?? new Dictionary<string, object>());
            sortableListItemWrapper.BadgeTag.MergeAttributes(badgeProps.DataAttributes != null ? badgeProps.DataAttributes.ToDictionary(x => "data-" + x.Key, y => y.Value) : new Dictionary<string, object>());
            sortableListItemWrapper.BadgeTag.InnerHtml += badgeProps.Text;



            #endregion

            #endregion

            #region Build nested list

            var properties = model.GetType().GetProperties();
            PropertyInfo nestedValuesProperty = null;

            // Find the first property in @model's properties 
            // decorated with a BsControlAttribute matching BsControlType.SortableList
            foreach (PropertyInfo prop in properties)
            {
                if (nestedValuesProperty != null)
                {
                    break;
                }

                var attributes = prop.GetCustomAttributes(true);

                foreach (var attr in attributes)
                {
                    var bsControl = attr as BsControlAttribute;

                    if (bsControl != null && bsControl.ControlType == BsControlType.SortableList)
                    {
                        nestedValuesProperty = prop;
                        break;
                    }
                }
            }

            if (nestedValuesProperty != null)
            {
                var value = nestedValuesProperty.GetValue(model, null);
                sortableListItemWrapper.Children = new List<SortableListItemWrapper>();

                if (value != null && (value as IEnumerable<TModel>) != null)
                {

                    foreach (var item in (value as IEnumerable<TModel>))
                    {
                        sortableListItemWrapper.Children.Add(Build(item, config, globalProperties, itemProperties, labelProperties, badgeProperties));
                    }
                }

            }

            #endregion

            return sortableListItemWrapper;
        }

        public string RenderInternal(SortableListItemWrapper list)
        {
            var htmlString = String.Empty;

            var div = new TagBuilder("div");
            var span = new TagBuilder("span");

            span.Attributes.Add("class", "grippy");
            div.Attributes.Add("class", "custom-checkbox");

            if (list.LabelTag.Attributes.ContainsKey("class"))
            {
                list.LabelTag.Attributes["class"] += " item_label";
            }
            else
            {
                list.LabelTag.Attributes.Add("class", "item_label");
            }

            var textFiller = new TagBuilder("text") { InnerHtml = "&nbsp;" };

            if (list.BadgeTag.Attributes.ContainsKey("class"))
            {
                list.BadgeTag.Attributes["class"] += " label";
            }
            else
            {
                list.BadgeTag.Attributes.Add("class", "label");
            }


            list.LabelTag.InnerHtml = textFiller.ToString() +
                                      list.BadgeTag.ToString() +
                                      textFiller.ToString() +
                                      list.LabelTag.InnerHtml;

            div.InnerHtml = span.ToString() + list.LabelTag.ToString();

            #region Nested elements

            if (list.Children != null && list.Children.Any())
            {
                foreach (var child in list.Children)
                {
                    if (list.RootTag.Attributes.ContainsKey("class"))
                    {
                        list.RootTag.Attributes["class"] += " bs-sortable";
                    }
                    else
                    {
                        list.RootTag.Attributes.Add("class", "bs-sortable");
                    }

                    list.RootTag.InnerHtml += RenderInternal(child);
                }
            }

            #endregion

            list.ItemTag.InnerHtml = div.ToString() + list.RootTag.ToString();

            htmlString = list.ItemTag.ToString();

            return htmlString;
        }

        public override string Render()
        {
            var innerHtml = String.Empty;

            foreach (var item in List.Children)
            {
                innerHtml += RenderInternal(item);
            }

            var ol = new TagBuilder("ol") { InnerHtml = innerHtml };

            ol.Attributes.Add("class", "bs-sortable");

            return ol.ToString();
        }

        #endregion


        #region Builder methods

        public BsSortableListHtmlBuilder<TModel> ItemProperties(Expression<Func<TModel, HtmlProperties>> itemProperties)
        {
            this.List = Build(this.Model, _config, _globalProperties, itemProperties, _labelProperties, _badgeProperties);

            _itemProperties = itemProperties;

            return this;
        }

        public BsSortableListHtmlBuilder<TModel> LabelProperties(Expression<Func<TModel, HtmlProperties>> labelProperties)
        {
            this.List = Build(this.Model, _config, _globalProperties, _itemProperties, labelProperties, _badgeProperties);

            _labelProperties = labelProperties;

            return this;
        }

        public BsSortableListHtmlBuilder<TModel> BadgeProperties(Expression<Func<TModel, HtmlProperties>> badgeProperties)
        {
            this.List = Build(this.Model, _config, _globalProperties, _itemProperties, _labelProperties, badgeProperties);

            _badgeProperties = badgeProperties;

            return this;
        }

        public BsSortableListHtmlBuilder<TModel> ListProperties(Expression<Func<TModel, HtmlProperties>> listProperties)
        {
            this.List = Build(this.Model, _config, listProperties, _itemProperties, _labelProperties, _badgeProperties);

            _globalProperties = listProperties;

            return this;
        }

        public BsSortableListHtmlBuilder<TModel> Configure(Expression<Func<TModel, BsSortableListConfiguration>> config)
        {
            this.List = Build(this.Model, config, _globalProperties, _itemProperties, _labelProperties, _badgeProperties);

            _config = config;

            return this;
        }

        #endregion

    }




}

=======
﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BForms.Models;
using BForms.Mvc;
using BForms.Utilities;

namespace BForms.Html
{
    public static class SortableListExtensions
    {

        public static MvcHtmlString BsSortableListFor<TOrderModel>(this HtmlHelper htmlHelper,
            IEnumerable<TOrderModel> model,
            Expression<Func<TOrderModel, string>> textExpression,
            Expression<Func<TOrderModel, string>> idExpression,
            Expression<Func<TOrderModel, string>> orderExpression)
        {
            return BsSortableListFor(htmlHelper, model, textExpression, idExpression, orderExpression,
                                     null, null, null, null, null, null, null);
        }

        public static MvcHtmlString BsSortableListFor<TOrderModel>(this HtmlHelper htmlHelper,
            IEnumerable<TOrderModel> model,
            Expression<Func<TOrderModel, string>> textExpression,
            Expression<Func<TOrderModel, string>> idExpression,
            Expression<Func<TOrderModel, string>> orderExpression,
            Expression<Func<TOrderModel, IDictionary<string, object>>> itemHtmlAttributes,
            Expression<Func<TOrderModel, IDictionary<string, object>>> itemDataAttributes,
            Expression<Func<TOrderModel, IDictionary<string, object>>> labelHtmlAttributes,
            Expression<Func<TOrderModel, string>> badgeText, 
            Expression<Func<TOrderModel, IDictionary<string, object>>> badgeHtmlAttributes)
        {
            return BsSortableListFor(htmlHelper, model, textExpression, idExpression, orderExpression,
                         itemHtmlAttributes, itemDataAttributes, labelHtmlAttributes,
                         badgeText, badgeHtmlAttributes, null, null);
        }

        public static MvcHtmlString BsSortableListFor<TOrderModel>(this HtmlHelper htmlHelper,
            IEnumerable<TOrderModel> model,
            Expression<Func<TOrderModel, string>> textExpression,
            Expression<Func<TOrderModel, string>> idExpression,
            Expression<Func<TOrderModel, string>> orderExpression,
            IDictionary<string, object> htmlAttributes,
            IDictionary<string, object> dataAttributes)
        {
            return BsSortableListFor(htmlHelper, model, textExpression, idExpression, orderExpression,
                         null, null, null, null, null, htmlAttributes, dataAttributes);
        }

        public static MvcHtmlString BsSortableListFor<TOrderModel>(this HtmlHelper htmlHelper,
            IEnumerable<TOrderModel> model,
            Expression<Func<TOrderModel, string>> textExpression,
            Expression<Func<TOrderModel, string>> idExpression,
            Expression<Func<TOrderModel, string>> orderExpression,
            Expression<Func<TOrderModel, IDictionary<string, object>>> itemHtmlAttributes,
            Expression<Func<TOrderModel, IDictionary<string, object>>> itemDataAttributes,
            Expression<Func<TOrderModel, IDictionary<string, object>>> labelHtmlAttributes,
            Expression<Func<TOrderModel, string>> badgeText, 
            Expression<Func<TOrderModel, IDictionary<string, object>>> badgeHtmlAttributes,
            IDictionary<string, object> htmlAttributes,
            IDictionary<string, object> dataAttributes)
        {
            var olTag = new TagBuilder("ol");
            var sortableDescriptorClass = "bs-sortable";

            olTag.Attributes.Add("class", sortableDescriptorClass + " check-list numbers ui-sortable");
            olTag.Attributes.Add("style", "list-style-type: decimal;");

            htmlAttributes = htmlAttributes ?? new Dictionary<string, object>();
            dataAttributes = dataAttributes ?? new Dictionary<string, object>();

            foreach (var attr in dataAttributes)
            {
                if (!htmlAttributes.ContainsKey("data-" + attr.Key))
                {
                    htmlAttributes.Add("data-" + attr.Key, attr.Value);
                }
            }

            olTag.MergeAttributes(htmlAttributes);

            var items = model;

            if (items != null && items.Any())
            {
                foreach (var item in items)
                {
                    olTag.InnerHtml += Render(item, textExpression, orderExpression, idExpression, 
                                            itemHtmlAttributes, itemDataAttributes, labelHtmlAttributes, 
                                            badgeText, badgeHtmlAttributes,
                                            htmlAttributes, dataAttributes);
                }
            }

            return new MvcHtmlString(olTag.ToString());
        }


        private static string Render<TOrderModel>(TOrderModel model,
            Expression<Func<TOrderModel, string>> textExpression,
            Expression<Func<TOrderModel, string>> orderExpression,
            Expression<Func<TOrderModel, string>> idExpression,
            Expression<Func<TOrderModel, IDictionary<string, object>>> itemHtmlAttributes,
            Expression<Func<TOrderModel, IDictionary<string, object>>> itemDataAttributes,
            Expression<Func<TOrderModel, IDictionary<string, object>>> labelHtmlAttributes,
            Expression<Func<TOrderModel, string>> badgeText, 
            Expression<Func<TOrderModel, IDictionary<string, object>>> badgeHtmlAttributes,
            IDictionary<string, object> htmlAttributes,
            IDictionary<string, object> dataAttributes)
        {
            var htmlString = String.Empty;

            #region Get model properties

            var properties = model.GetType().GetProperties();
            PropertyInfo nestedValuesProperty = null;

            // Find the first property in @model's properties 
            // decorated with a BsControlAttribute matching BsControlType.SortableList
            foreach (PropertyInfo prop in properties)
            {
                if (nestedValuesProperty != null)
                {
                    break;
                }

                var attributes = prop.GetCustomAttributes(true);

                foreach (var attr in attributes)
                {
                    var bsControl = attr as BsControlAttribute;

                    if (bsControl != null && bsControl.ControlType == BsControlType.SortableList)
                    {
                        nestedValuesProperty = prop;
                        break;
                    }
                }
            }

            #endregion

            #region Build tags

            var liTag = new TagBuilder("li");
            var divTag = new TagBuilder("div");
            var spanTag = new TagBuilder("span");
            var labelTag = new TagBuilder("p");
            var labelSpanTag = new TagBuilder("span");

            var text = textExpression.Compile().Invoke(model)  ?? String.Empty;
            var order = orderExpression.Compile().Invoke(model) ?? String.Empty;
            var id = idExpression.Compile().Invoke(model) ?? String.Empty;

            divTag.Attributes.Add("class", "custom-checkbox");
            spanTag.Attributes.Add("class", "grippy");

            #region Badge

            var labelSpanTagAttributes = badgeHtmlAttributes != null
                ? badgeHtmlAttributes.Compile().Invoke(model)
                : null;

            if (labelSpanTagAttributes != null)
            {
                foreach (var attr in labelSpanTagAttributes)
                {
                    labelSpanTag.Attributes.Add(attr.Key, attr.Value.ToString());
                }
            }

            if (labelSpanTag.Attributes.ContainsKey("class"))
            {
                labelSpanTag.Attributes["class"] += " label";
            }
            else
            {
                labelSpanTag.Attributes.Add("class", "label");
            }

            var labelSpanTagText = badgeText != null ? badgeText.Compile().Invoke(model) : String.Empty;

            labelSpanTag.InnerHtml = labelSpanTagText;

            #endregion

            #region Label

            if (labelHtmlAttributes != null)
            {
                var labelAttributes = labelHtmlAttributes.Compile().Invoke(model);

                foreach (var attr in labelAttributes)
                {
                    labelTag.Attributes.Add(attr.Key, attr.Value.ToString());
                }
            }

            if (labelTag.Attributes.ContainsKey("class"))
            {
                labelTag.Attributes["class"] += " entity_name";
            }
            else
            {
                labelTag.Attributes.Add("class", "entity_name");
            }

            labelTag.Attributes.Add("style", "display: block;");


            labelTag.InnerHtml += new TagBuilder("text") { InnerHtml = "&nbsp;" }
                      + labelSpanTag.ToString()
                      + new TagBuilder("text") { InnerHtml = "&nbsp;" }
                      + text;

            #endregion

            #region List item

            liTag.Attributes.Add("style", "margin-left: 20px; margin-bottom: 7px;");
            liTag.Attributes.Add("data-objid", id);
            liTag.Attributes.Add("data-order", order);

            var liHtmlAttributes = new Dictionary<string, object>();
            var liDataAttributes = new Dictionary<string, object>();

            if (itemHtmlAttributes != null)
            {
                liHtmlAttributes = itemHtmlAttributes.Compile().Invoke(model).ToDictionary(x=> x.Key, y=> y.Value);
            }

            if (itemDataAttributes != null)
            {
                liDataAttributes = itemDataAttributes.Compile().Invoke(model).ToDictionary(x => x.Key, y => y.Value);
            }

            foreach (var attr in liDataAttributes)
            {
                if (!liHtmlAttributes.ContainsKey("data-" + attr.Key))
                {
                    liHtmlAttributes.Add("data-" + attr.Key, attr.Value);
                }
            }

            liTag.MergeAttributes(liHtmlAttributes);

            divTag.InnerHtml += spanTag.ToString() + labelTag.ToString();
            liTag.InnerHtml += divTag.ToString();

            #endregion

            #endregion

            #region Build nested list

            if (nestedValuesProperty != null)
            {
                var value = nestedValuesProperty.GetValue(model, null);

                if (value != null && (value as IEnumerable<TOrderModel>) != null)
                {
                    var newOlTag = new TagBuilder("ol");
                    var sortableDescriptorClass = "bs-sortable";

                    newOlTag.Attributes.Add("class", sortableDescriptorClass + " check-list numbers ui-sortable");
                    newOlTag.Attributes.Add("style", "list-style-type: decimal;");

                    htmlAttributes = htmlAttributes ?? new Dictionary<string, object>();
                    dataAttributes = dataAttributes ?? new Dictionary<string, object>();

                    foreach (var attr in dataAttributes)
                    {
                        if (!htmlAttributes.ContainsKey("data-" + attr.Key))
                        {
                            htmlAttributes.Add("data-" + attr.Key, attr.Value);
                        }
                    }

                    newOlTag.MergeAttributes(htmlAttributes);

                    foreach (var item in (value as IEnumerable<TOrderModel>))
                    {
                        newOlTag.InnerHtml += Render(item, textExpression, orderExpression, idExpression,
                                        itemHtmlAttributes, itemDataAttributes, labelHtmlAttributes,
                                        badgeText, badgeHtmlAttributes,
                                        htmlAttributes, dataAttributes);
                    }

                    liTag.InnerHtml += newOlTag.ToString();
                }

            }

            #endregion

            htmlString += liTag.ToString();

            return htmlString;
        }

    }


}
>>>>>>> theirs
