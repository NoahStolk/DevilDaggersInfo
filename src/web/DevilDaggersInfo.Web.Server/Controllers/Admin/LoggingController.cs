using DevilDaggersInfo.Web.Core.Claims;
using Microsoft.AspNetCore.Authorization;

namespace DevilDaggersInfo.Web.Server.Controllers.Admin;

[Route("api/admin/logging")]
[ApiController]
[Authorize(Roles = Roles.Admin)]
public class LoggingController : ControllerBase
{
	[HttpPost]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public ActionResult TestException(string? message)
	{
		throw new Exception("Test exception:" + message);
	}
}
