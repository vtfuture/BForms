using BForms.Panels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BForms.Renderers
{
    public class BsPanelsBaseRenderer<TModel> : BsBaseRenderer<BsPanelsHtmlBuilder<TModel>>
    {
        public BsPanelsBaseRenderer(){}

        public BsPanelsBaseRenderer(BsPanelsHtmlBuilder<TModel> builder)
            : base(builder)
        { 
        }

        public virtual TagBuilder GetContainer()
        {
            var container = new TagBuilder("div");

            container.AddCssClass("panel-group");

            return container;
        }

        public virtual string RenderPanels()
        {
            var panelsHtml = string.Empty;

            foreach (var panel in this.Builder.panelsConfig.Panels)
            {
                if (string.IsNullOrEmpty(panel.readonlyUrl) && !string.IsNullOrEmpty(this.Builder.panelsConfig.readonlyUrl))
                {
                    panel.ReadonlyUrl(this.Builder.panelsConfig.readonlyUrl);
                }

                if (string.IsNullOrEmpty(panel.editableUrl) && !string.IsNullOrEmpty(this.Builder.panelsConfig.editableUrl) && panel.isEditable)
                {
                    panel.EditableUrl(this.Builder.panelsConfig.editableUrl);
                }

                if (string.IsNullOrEmpty(panel.saveUrl) && !string.IsNullOrEmpty(this.Builder.panelsConfig.saveUrl))
                {
                    panel.SaveUrl(this.Builder.panelsConfig.saveUrl);
                }

                panelsHtml += panel.ToString();
            }

            return panelsHtml;
        }

        public override string Render()
        {
            var container = this.GetContainer();

            container.InnerHtml += this.RenderPanels();

            return container.ToString();
        }
    }
}
