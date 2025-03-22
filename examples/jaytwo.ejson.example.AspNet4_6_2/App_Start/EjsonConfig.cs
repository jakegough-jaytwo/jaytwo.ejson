using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using jaytwo.ejson.Configuration.AspNet;

namespace jaytwo.ejson.example.AspNet4_6_2
{
    public class EjsonConfig
    {
        public static void ConfigureEjsonAppSecrets()
        {
            new EjsonLoader().Load(HostingEnvironment.MapPath(@"~\appsecrets.json"));
        }
    }
}
