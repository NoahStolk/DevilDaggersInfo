using DevilDaggersInfo.Web.BlazorWasm.Server.Converters.Public;
using DevilDaggersInfo.Web.BlazorWasm.Shared.Dto.Public.Leaderboards;

namespace DevilDaggersInfo.Web.BlazorWasm.Server.Controllers.Public;

[Route("api/leaderboards")]
[ApiController]
public class LeaderboardsController : ControllerBase
{
	private readonly LeaderboardClient _leaderboardClient;

	public LeaderboardsController(LeaderboardClient leaderboardClient)
	{
		_leaderboardClient = leaderboardClient;
	}

	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<ActionResult<GetLeaderboard?>> GetLeaderboard([Range(1, int.MaxValue)] int rankStart = 1)
	{
		LeaderboardResponse l = await _leaderboardClient.GetLeaderboard(rankStart);
		return l.ToGetLeaderboardPublic();
	}

	[HttpGet("entry/by-id")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<GetEntry>> GetEntryById([Required, Range(1, int.MaxValue)] int id)
	{
		EntryResponse e = await _leaderboardClient.GetEntryById(id);
		return e.ToGetEntryPublic();
	}

	[HttpGet("entry/by-ids")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<List<GetEntry>>> GetEntriesByIds(string commaSeparatedIds)
	{
		IEnumerable<int> ids = commaSeparatedIds.Split(',').Where(s => int.TryParse(s, out _)).Select(int.Parse);

		List<EntryResponse> el = await _leaderboardClient.GetEntriesByIds(ids);
		return el.ConvertAll(e => e.ToGetEntryPublic());
	}

	[HttpGet("entry/by-username")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<List<GetEntry>>> GetEntriesByName([Required, MinLength(3), MaxLength(16)] string name)
	{
		List<EntryResponse> el = await _leaderboardClient.GetEntriesByName(name);
		return el.ConvertAll(e => e.ToGetEntryPublic());
	}

	[HttpGet("entry/by-rank")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<GetEntry>> GetEntryByRank([Required, Range(1, int.MaxValue)] int rank)
	{
		// TODO: Implement separate method that only parses the first entry.
		LeaderboardResponse l = await _leaderboardClient.GetLeaderboard(rank);
		if (l.Entries.Count == 0)
			return NotFound();

		return l.Entries[0].ToGetEntryPublic();
	}
}
