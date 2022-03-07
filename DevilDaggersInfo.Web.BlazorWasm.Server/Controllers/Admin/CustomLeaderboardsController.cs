using DevilDaggersInfo.Web.BlazorWasm.Server.Converters.Admin;
using DevilDaggersInfo.Web.BlazorWasm.Shared.Dto;
using DevilDaggersInfo.Web.BlazorWasm.Shared.Dto.Admin.CustomLeaderboards;
using DevilDaggersInfo.Web.BlazorWasm.Shared.Enums.Sortings.Admin;
using Microsoft.AspNetCore.Authorization;

namespace DevilDaggersInfo.Web.BlazorWasm.Server.Controllers.Admin;

[Route("api/admin/custom-leaderboards")]
[ApiController]
[Authorize(Roles = Roles.CustomLeaderboards)]
public class CustomLeaderboardsController : ControllerBase
{
	private readonly ApplicationDbContext _dbContext;
	private readonly IFileSystemService _fileSystemService;
	private readonly AuditLogger _auditLogger;

	public CustomLeaderboardsController(ApplicationDbContext dbContext, IFileSystemService fileSystemService, AuditLogger auditLogger)
	{
		_dbContext = dbContext;
		_fileSystemService = fileSystemService;
		_auditLogger = auditLogger;
	}

	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public ActionResult<Page<GetCustomLeaderboardForOverview>> GetCustomLeaderboards(
		[Range(0, 1000)] int pageIndex = 0,
		[Range(PagingConstants.PageSizeMin, PagingConstants.PageSizeMax)] int pageSize = PagingConstants.PageSizeDefault,
		CustomLeaderboardSorting? sortBy = null,
		bool ascending = false)
	{
		IQueryable<CustomLeaderboardEntity> customLeaderboardsQuery = _dbContext.CustomLeaderboards
			.AsNoTracking()
			.Include(cl => cl.Spawnset);

		customLeaderboardsQuery = sortBy switch
		{
			CustomLeaderboardSorting.Category => customLeaderboardsQuery.OrderBy(cl => cl.Category, ascending).ThenBy(cl => cl.Id),
			CustomLeaderboardSorting.DateCreated => customLeaderboardsQuery.OrderBy(cl => cl.DateCreated, ascending).ThenBy(cl => cl.Id),
			CustomLeaderboardSorting.IsArchived => customLeaderboardsQuery.OrderBy(cl => cl.IsArchived, ascending).ThenBy(cl => cl.Id),
			CustomLeaderboardSorting.SpawnsetName => customLeaderboardsQuery.OrderBy(cl => cl.Spawnset.Name, ascending).ThenBy(cl => cl.Id),
			CustomLeaderboardSorting.TimeBronze => customLeaderboardsQuery.OrderBy(cl => cl.TimeBronze, ascending).ThenBy(cl => cl.Id),
			CustomLeaderboardSorting.TimeSilver => customLeaderboardsQuery.OrderBy(cl => cl.TimeSilver, ascending).ThenBy(cl => cl.Id),
			CustomLeaderboardSorting.TimeGolden => customLeaderboardsQuery.OrderBy(cl => cl.TimeGolden, ascending).ThenBy(cl => cl.Id),
			CustomLeaderboardSorting.TimeDevil => customLeaderboardsQuery.OrderBy(cl => cl.TimeDevil, ascending).ThenBy(cl => cl.Id),
			CustomLeaderboardSorting.TimeLeviathan => customLeaderboardsQuery.OrderBy(cl => cl.TimeLeviathan, ascending).ThenBy(cl => cl.Id),
			_ => customLeaderboardsQuery.OrderBy(cl => cl.Id, ascending),
		};

		List<CustomLeaderboardEntity> customLeaderboards = customLeaderboardsQuery
			.Skip(pageIndex * pageSize)
			.Take(pageSize)
			.ToList();

		return new Page<GetCustomLeaderboardForOverview>
		{
			Results = customLeaderboards.ConvertAll(cl => cl.ToGetCustomLeaderboardForOverview()),
			TotalResults = _dbContext.CustomLeaderboards.Count(),
		};
	}

