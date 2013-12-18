using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace BForms.Docs.Helpers
{
    public class ImageResult : ActionResult
    {
        public ImageResult()
        {
        }

        public byte[] ImageData
        {
            get;
            set;
        }

        public MemoryStream ImageStream
        {
            get;
            set;
        }

        public string MimeType
        {
            get;
            set;
        }

        public HttpCacheability Cacheability
        {
            get;
            set;
        }

        public string ETag
        {
            get;
            set;
        }

        public DateTime? Expires
        {
            get;
            set;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (this.ImageData == null && ImageStream == null)
            {
                throw new ArgumentNullException("ImageData or ImageStream");
            }

            if (string.IsNullOrEmpty(this.MimeType))
            {
                throw new ArgumentNullException("MimeType");
            }

            context.HttpContext.Response.ContentType = this.MimeType;

            if (!string.IsNullOrEmpty(this.ETag))
            {
                context.HttpContext.Response.Cache.SetETag(this.ETag);
            }

            if (this.Expires.HasValue)
            {
                context.HttpContext.Response.Cache.SetCacheability(this.Cacheability);
                context.HttpContext.Response.Cache.SetExpires(this.Expires.Value);
            }

            if (ImageStream != null)
            {
                ImageData = ImageStream.ToArray();
            }
            context.HttpContext.Response.BinaryWrite(this.ImageData);
            context.HttpContext.Response.Flush();
        }
    }
}
