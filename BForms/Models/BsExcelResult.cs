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
    public class BsExcelResult<T> : ActionResult where T : class
    {
        private string FileName { get; set; }
        private IEnumerable<T> Items { get; set; }

        public BsExcelResult(string fileName, IEnumerable<T> items)
        {
            FileName = fileName;
            Items = items;
        }

        /// <summary> 
        /// Execute the Excel Result. 
        /// </summary> 
        /// <param name="context">Controller context.</param> 
        public override void ExecuteResult(ControllerContext context)
        {
            WriteStream(Items.ToExcel(FileName), FileName);
        }

        /// <summary> 
        /// Writes the memory stream to the browser. 
        /// </summary> 
        /// <param name="memoryStream">Memory stream.</param> 
        /// <param name="fileName">Excel file name.</param> 
        private static void WriteStream(MemoryStream memoryStream, string fileName)
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
