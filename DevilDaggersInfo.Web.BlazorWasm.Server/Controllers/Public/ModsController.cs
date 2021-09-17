﻿using DevilDaggersInfo.Web.BlazorWasm.Server.Caches.ModArchives;
using DevilDaggersInfo.Web.BlazorWasm.Shared.Dto.Public.Mods;

namespace DevilDaggersInfo.Web.BlazorWasm.Server.Controllers.Public;

[Route("api/mods")]
[ApiController]
public class ModsController : ControllerBase
{
	private readonly ApplicationDbContext _dbContext;
	private readonly IFileSystemService _fileSystemService;
	private readonly ModArchiveCache _modArchiveCache;

	public ModsController(ApplicationDbContext dbContext, IFileSystemService fileSystemService, ModArchiveCache modArchiveCache)
	{
		_dbContext = dbContext;
		_fileSystemService = fileSystemService;
		_modArchiveCache = modArchiveCache;
	}

	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public List<GetModDdae> GetPublicModsForDdae(string? authorFilter = null, string? nameFilter = null, bool? isHostedFilter = null)
	{
		IEnumerable<ModEntity> modsQuery = _dbContext.Mods
			.AsNoTracking()
			.Include(am => am.PlayerMods)
				.ThenInclude(pam => pam.Player)
			.Where(am => !am.IsHidden);

		if (!string.IsNullOrWhiteSpace(authorFilter))
			modsQuery = modsQuery.Where(am => am.PlayerMods.Any(pam => pam.Player.PlayerName.Contains(authorFilter, StringComparison.InvariantCultureIgnoreCase)));
		if (!string.IsNullOrWhiteSpace(nameFilter))
			modsQuery = modsQuery.Where(am => am.Name.Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase));

		List<ModEntity> mods = modsQuery.ToList();

		Dictionary<ModEntity, (bool FileExists, string? Path)> modsWithFileInfo = mods.ToDictionary(am => am, am =>
		{
			string filePath = Path.Combine(_fileSystemService.GetPath(DataSubDirectory.Mods), $"{am.Name}.zip");
			bool fileExists = IoFile.Exists(filePath);
			return (fileExists, fileExists ? filePath : null);
		});

		if (isHostedFilter.HasValue)
			modsWithFileInfo = modsWithFileInfo.Where(kvp => kvp.Value.FileExists == isHostedFilter.Value).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

		return modsWithFileInfo
			.Select(amwfi =>
			{
				bool? containsProhibitedAssets = null;
				GetModArchiveDdae? modArchive = null;
				ModTypes modTypes;
				if (amwfi.Value.FileExists)
				{
					ModArchiveCacheData archiveData = _modArchiveCache.GetArchiveDataByFilePath(amwfi.Value.Path!);
					containsProhibitedAssets = archiveData.Binaries.Any(md => md.Chunks.Any(mad => mad.IsProhibited));
					modArchive = new()
					{
						FileSize = archiveData.FileSize,
						FileSizeExtracted = archiveData.FileSizeExtracted,
						Binaries = archiveData.Binaries.ConvertAll(b => new GetModBinaryDdae
						{
							Name = b.Name,
							Size = b.Size,
							ModBinaryType = b.ModBinaryType,
						}),
					};

					ModBinaryCacheData? ddBinary = archiveData.Binaries.Find(md => md.ModBinaryType == ModBinaryType.Dd);

					modTypes = ModTypes.None;
					if (archiveData.Binaries.Any(md => md.ModBinaryType == ModBinaryType.Audio))
						modTypes |= ModTypes.Audio;
					if (archiveData.Binaries.Any(md => md.ModBinaryType == ModBinaryType.Core) || ddBinary?.Chunks.Any(mad => mad.AssetType == AssetType.Shader) == true)
						modTypes |= ModTypes.Shader;
					if (ddBinary?.Chunks.Any(mad => mad.AssetType == AssetType.ModelBinding || mad.AssetType == AssetType.Model) == true)
						modTypes |= ModTypes.Model;
					if (ddBinary?.Chunks.Any(mad => mad.AssetType == AssetType.Texture) == true)
						modTypes |= ModTypes.Texture;
				}
				else
				{
					modTypes = amwfi.Key.ModTypes;
				}

				string modScreenshotsDirectory = Path.Combine(_fileSystemService.GetPath(DataSubDirectory.ModScreenshots), amwfi.Key.Name);
				List<string> screenshotFileNames;
				if (Directory.Exists(modScreenshotsDirectory))
					screenshotFileNames = Directory.GetFiles(modScreenshotsDirectory).Select(p => Path.GetFileName(p)).ToList();
				else
					screenshotFileNames = new();

				return new GetModDdae
				{
					Name = amwfi.Key.Name,
					HtmlDescription = amwfi.Key.HtmlDescription,
					TrailerUrl = amwfi.Key.TrailerUrl,
					Authors = amwfi.Key.PlayerMods.Select(pam => pam.Player.PlayerName).OrderBy(s => s).ToList(),
					LastUpdated = amwfi.Key.LastUpdated,
					ModTypes = modTypes,
					IsHosted = amwfi.Value.FileExists,
					ContainsProhibitedAssets = containsProhibitedAssets,
					ModArchive = modArchive,
					ScreenshotFileNames = screenshotFileNames,
				};
			})
			.ToList();
	}

	[HttpGet("total-data")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public ActionResult<GetTotalModData> GetTotalModData()
	{
		return new GetTotalModData
		{
			Count = _dbContext.Mods.AsNoTracking().Select(m => m.Id).Count(),
		};
	}

	[HttpGet("{modName}/file")]
	[ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public ActionResult GetModFile([Required] string modName)
	{
		if (!_dbContext.Mods.Any(m => m.Name == modName))
			return NotFound();

		string fileName = $"{modName}.zip";
		string path = Path.Combine(_fileSystemService.GetPath(DataSubDirectory.Mods), fileName);
		if (!IoFile.Exists(path))
			return BadRequest($"Mod file '{fileName}' does not exist.");

		return File(IoFile.ReadAllBytes(path), MediaTypeNames.Application.Zip, fileName);
	}
}
