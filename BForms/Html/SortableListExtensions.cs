using System;
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

    #region Helper classes

    public class HtmlItemAttributes
    {
        public string Text { get; set; }
        public IDictionary<string, object> HtmlAttributes { get; set; }
        public IDictionary<string, object> DataAttributes { get; set; }
    }

    #endregion


}
