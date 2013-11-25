using BForms.Grid;
using BForms.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BForms.Models
{
    /// <summary>
    /// Action Result used to export an excel file
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BsExcelResult<T> : ActionResult where T : class
    {
        private string fileName { get; set; }
        private IEnumerable<T> items { get; set; }
        private BsGridExcelBuilder<T> builder { get; set; }

        public BsExcelResult(){}

        public BsExcelResult(string fileName, IEnumerable<T> items)
        {
            this.fileName = fileName;
            this.items = items;
        }

        public BsExcelResult(string fileName, BsGridExcelBuilder<T> builder)
        {
            this.fileName = fileName;
            this.builder = builder;
        }
        /// <summary> 
        /// Execute the Excel Result. 
        /// </summary> 
        /// <param name="context">Controller context.</param> 
        public override void ExecuteResult(ControllerContext context)
        {
            var builder = this.builder ?? new BsGridExcelBuilder<T>(fileName, items);
            var stream = builder.ToStream();
            WriteStream(stream, fileName);
        }

        /// <summary> 
        /// Writes the memory stream to the browser. 
        /// </summary> 
        /// <param name="memoryStream">Memory stream.</param> 
        /// <param name="fileName">Excel file name.</param> 
        protected static void WriteStream(MemoryStream memoryStream, string fileName)
        {
            HttpContext context = HttpContext.Current;
            context.Response.Clear();
            context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            context.Response.AddHeader("content-disposition",
              String.Format("attachment;filename={0}", fileName));
            context.Response.BinaryWrite(memoryStream.ToArray());
            context.Response.End();
        }
    }
}
