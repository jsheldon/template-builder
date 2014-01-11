using System.Web.Optimization;

namespace Forumz.Web.Core.Startup
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Assets/Js/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Assets/Js/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Assets/Js/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Assets/Js/bootstrap.js",
                "~/Assets/Js/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Assets/Css/bootstrap.css",
                "~/Assets/Css/site.css"));
        }
    }
}