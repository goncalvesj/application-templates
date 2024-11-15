using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Net.MVC.Models;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace Net.MVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ITokenAcquisition _tokenAcquisition;
    private readonly IConfiguration _configuration;

    public HomeController(ILogger<HomeController> logger
        , ITokenAcquisition tokenAcquisition
        , IConfiguration configuration
        )
    {
        _logger = logger;
        _tokenAcquisition = tokenAcquisition;
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        return View();
    }

    [AuthorizeForScopes(ScopeKeySection = "WeatherApi:Scope")]
    public async Task<IActionResult> Authenticated()
    {
        var scope = _configuration["WeatherApi:Scope"] ?? "";

        var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { scope }); ;

        ViewBag.Token = accessToken;

        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri(_configuration["WeatherApi:BaseAddress"] ?? "");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await client.GetAsync("/weatherforecast");
            response.EnsureSuccessStatusCode();

            var stringResult = await response.Content.ReadAsStringAsync();
            ViewBag.ApiResult = stringResult;
        }

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
