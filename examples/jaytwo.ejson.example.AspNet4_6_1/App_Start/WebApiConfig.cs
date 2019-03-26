using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace jaytwo.ejson.example.AspNet4_6_1
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { controller = "Home", id = RouteParameter.Optional }
            );
        }
    }
}
