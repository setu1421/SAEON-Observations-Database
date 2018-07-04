using System.Web;
using System.Web.Optimization;

namespace SAEON.Observations.QuerySite
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/mdbootstrap").Include(
                      "~/node_modules/mdbootstrap/css/bootstrap.css",
                      "~/node_modules/mdbootstrap/css/mdb.css"));

            bundles.Add(new ScriptBundle("~/bundles/mdbootstrap").Include(
                      "~/node_modules/mdbootstrap/js/jquery-3.3.1.min.js",
                      "~/node_modules/mdbootstrap/js/popper.min.js",
                      "~/node_modules/mdbootstrap/js/bootstrap.js",
                      "~/node_modules/mdbootstrap/js/mdb.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/site.css",
                      "~/Content/SAEON.css"));

            BundleTable.EnableOptimizations = true;
        }
    }
}
