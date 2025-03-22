using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace jaytwo.ejson.example.AspNetCore3_0.Controllers
{
    public class HomeController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var message = $"[{GetType().Assembly.GetName().Name}]\n";
            message += $"Your super secret is: {_configuration["superSecret"]}\n";
            message += $"Your secret healthcheck is: {_configuration["secretHealthcheck"]}\n";

            return Content(message);
        }
    }
}
