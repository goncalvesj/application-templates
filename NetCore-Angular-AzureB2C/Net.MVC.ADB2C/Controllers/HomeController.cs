using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Net.MVC.ADB2C.Models;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace Net.MVC.ADB2C.Controllers;

[Authorize]
[AuthorizeForScopes(Scopes = new string[] { "https://yxtc4b2c.onmicrosoft.com/10ea9ce4-7156-470f-891f-9a50a93f68d0/weather.read" })]
public class HomeController : Controller
{
	private readonly ILogger<HomeController> _logger;
	private readonly ITokenAcquisition _tokenAcquisition;


	public HomeController(ILogger<HomeController> logger
		, ITokenAcquisition tokenAcquisition
		)
	{
		_logger = logger;
		_tokenAcquisition = tokenAcquisition;
	}

	public IActionResult Index()
	{
		return View();
	}

    public async Task<IActionResult> GetNames()
    {       
        return Ok(new { Name = "JP" });
    }

    public async Task<IActionResult> Privacy()
	{
		var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] {
						"https://yxtc4b2c.onmicrosoft.com/10ea9ce4-7156-470f-891f-9a50a93f68d0/weather.read"
					});

		ViewBag.Token = accessToken;

		using (var client = new HttpClient())
		{
			client.BaseAddress = new Uri("https://localhost:7093");
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
