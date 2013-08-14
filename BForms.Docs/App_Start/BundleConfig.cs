using System.Web;
using System.Web.Optimization;

namespace BForms.Docs
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/BForms/css").Include(
                      "~/Scripts/BForms/Components/Bootstrap/css/*.css",
                      "~/Scripts/BForms/Stylesheets/*.css",
                      "~/Content/Stylesheets/*.css"));
        }
    }
}