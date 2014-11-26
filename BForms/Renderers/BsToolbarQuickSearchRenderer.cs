﻿using BForms.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BForms.Renderers
{
    public class BsToolbarQuickSearchRenderer : BsBaseRenderer<BsToolbarQuickSearch>
    {
        public BsToolbarQuickSearchRenderer(){}

        public BsToolbarQuickSearchRenderer(BsToolbarQuickSearch builder)
            : base(builder)
        { 
        }

        // Step 4: implement Render html function. Here you decide how your 
        // custom control will look like based on the customization settings
        /// <summary>
        /// Renders custom control
        /// </summary>
        public override string Render()
        {
            var liBuilder = new TagBuilder("li");

            var divBuilder = new TagBuilder("div");

            divBuilder.AddCssClass("navbar-form");
            divBuilder.MergeAttribute("role","search");

            var inputGroupBuilder = new TagBuilder("div");
            inputGroupBuilder.AddCssClass("form-group bs-quick_search");

            var inputBuilder = new TagBuilder("input");
            inputBuilder.AddCssClass("form-control bs-text");
            inputBuilder.MergeAttribute("type", "search");
            inputBuilder.MergeAttribute("placeholder", this.Builder.placeholder);

            inputGroupBuilder.InnerHtml += inputBuilder.ToString(TagRenderMode.SelfClosing);

            divBuilder.InnerHtml += inputGroupBuilder;

            liBuilder.InnerHtml += divBuilder;

            return liBuilder.ToString();
        }
    }
}
