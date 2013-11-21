using System.Web;
using System.Web.Optimization;

namespace BForms.EndToEnd
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/css")
                .Include("~/Scripts/BForms/Components/Bootstrap/css/*.css", new CssRewriteUrlTransform())
                .Include("~/Scripts/BForms/Plugins/Datepicker/css/*.css", new CssRewriteUrlTransform())
                .Include("~/Scripts/BForms/Plugins/Select2/css/*.css", new CssRewriteUrlTransform())
                .Include("~/Scripts/BForms/Stylesheets/*.css", new CssRewriteUrlTransform())
                .Include("~/Content/Stylesheets/*.css", new CssRewriteUrlTransform())
                );
        }
    }
}
