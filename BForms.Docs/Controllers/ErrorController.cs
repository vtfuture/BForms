using System;
using System.Web.Mvc;

namespace BForms.Docs.Controllers
{
    public class ErrorController : BaseController
    {
        //
        // GET: /Error/
        public ActionResult Index()
        {
            return View("Error");
        }

        public ViewResult NotFound()
        {
            return View("NotFound");
        }

        //public void LogException(ErrorResponse[] errors)
        //{
        //}

        //public class ErrorResponse
        //{
        //    public int ColumnNumber { get; set; }

        //    public string ErrorMessage { get; set; }

        //    public string ErrorType { get; set; }

        //    public string Stack { get; set; }

        //    public DateTime Timestamp { get; set; }

        //    public string Url { get; set; }

        //    public string Href { get; set; }

        //    public string ReadyState { get; set; }

        //    public string UserAgent { get; set; }

        //    public string Referrer { get; set; }
        //}
    }
}