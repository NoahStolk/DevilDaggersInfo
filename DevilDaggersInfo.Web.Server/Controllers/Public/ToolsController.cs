using DevilDaggersInfo.Web.Server.Converters.Public;
using DevilDaggersInfo.Web.Server.InternalModels;
using DevilDaggersInfo.Web.Shared.Dto.Public.Tools;

namespace DevilDaggersInfo.Web.Server.Controllers.Public;

[Route("api/tools")]
[ApiController]
public class ToolsController : ControllerBase
{
	private readonly IFileSystemService _fileSystemService;
	private readonly ApplicationDbContext _dbContext;
	private readonly IToolHelper _toolHelper;
	private readonly ILogger<ToolsController> _logger;

	public ToolsController(IFileSystemService fileSystemService, ApplicationDbContext dbContext, IToolHelper toolHelper, ILogger<ToolsController> logger)
	{
		_fileSystemService = fileSystemService;
		_dbContext = dbContext;
		_toolHelper = toolHelper;
		_logger = logger;
	}

	[HttpGet("{toolName}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public ActionResult<GetTool> GetTool([Required] string toolName)
	{
		Tool? tool = _toolHelper.GetToolByName(toolName);
		if (tool == null)
			return NotFound();

		List<ToolStatisticEntity> toolStatistics = _dbContext.ToolStatistics
			.AsNoTracking()
			.Where(ts => ts.ToolName == tool.Name)
			.ToList();

		string path = Path.Combine(_fileSystemService.GetPath(DataSubDirectory.Tools), tool.Name, $"{tool.Name}{tool.VersionNumber}.zip");
		int fileSize = 0;
		if (IoFile.Exists(path))
			fileSize = (int)new FileInfo(path).Length;
		else
			_logger.LogWarning("Tool file '{path}' does not exist.", path);

		return tool.ToGetTool(toolStatistics, fileSize);
	}

	[HttpGet("{toolName}/file")]
	[ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public ActionResult GetToolFile([Required] string toolName)
	{
		Tool? tool = _toolHelper.GetToolByName(toolName);
		if (tool == null)
			return NotFound();

		string path = Path.Combine(_fileSystemService.GetPath(DataSubDirectory.Tools), tool.Name, $"{tool.Name}{tool.VersionNumber}.zip");
		if (!IoFile.Exists(path))
			throw new($"Tool file '{path}' does not exist.");

		ToolStatisticEntity? toolStatistic = _dbContext.ToolStatistics.FirstOrDefault(ts => ts.ToolName == tool.Name && ts.VersionNumber == tool.VersionNumber.ToString());
		if (toolStatistic == null)
			_dbContext.ToolStatistics.Add(new ToolStatisticEntity { DownloadCount = 1, ToolName = tool.Name, VersionNumber = tool.VersionNumber.ToString() });
		else
			toolStatistic.DownloadCount++;

		_dbContext.SaveChanges();

		return File(IoFile.ReadAllBytes(path), MediaTypeNames.Application.Zip, $"{toolName}{tool.VersionNumber}.zip");
	}
}