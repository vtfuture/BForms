﻿using BForms.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BForms.Renderers
{
    public class BsToolbarItemGroupActionRenderer<TToolbar> : BsBaseRenderer<BsToolbarItemGroupActionLink<TToolbar>>
    {
        public BsToolbarItemGroupActionRenderer()
        {

        }

        public BsToolbarItemGroupActionRenderer(BsToolbarItemGroupActionLink<TToolbar> builder)
            : base(builder)
        {

        }

        /// <summary>
        /// Renders component
        /// </summary>
        public override string Render()
        {
            var actionBuilder = new TagBuilder("a");
            actionBuilder.AddCssClass(this.Builder.descriptorClass);
            actionBuilder.AddCssClass(this.Builder.styleClasses);
            actionBuilder.MergeAttribute("href", this.Builder.href ?? "#");

            if (!string.IsNullOrEmpty(this.Builder.title))
            {
                actionBuilder.MergeAttribute("title", this.Builder.title);
            }

            actionBuilder.InnerHtml += (this.Builder.glyphIcon.HasValue
                ? GetGlyphicon(this.Builder.glyphIcon.Value) + " "
                : "") + this.Builder.text;

            return actionBuilder.ToString();
        }
    }
}
