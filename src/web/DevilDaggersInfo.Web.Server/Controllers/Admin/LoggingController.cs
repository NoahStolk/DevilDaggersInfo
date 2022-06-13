using DevilDaggersInfo.Web.Core.Claims;
using Microsoft.AspNetCore.Authorization;

namespace DevilDaggersInfo.Web.Server.Controllers.Admin;

[Route("api/admin/logging")]
[ApiController]
[Authorize(Roles = Roles.Admin)]
public class LoggingController : ControllerBase
{
	private readonly ILogger<LoggingController> _logger;

	public LoggingController(ILogger<LoggingController> logger)
	{
		_logger = logger;
	}

	[HttpPost("test-exception")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public ActionResult TestException(string? message)
	{
		throw new Exception("Test exception:" + message);
	}

	[HttpPost("log-error")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public ActionResult LogError(string? message)
	{
		_logger.LogError("Test log error: {message}", message);
		return Ok();
	}

	[HttpPost("log-warning")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public ActionResult LogWarning(string? message)
	{
		_logger.LogWarning("Test log warning: {message}", message);
		return Ok();
	}

	[HttpPost("log-info")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public ActionResult LogInfo(string? message)
	{
		_logger.LogInformation("Test log info: {message}", message);
		return Ok();
	}
}
