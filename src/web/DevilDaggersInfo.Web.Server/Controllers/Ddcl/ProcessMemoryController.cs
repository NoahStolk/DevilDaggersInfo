using DevilDaggersInfo.Api.Ddcl.ProcessMemory;

namespace DevilDaggersInfo.Web.Server.Controllers.Ddcl;

[Route("api/ddcl/process-memory")]
[ApiController]
public class ProcessMemoryController : ControllerBase
{
	private readonly ApplicationDbContext _dbContext;

	public ProcessMemoryController(ApplicationDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	// FORBIDDEN: Used by ddstats-rust.
	[Obsolete("Use the new route instead.")]
	[HttpGet("/api/process-memory/marker")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public ActionResult<Marker> GetMarkerObsolete(SupportedOperatingSystem operatingSystem)
	{
		return GetMarkerRepo(operatingSystem);
	}

	[HttpGet("marker")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public ActionResult<Marker> GetMarker(SupportedOperatingSystem operatingSystem)
	{
		return GetMarkerRepo(operatingSystem);
	}

	private ActionResult<Marker> GetMarkerRepo(SupportedOperatingSystem operatingSystem)
	{
		string name = operatingSystem switch
		{
			SupportedOperatingSystem.Windows => "WindowsSteam",
			SupportedOperatingSystem.Linux => "LinuxSteam",
			_ => throw new UnsupportedOperatingSystemException($"Operating system '{operatingSystem}' is not supported."),
		};

		MarkerEntity? marker = _dbContext.Markers.FirstOrDefault(m => m.Name == name);
		if (marker == null)
			throw new($"Marker key '{name}' was not found in database.");

		return new Marker { Value = marker.Value };
	}
}
