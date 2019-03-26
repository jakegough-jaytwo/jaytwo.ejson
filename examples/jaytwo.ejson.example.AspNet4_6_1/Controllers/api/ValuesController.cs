using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace jaytwo.ejson.example.AspNet4_6_1.Controllers
{
    public class ValuesController : ApiController
    {
        public IEnumerable<string> Get()
        {
            yield return ConfigurationManager.AppSettings["SecretAppSettings"];
            yield return ConfigurationManager.ConnectionStrings["SecretConnectionString"]?.ConnectionString;
        }
    }
}