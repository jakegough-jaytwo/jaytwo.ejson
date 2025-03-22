using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace jaytwo.ejson.example.AspNet4_6_2.Controllers
{
    public class ValuesController : ApiController
    {
        public IEnumerable<string> Get()
        {
            var foo = ConfigurationManager.AppSettings["SecretAppSettings"];
            if (!string.IsNullOrEmpty(foo))
            {
                yield return foo!;
            }

            var bar = ConfigurationManager.ConnectionStrings?["SecretConnectionString"]?.ConnectionString;
            if (!string.IsNullOrEmpty(bar))
            {
                yield return bar!;
            }
        }
    }
}
