using DevilDaggersInfo.Web.BlazorWasm.Server.Converters.Admin;
using DevilDaggersInfo.Web.BlazorWasm.Shared.Dto;
using DevilDaggersInfo.Web.BlazorWasm.Shared.Dto.Admin.Players;
using DevilDaggersInfo.Web.BlazorWasm.Shared.Enums.Sortings.Admin;
using Microsoft.AspNetCore.Authorization;

namespace DevilDaggersInfo.Web.BlazorWasm.Server.Controllers.Admin;

[Route("api/admin/players")]
[ApiController]
public class PlayersController : ControllerBase
{
	private readonly ApplicationDbContext _dbContext;
	private readonly AuditLogger _auditLogger;

	public PlayersController(ApplicationDbContext dbContext, AuditLogger auditLogger)
	{
		_dbContext = dbContext;
		_auditLogger = auditLogger;
	}

	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[Authorize(Roles = Roles.Players)]
	public ActionResult<Page<GetPlayerForOverview>> GetPlayers(
		[Range(0, 1000)] int pageIndex = 0,
		[Range(PagingConstants.PageSizeMin, PagingConstants.PageSizeMax)] int pageSize = PagingConstants.PageSizeDefault,
		PlayerSorting? sortBy = null,
		bool ascending = false)
	{
		IQueryable<PlayerEntity> playersQuery = _dbContext.Players.AsNoTracking();

		playersQuery = sortBy switch
		{
			PlayerSorting.BanDescription => playersQuery.OrderBy(p => p.BanDescription, ascending).ThenBy(p => p.Id),
			PlayerSorting.BanResponsibleId => playersQuery.OrderBy(p => p.BanResponsibleId, ascending).ThenBy(p => p.Id),
			PlayerSorting.BanType => playersQuery.OrderBy(p => p.BanType, ascending).ThenBy(p => p.Id),
			PlayerSorting.CommonName => playersQuery.OrderBy(p => p.CommonName, ascending).ThenBy(p => p.Id),
			PlayerSorting.CountryCode => playersQuery.OrderBy(p => p.CountryCode, ascending).ThenBy(p => p.Id),
			PlayerSorting.DiscordUserId => playersQuery.OrderBy(p => p.DiscordUserId, ascending).ThenBy(p => p.Id),
			PlayerSorting.Dpi => playersQuery.OrderBy(p => p.Dpi, ascending).ThenBy(p => p.Id),
			PlayerSorting.Fov => playersQuery.OrderBy(p => p.Fov, ascending).ThenBy(p => p.Id),
			PlayerSorting.Gamma => playersQuery.OrderBy(p => p.Gamma, ascending).ThenBy(p => p.Id),
			PlayerSorting.HasFlashHandEnabled => playersQuery.OrderBy(p => p.HasFlashHandEnabled, ascending).ThenBy(p => p.Id),
			PlayerSorting.HideDonations => playersQuery.OrderBy(p => p.HideDonations, ascending).ThenBy(p => p.Id),
			PlayerSorting.HidePastUsernames => playersQuery.OrderBy(p => p.HidePastUsernames, ascending).ThenBy(p => p.Id),
			PlayerSorting.HideSettings => playersQuery.OrderBy(p => p.HideSettings, ascending).ThenBy(p => p.Id),
			PlayerSorting.InGameSens => playersQuery.OrderBy(p => p.InGameSens, ascending).ThenBy(p => p.Id),
			PlayerSorting.IsBannedFromDdcl => playersQuery.OrderBy(p => p.IsBannedFromDdcl, ascending).ThenBy(p => p.Id),
			PlayerSorting.IsRightHanded => playersQuery.OrderBy(p => p.IsRightHanded, ascending).ThenBy(p => p.Id),
			PlayerSorting.PlayerName => playersQuery.OrderBy(p => p.PlayerName, ascending).ThenBy(p => p.Id),
			PlayerSorting.UsesLegacyAudio => playersQuery.OrderBy(p => p.UsesLegacyAudio, ascending).ThenBy(p => p.Id),
			PlayerSorting.UsesHrtf => playersQuery.OrderBy(p => p.UsesHrtf, ascending).ThenBy(p => p.Id),
			PlayerSorting.UsesInvertY => playersQuery.OrderBy(p => p.UsesInvertY, ascending).ThenBy(p => p.Id),
			PlayerSorting.VerticalSync => playersQuery.OrderBy(p => p.VerticalSync, ascending).ThenBy(p => p.Id),
			_ => playersQuery.OrderBy(p => p.Id, ascending),
		};

		List<PlayerEntity> players = playersQuery
			.Skip(pageIndex * pageSize)
			.Take(pageSize)
			.ToList();

		return new Page<GetPlayerForOverview>
		{
			Results = players.ConvertAll(p => p.ToGetPlayerForOverview()),
			TotalResults = _dbContext.Players.Count(),
		};
	}