	[HttpGet("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public ActionResult<GetCustomLeaderboard> GetCustomLeaderboardById(int id)
	{
		CustomLeaderboardEntity? customLeaderboard = _dbContext.CustomLeaderboards
			.AsNoTracking()
			.Where(cl => !cl.IsArchived)
			.Include(cl => cl.Spawnset)
				.ThenInclude(sf => sf.Player)
			.FirstOrDefault(cl => cl.Id == id);

		if (customLeaderboard == null)
			return NotFound($"Leaderboard with ID '{id}' was not found.");

		return customLeaderboard.ToGetCustomLeaderboard();
	}

	[HttpPost]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult> AddCustomLeaderboard(AddCustomLeaderboard addCustomLeaderboard)
	{
		if (_dbContext.CustomLeaderboards.Any(cl => cl.SpawnsetId == addCustomLeaderboard.SpawnsetId))
			return BadRequest("A leaderboard for this spawnset already exists.");

		if (addCustomLeaderboard.Category.IsAscending())
		{
			if (addCustomLeaderboard.TimeLeviathan >= addCustomLeaderboard.TimeDevil)
				return BadRequest("For ascending leaderboards, Leviathan time must be smaller than Devil time.");
			if (addCustomLeaderboard.TimeDevil >= addCustomLeaderboard.TimeGolden)
				return BadRequest("For ascending leaderboards, Devil time must be smaller than Golden time.");
			if (addCustomLeaderboard.TimeGolden >= addCustomLeaderboard.TimeSilver)
				return BadRequest("For ascending leaderboards, Golden time must be smaller than Silver time.");
			if (addCustomLeaderboard.TimeSilver >= addCustomLeaderboard.TimeBronze)
				return BadRequest("For ascending leaderboards, Silver time must be smaller than Bronze time.");
		}
		else
		{
			if (addCustomLeaderboard.TimeLeviathan <= addCustomLeaderboard.TimeDevil)
				return BadRequest("For descending leaderboards, Leviathan time must be greater than Devil time.");
			if (addCustomLeaderboard.TimeDevil <= addCustomLeaderboard.TimeGolden)
				return BadRequest("For descending leaderboards, Devil time must be greater than Golden time.");
			if (addCustomLeaderboard.TimeGolden <= addCustomLeaderboard.TimeSilver)
				return BadRequest("For descending leaderboards, Golden time must be greater than Silver time.");
			if (addCustomLeaderboard.TimeSilver <= addCustomLeaderboard.TimeBronze)
				return BadRequest("For descending leaderboards, Silver time must be greater than Bronze time.");
		}

		var spawnset = _dbContext.Spawnsets
			.AsNoTracking()
			.Select(sf => new { sf.Id, sf.Name })
			.FirstOrDefault(sf => sf.Id == addCustomLeaderboard.SpawnsetId);
		if (spawnset == null)
			return BadRequest($"Spawnset with ID '{addCustomLeaderboard.SpawnsetId}' does not exist.");

		if (!SpawnsetBinary.TryParse(System.IO.File.ReadAllBytes(Path.Combine(_fileSystemService.GetPath(DataSubDirectory.Spawnsets), spawnset.Name)), out SpawnsetBinary? spawnsetBinary))
			throw new($"Could not parse survival file '{spawnset.Name}'. Please review the file. Also review how this file ended up in the 'spawnsets' directory, as it is not possible to upload non-survival files from within the Admin pages.");

		GameMode requiredGameMode = addCustomLeaderboard.Category.GetRequiredGameModeForCategory();
		if (spawnsetBinary.GameMode != requiredGameMode)
			return BadRequest($"Game mode must be '{requiredGameMode}' when the custom leaderboard category is '{addCustomLeaderboard.Category}'. The spawnset has game mode '{spawnsetBinary.GameMode}'.");

		if (spawnsetBinary.TimerStart != 0)
			return BadRequest("Cannot create a leaderboard for spawnset that uses the TimerStart value. This value is meant for practice and it is confusing to use it with custom leaderboards, as custom leaderboards always use the 'actual' timer value.");

		if (addCustomLeaderboard.Category == CustomLeaderboardCategory.Default && !spawnsetBinary.HasEndLoop())
			return BadRequest($"Custom leaderboard with category {CustomLeaderboardCategory.Default} must have an end loop.");

		CustomLeaderboardEntity customLeaderboard = new()
		{
			DateCreated = DateTime.UtcNow,
			SpawnsetId = addCustomLeaderboard.SpawnsetId,
			Category = addCustomLeaderboard.Category,
			TimeBronze = addCustomLeaderboard.TimeBronze.To10thMilliTime(),
			TimeSilver = addCustomLeaderboard.TimeSilver.To10thMilliTime(),
			TimeGolden = addCustomLeaderboard.TimeGolden.To10thMilliTime(),
			TimeDevil = addCustomLeaderboard.TimeDevil.To10thMilliTime(),
			TimeLeviathan = addCustomLeaderboard.TimeLeviathan.To10thMilliTime(),
		};
		_dbContext.CustomLeaderboards.Add(customLeaderboard);
		_dbContext.SaveChanges();

		await _auditLogger.LogAdd(addCustomLeaderboard.GetLog(), User, customLeaderboard.Id);

		return Ok(customLeaderboard.Id);
	}

	[HttpPut("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult> EditCustomLeaderboardById(int id, EditCustomLeaderboard editCustomLeaderboard)
	{
		if (editCustomLeaderboard.Category.IsAscending())
		{
			if (editCustomLeaderboard.TimeLeviathan >= editCustomLeaderboard.TimeDevil)
				return BadRequest("For ascending leaderboards, Leviathan time must be smaller than Devil time.");
			if (editCustomLeaderboard.TimeDevil >= editCustomLeaderboard.TimeGolden)
				return BadRequest("For ascending leaderboards, Devil time must be smaller than Golden time.");
			if (editCustomLeaderboard.TimeGolden >= editCustomLeaderboard.TimeSilver)
				return BadRequest("For ascending leaderboards, Golden time must be smaller than Silver time.");
			if (editCustomLeaderboard.TimeSilver >= editCustomLeaderboard.TimeBronze)
				return BadRequest("For ascending leaderboards, Silver time must be smaller than Bronze time.");
		}
		else
		{
			if (editCustomLeaderboard.TimeLeviathan <= editCustomLeaderboard.TimeDevil)
				return BadRequest("For descending leaderboards, Leviathan time must be greater than Devil time.");
			if (editCustomLeaderboard.TimeDevil <= editCustomLeaderboard.TimeGolden)
				return BadRequest("For descending leaderboards, Devil time must be greater than Golden time.");
			if (editCustomLeaderboard.TimeGolden <= editCustomLeaderboard.TimeSilver)
				return BadRequest("For descending leaderboards, Golden time must be greater than Silver time.");
			if (editCustomLeaderboard.TimeSilver <= editCustomLeaderboard.TimeBronze)
				return BadRequest("For descending leaderboards, Silver time must be greater than Bronze time.");
		}

		CustomLeaderboardEntity? customLeaderboard = _dbContext.CustomLeaderboards.FirstOrDefault(cl => cl.Id == id);
		if (customLeaderboard == null)
			return NotFound();

		var spawnset = _dbContext.Spawnsets
			.AsNoTracking()
			.Select(sf => new { sf.Id, sf.Name })
			.FirstOrDefault(sf => sf.Id == customLeaderboard.SpawnsetId);
		if (spawnset == null)
			return BadRequest($"Spawnset with ID '{customLeaderboard.SpawnsetId}' does not exist.");

		if (!SpawnsetBinary.TryParse(System.IO.File.ReadAllBytes(Path.Combine(_fileSystemService.GetPath(DataSubDirectory.Spawnsets), spawnset.Name)), out SpawnsetBinary? spawnsetBinary))
			throw new($"Could not parse survival file '{spawnset.Name}'. Please review the file. Also review how this file ended up in the 'spawnsets' directory, as it is not possible to upload non-survival files from within the Admin pages.");

		if (editCustomLeaderboard.Category == CustomLeaderboardCategory.TimeAttack && spawnsetBinary.GameMode != GameMode.TimeAttack
		 || editCustomLeaderboard.Category != CustomLeaderboardCategory.TimeAttack && spawnsetBinary.GameMode == GameMode.TimeAttack)
		{
			return BadRequest($"Spawnset game mode is '{spawnsetBinary.GameMode}' while custom leaderboard category is '{editCustomLeaderboard.Category}'.");
		}

		if (spawnsetBinary.TimerStart != 0)
			return BadRequest("Cannot create a leaderboard for spawnset that uses the TimerStart value. This value is meant for practice and it is confusing to use it with custom leaderboards, as custom leaderboards always use the 'actual' timer value.");

		EditCustomLeaderboard logDto = new()
		{
			Category = customLeaderboard.Category,
			TimeBronze = customLeaderboard.TimeBronze.ToSecondsTime(),
			TimeSilver = customLeaderboard.TimeSilver.ToSecondsTime(),
			TimeGolden = customLeaderboard.TimeGolden.ToSecondsTime(),
			TimeDevil = customLeaderboard.TimeDevil.ToSecondsTime(),
			TimeLeviathan = customLeaderboard.TimeLeviathan.ToSecondsTime(),
			IsArchived = customLeaderboard.IsArchived,
		};

		customLeaderboard.Category = editCustomLeaderboard.Category;
		customLeaderboard.TimeBronze = editCustomLeaderboard.TimeBronze.To10thMilliTime();
		customLeaderboard.TimeSilver = editCustomLeaderboard.TimeSilver.To10thMilliTime();
		customLeaderboard.TimeGolden = editCustomLeaderboard.TimeGolden.To10thMilliTime();
		customLeaderboard.TimeDevil = editCustomLeaderboard.TimeDevil.To10thMilliTime();
		customLeaderboard.TimeLeviathan = editCustomLeaderboard.TimeLeviathan.To10thMilliTime();
		customLeaderboard.IsArchived = editCustomLeaderboard.IsArchived;
		_dbContext.SaveChanges();

		await _auditLogger.LogEdit(logDto.GetLog(), editCustomLeaderboard.GetLog(), User, customLeaderboard.Id);

		return Ok();
	}

	[HttpDelete("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult> DeleteCustomLeaderboardById(int id)
	{
		CustomLeaderboardEntity? customLeaderboard = _dbContext.CustomLeaderboards.FirstOrDefault(cl => cl.Id == id);
		if (customLeaderboard == null)
			return NotFound();

		if (_dbContext.CustomEntries.Any(ce => ce.CustomLeaderboardId == id))
			return BadRequest("Custom leaderboard with scores cannot be deleted.");

		_dbContext.CustomLeaderboards.Remove(customLeaderboard);
		_dbContext.SaveChanges();

		await _auditLogger.LogDelete(customLeaderboard.GetLog(), User, customLeaderboard.Id);

		return Ok();
	}
}
