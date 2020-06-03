using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NetCore.Angular.AzureB2C.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class ConfigController : ControllerBase
	{
		private readonly ILogger<ConfigController> _logger;

		private readonly SettingsDto _settingsDto;

		public ConfigController(ILogger<ConfigController> logger, IOptions<SettingsDto> settingsDto)
		{
			_logger = logger;
			_settingsDto = settingsDto.Value;
		}

		[HttpGet]
		public IActionResult Get()
		{
			return Ok(_settingsDto);
		}
	}
}