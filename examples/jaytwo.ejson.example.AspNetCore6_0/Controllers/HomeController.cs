using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace jaytwo.ejson.example.AspNetCore6_0.Controllers;

public class HomeController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public HomeController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [Route("/")]
    public IActionResult Index()
    {
        var message = $"[{GetType().Assembly.GetName().Name}]\n";
        message += $"Your super secret is: {_configuration["superSecret"]}\n";
        message += $"Your secret healthcheck is: {_configuration["secretHealthcheck"]}\n";

        return Content(message);
    }
}
