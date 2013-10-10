using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BForms.Grid
{
    public class BsHtmlTag : IDisposable
    {

        private bool _disposed;
        private readonly TextWriter _writer;
        private readonly string _tagName;

        public BsHtmlTag(ViewContext viewContext, string tagName)
        {
            if (viewContext == null)
            {
                throw new ArgumentNullException("viewContext");
            }

            _writer = viewContext.Writer;
            _tagName = tagName;
        }

        public void Dispose()
        {
            Dispose(true /* disposing */);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
                _writer.Write("</" + _tagName + ">");
            }
        }
    }
}