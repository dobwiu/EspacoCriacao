using System.Web;
using System.Web.Optimization;

namespace EcWebApp
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/methods_pt.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/icheck.js",
                      "~/Scripts/bootbox.js",
                      "~/Scripts/bootstrap-notify.js",
                      "~/Scripts/jquery.slimscroll.js",
                      "~/Scripts/moment-with-locales.js",
                      "~/Scripts/bootstrap-datetimepicker.js"));

            bundles.Add(new ScriptBundle("~/bundles/datatables").Include(
                      "~/Scripts/datatables/jquery.dataTables.js",
                      "~/Scripts/datatables/dataTables.bootstrap.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/font-awesome.css",
                      "~/Content/ionicons.css",
                      "~/Content/animate.css",
                      "~/Content/AdminLTE.css",
                      "~/Content/skins/skin-blue.css",
                      "~/Content/bootstrap-datetimepicker.css"));

            bundles.Add(new StyleBundle("~/Content/datatables").Include(
             "~/Content/datatables/css/dataTables.bootstrap.css",
             "~/Content/datatables/css/fixedHeader.bootstrap.css",
             "~/Content/datatables/css/responsive.bootstrap.css",
             "~/Content/datatables/css/scroller.bootstrap.css"));
        }
    }
}
