using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using CodeFirstMembershipSharp;

namespace Solbakken
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            BundleTable.Bundles.RegisterTemplateBundles();
            RegisterBundles();
            //Database.SetInitializer(new DataContextInitializer());
        }

        private void RegisterBundles()
        {
            var bundle = new Bundle("~/Scripts/cors/js", new JsMinify());
            bundle.AddDirectory("~/Scripts/cors", "*.js", true);
            BundleTable.Bundles.Add(bundle);
            bundle = new Bundle("~/Scripts/wow/js", new JsMinify());
            bundle.AddDirectory("~/Scripts/wow", "*.js", true);
            BundleTable.Bundles.Add(bundle);
            bundle = new Bundle("~/Content/wow/css", new CssMinify());
            bundle.AddDirectory("~/Content/wow", "*.css", true);
            BundleTable.Bundles.Add(bundle);
            bundle = new Bundle("~/Content/fileupload/css", new CssMinify());
            bundle.AddDirectory("~/Content/fileupload", "*.css", true);
            BundleTable.Bundles.Add(bundle);
        }
    }
}