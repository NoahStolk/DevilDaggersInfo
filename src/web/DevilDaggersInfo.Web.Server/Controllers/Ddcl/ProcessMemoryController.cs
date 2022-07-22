using DevilDaggersInfo.Api.Ddcl.ProcessMemory;
using DevilDaggersInfo.Web.Server.Domain.Repositories;

namespace DevilDaggersInfo.Web.Server.Controllers.Ddcl;

[Route("api/ddcl/process-memory")]
[ApiController]
public class ProcessMemoryController : ControllerBase
{
	private readonly MarkerRepository _markerRepository;

	public ProcessMemoryController(MarkerRepository markerRepository)
	{
		_markerRepository = markerRepository;
	}

	// FORBIDDEN: Used by ddstats-rust.
	[Obsolete("Use the new route instead.")]
	[HttpGet("/api/process-memory/marker")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<Marker>> GetMarkerObsolete([Required] SupportedOperatingSystem operatingSystem)
		=> await GetMarkerRepo(operatingSystem);

	[HttpGet("marker")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<Marker>> GetMarker([Required] SupportedOperatingSystem operatingSystem)
		=> await GetMarkerRepo(operatingSystem);

	private async Task<Marker> GetMarkerRepo(SupportedOperatingSystem operatingSystem) => new Marker
	{
		Value = await _markerRepository.GetMarkerAsync(operatingSystem switch
		{
			SupportedOperatingSystem.Windows => "WindowsSteam",
			SupportedOperatingSystem.Linux => "LinuxSteam",
			_ => throw new UnsupportedOperatingSystemException($"Operating system '{operatingSystem}' is not supported."),
		}),
	};
}