	[HttpGet("names")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[Authorize(Roles = $"{Roles.Mods},{Roles.Spawnsets}")]
	public ActionResult<List<GetPlayerName>> GetPlayerNames()
	{
		var players = _dbContext.Players
			.AsNoTracking()
			.Select(p => new { p.Id, p.PlayerName })
			.ToList();

		return players.ConvertAll(p => new GetPlayerName
		{
			Id = p.Id,
			PlayerName = p.PlayerName,
		});
	}

	[HttpGet("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[Authorize(Roles = Roles.Players)]
	public ActionResult<GetPlayer> GetPlayerById(int id)
	{
		PlayerEntity? player = _dbContext.Players
			.AsSingleQuery()
			.AsNoTracking()
			.Include(p => p.PlayerMods)
			.FirstOrDefault(p => p.Id == id);
		if (player == null)
			return NotFound();

		return player.ToGetPlayer();
	}

	[HttpPost]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status409Conflict)]
	[Authorize(Roles = Roles.Players)]
	public async Task<ActionResult> AddPlayer(AddPlayer addPlayer)
	{
		if (addPlayer.BanType != BanType.NotBanned)
		{
			if (!string.IsNullOrWhiteSpace(addPlayer.CountryCode))
				return BadRequest("Banned players must not have a country code.");

			if (addPlayer.Dpi.HasValue ||
				addPlayer.InGameSens.HasValue ||
				addPlayer.Fov.HasValue ||
				addPlayer.IsRightHanded.HasValue ||
				addPlayer.HasFlashHandEnabled.HasValue ||
				addPlayer.Gamma.HasValue ||
				addPlayer.UsesLegacyAudio.HasValue ||
				addPlayer.UsesHrtf.HasValue ||
				addPlayer.UsesInvertY.HasValue ||
				addPlayer.VerticalSync != VerticalSync.Unknown)
			{
				return BadRequest("Banned players must not have settings.");
			}
		}
		else
		{
			if (!string.IsNullOrWhiteSpace(addPlayer.BanDescription))
				return BadRequest("BanDescription must only be used for banned players.");

			if (addPlayer.BanResponsibleId.HasValue)
				return BadRequest("BanResponsibleId must only be used for banned players.");
		}

		if (_dbContext.Players.Any(p => p.Id == addPlayer.Id))
			return Conflict($"Player with ID '{addPlayer.Id}' already exists.");

		foreach (int modId in addPlayer.ModIds ?? new())
		{
			if (!_dbContext.Mods.Any(m => m.Id == modId))
				return BadRequest($"Mod with ID '{modId}' does not exist.");
		}

		PlayerEntity player = new()
		{
			Id = addPlayer.Id,
			PlayerName = await GetPlayerName(addPlayer.Id),
			CommonName = addPlayer.CommonName,
			DiscordUserId = addPlayer.DiscordUserId,
			CountryCode = addPlayer.CountryCode,
			Dpi = addPlayer.Dpi,
			InGameSens = addPlayer.InGameSens,
			Fov = addPlayer.Fov,
			IsRightHanded = addPlayer.IsRightHanded,
			HasFlashHandEnabled = addPlayer.HasFlashHandEnabled,
			Gamma = addPlayer.Gamma,
			UsesLegacyAudio = addPlayer.UsesLegacyAudio,
			UsesHrtf = addPlayer.UsesHrtf,
			UsesInvertY = addPlayer.UsesInvertY,
			VerticalSync = addPlayer.VerticalSync,
			BanType = addPlayer.BanType,
			BanDescription = addPlayer.BanDescription,
			BanResponsibleId = addPlayer.BanResponsibleId,
			IsBannedFromDdcl = addPlayer.IsBannedFromDdcl,
			HideSettings = addPlayer.HideSettings,
			HideDonations = addPlayer.HideDonations,
			HidePastUsernames = addPlayer.HidePastUsernames,
		};
		_dbContext.Players.Add(player);
		_dbContext.SaveChanges(); // Save changes here so PlayerMod entities can be assigned properly.

		UpdatePlayerMods(addPlayer.ModIds ?? new(), player.Id);
		_dbContext.SaveChanges();

		await _auditLogger.LogAdd(addPlayer.GetLog(), User, player.Id);

		return Ok(player.Id);
	}

	[HttpPut("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[Authorize(Roles = Roles.Players)]
	public async Task<ActionResult> EditPlayerById(int id, EditPlayer editPlayer)
	{
		if (editPlayer.BanType != BanType.NotBanned)
		{
			if (!string.IsNullOrWhiteSpace(editPlayer.CountryCode))
				return BadRequest("Banned players must not have a country code.");

			if (editPlayer.Dpi.HasValue ||
				editPlayer.InGameSens.HasValue ||
				editPlayer.Fov.HasValue ||
				editPlayer.IsRightHanded.HasValue ||
				editPlayer.HasFlashHandEnabled.HasValue ||
				editPlayer.Gamma.HasValue ||
				editPlayer.UsesLegacyAudio.HasValue ||
				editPlayer.UsesHrtf.HasValue ||
				editPlayer.UsesInvertY.HasValue ||
				editPlayer.VerticalSync != VerticalSync.Unknown)
			{
				return BadRequest("Banned players must not have settings.");
			}
		}
		else
		{
			if (!string.IsNullOrWhiteSpace(editPlayer.BanDescription))
				return BadRequest("BanDescription must only be used for banned players.");

			if (editPlayer.BanResponsibleId.HasValue)
				return BadRequest("BanResponsibleId must only be used for banned players.");
		}

		foreach (int modId in editPlayer.ModIds ?? new())
		{
			if (!_dbContext.Mods.Any(m => m.Id == modId))
				return BadRequest($"Mod with ID '{modId}' does not exist.");
		}

		PlayerEntity? player = _dbContext.Players
			.AsSingleQuery()
			.Include(p => p.PlayerMods)
			.FirstOrDefault(p => p.Id == id);
		if (player == null)
			return NotFound();

		EditPlayer oldDtoLog = new()
		{
			CommonName = player.CommonName,
			DiscordUserId = player.DiscordUserId,
			CountryCode = player.CountryCode,
			Dpi = player.Dpi,
			InGameSens = player.InGameSens,
			Fov = player.Fov,
			IsRightHanded = player.IsRightHanded,
			HasFlashHandEnabled = player.HasFlashHandEnabled,
			Gamma = player.Gamma,
			UsesLegacyAudio = player.UsesLegacyAudio,
			UsesHrtf = player.UsesHrtf,
			UsesInvertY = player.UsesInvertY,
			VerticalSync = player.VerticalSync,
			HideSettings = player.HideSettings,
			HideDonations = player.HideDonations,
			HidePastUsernames = player.HidePastUsernames,
			ModIds = player.PlayerMods.ConvertAll(pam => pam.ModId),
			BanDescription = player.BanDescription,
			BanResponsibleId = player.BanResponsibleId,
			BanType = player.BanType,
			IsBannedFromDdcl = player.IsBannedFromDdcl,
		};

		player.PlayerName = await GetPlayerName(id);
		player.CommonName = editPlayer.CommonName;
		player.DiscordUserId = editPlayer.DiscordUserId;
		player.CountryCode = editPlayer.CountryCode;
		player.Dpi = editPlayer.Dpi;
		player.InGameSens = editPlayer.InGameSens;
		player.Fov = editPlayer.Fov;
		player.IsRightHanded = editPlayer.IsRightHanded;
		player.HasFlashHandEnabled = editPlayer.HasFlashHandEnabled;
		player.Gamma = editPlayer.Gamma;
		player.UsesLegacyAudio = editPlayer.UsesLegacyAudio;
		player.UsesHrtf = editPlayer.UsesHrtf;
		player.UsesInvertY = editPlayer.UsesInvertY;
		player.VerticalSync = editPlayer.VerticalSync;
		player.HideSettings = editPlayer.HideSettings;
		player.HideDonations = editPlayer.HideDonations;
		player.HidePastUsernames = editPlayer.HidePastUsernames;
		player.BanDescription = editPlayer.BanDescription;
		player.BanResponsibleId = editPlayer.BanResponsibleId;
		player.BanType = editPlayer.BanType;
		player.IsBannedFromDdcl = editPlayer.IsBannedFromDdcl;

		UpdatePlayerMods(editPlayer.ModIds ?? new(), player.Id);
		_dbContext.SaveChanges();

		await _auditLogger.LogEdit(oldDtoLog.GetLog(), editPlayer.GetLog(), User, player.Id);

		return Ok();
	}

	[HttpDelete("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[Authorize(Roles = Roles.Players)]
	public async Task<ActionResult> DeletePlayerById(int id)
	{
		PlayerEntity? player = _dbContext.Players.FirstOrDefault(p => p.Id == id);
		if (player == null)
			return NotFound();

		if (_dbContext.CustomEntries.Any(ce => ce.PlayerId == id))
			return BadRequest("Player with custom leaderboard scores cannot be deleted.");

		if (_dbContext.Donations.Any(d => d.PlayerId == id))
			return BadRequest("Player with donations cannot be deleted.");

		if (_dbContext.PlayerMods.Any(pam => pam.PlayerId == id))
			return BadRequest("Player with mods cannot be deleted.");

		if (_dbContext.Spawnsets.Any(sf => sf.PlayerId == id))
			return BadRequest("Player with spawnsets cannot be deleted.");

		_dbContext.Players.Remove(player);
		_dbContext.SaveChanges();

		await _auditLogger.LogDelete(player.GetLog(), User, player.Id);

		return Ok();
	}

	private static async Task<string> GetPlayerName(int id)
	{
		try
		{
			return (await LeaderboardClient.Instance.GetEntryById(id)).Username;
		}
		catch
		{
			return string.Empty;
		}
	}

	private void UpdatePlayerMods(List<int> modIds, int playerId)
	{
		foreach (PlayerModEntity newEntity in modIds.ConvertAll(ami => new PlayerModEntity { ModId = ami, PlayerId = playerId }))
		{
			if (!_dbContext.PlayerMods.Any(pam => pam.ModId == newEntity.ModId && pam.PlayerId == newEntity.PlayerId))
				_dbContext.PlayerMods.Add(newEntity);
		}

		foreach (PlayerModEntity entityToRemove in _dbContext.PlayerMods.Where(pam => pam.PlayerId == playerId && !modIds.Contains(pam.ModId)))
			_dbContext.PlayerMods.Remove(entityToRemove);
	}
}
