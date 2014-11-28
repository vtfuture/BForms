using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using BForms.Models;
using BForms.Utilities;
using DocumentFormat.OpenXml.Office.CustomUI;

namespace BForms.Html
{
    /// <summary>
    /// Represent bootstrap support for upload control
    /// </summary>
    public static class UploadExtensions
    {
        internal static MvcHtmlString UploadForInternal<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression,
            string format, IDictionary<string, object> htmlAttributes)
        {
            htmlAttributes.MergeAttribute("type", "file");
            var fileInput = htmlHelper.TextBoxFor(expression, format, htmlAttributes);

            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

            var placeholder = string.IsNullOrEmpty(metadata.Watermark) ? Resources.BFormsResources.BF_ChooseFile : metadata.Watermark;

            var formControl = new TagBuilder("div");
            formControl.AddCssClass("form-control bs-uploadFormControl");

            formControl.MergeAttribute("data-title", placeholder);
            formControl.MergeAttribute("data-choosefile", placeholder);
            formControl.InnerHtml += fileInput;

            var cameraGlyph = htmlHelper.BsGlyphicon(Glyphicon.File);

            var uploadBtn = new TagBuilder("span");
            uploadBtn.AddCssClass("btn");
            uploadBtn.AddCssClass("btn-theme");
            uploadBtn.InnerHtml += Resources.BFormsResources.BF_Upload;

            formControl.InnerHtml += cameraGlyph;
            formControl.InnerHtml += uploadBtn;

            var removeA = new TagBuilder("a");
            removeA.MergeAttribute("style", "display:none");
            removeA.AddCssClass("btn");
            removeA.AddCssClass("btn-danger");
            removeA.AddCssClass("input-group-addon");
            removeA.AddCssClass("bs-removeUploadBtn");
            var removeGlyph = htmlHelper.BsGlyphicon(Glyphicon.Remove);
            removeA.InnerHtml += removeGlyph;

            var inputGroupWrapper = new TagBuilder("div");
            inputGroupWrapper.AddCssClass("input-file-upload");
            inputGroupWrapper.AddCssClass("bs-uploadWrapper");

            inputGroupWrapper.InnerHtml += formControl;
            inputGroupWrapper.InnerHtml += removeA;

            return new MvcHtmlString(inputGroupWrapper.ToString());
        }
    }
}
