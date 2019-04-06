using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MyOnlineShop
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
               name: "Cart",
               url: "Cart/{action}/{id}",
               defaults: new { controller = "Cart", action = "Index", id = UrlParameter.Optional },
               namespaces: new[] { "MyOnlineShop.Controllers" }
               );

            routes.MapRoute(
                name: "Shop",
                url: "Shop/{action}/{name}",
                defaults: new { controller = "Shop", action = "Categories", name = UrlParameter.Optional },
                namespaces: new[] { "MyOnlineShop.Controllers" }
                );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
