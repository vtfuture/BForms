using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BForms.Utilities
{
    internal static class ActionResultHelpers
    {
        internal static void AddSupportForCompression(this ControllerContext context)
        {
            var acceptEncoding = ((context.HttpContext.Request.Headers["Accept-Encoding"] as string) ?? string.Empty).ToLower();
            if (acceptEncoding.Contains("gzip"))
            {
                context.HttpContext.Response.Filter = new System.IO.Compression.GZipStream(context.HttpContext.Response.Filter,
                System.IO.Compression.CompressionMode.Compress);

                context.HttpContext.Response.AddHeader("Content-Encoding", "gzip");
            }
            else if (acceptEncoding.Contains("deflate"))
            {
                context.HttpContext.Response.Filter = new System.IO.Compression.DeflateStream(context.HttpContext.Response.Filter,
                System.IO.Compression.CompressionMode.Compress);
                context.HttpContext.Response.AddHeader("Content-Encoding", "deflate");
            }
            else
            {
                context.HttpContext.Response.AppendHeader("Content-Encoding", "utf-8");
            }
        }
    }
}
