using DevilDaggersInfo.Web.Server.HostedServices.DdInfoDiscordBot;
using DSharpPlus.Entities;

namespace DevilDaggersInfo.Web.Server.HostedServices;

public class DatabaseLoggerBackgroundService : AbstractBackgroundService
{
	private readonly IServiceScopeFactory _serviceScopeFactory;

	public DatabaseLoggerBackgroundService(BackgroundServiceMonitor backgroundServiceMonitor, ILogger<DatabaseLoggerBackgroundService> logger, IServiceScopeFactory serviceScopeFactory)
		: base(backgroundServiceMonitor, logger)
	{
		_serviceScopeFactory = serviceScopeFactory;
	}

	protected override TimeSpan Interval => TimeSpan.FromHours(1);

	protected override async Task ExecuteTaskAsync(CancellationToken stoppingToken)
	{
		if (DiscordServerConstants.DatabaseMessage == null)
			return;

		using IServiceScope scope = _serviceScopeFactory.CreateScope();
		using ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

		List<InformationSchemaTable> tables = await dbContext.InformationSchemaTables
			.FromSqlRaw($@"SELECT
	table_name AS `{nameof(InformationSchemaTable.Table)}`,
	data_length `{nameof(InformationSchemaTable.DataSize)}`,
	index_length `{nameof(InformationSchemaTable.IndexSize)}`,
	avg_row_length `{nameof(InformationSchemaTable.AverageRowLength)}`,
	table_rows `{nameof(InformationSchemaTable.TableRows)}`
FROM information_schema.TABLES
WHERE table_schema = 'devildaggers'
ORDER BY table_name ASC;")
			.ToListAsync(stoppingToken);

		DiscordEmbedBuilder builder = new()
		{
			Title = $"Database {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}",
			Color = DiscordColor.White,
		};
		foreach (InformationSchemaTable table in tables)
		{
			string value = $@"`{"DataSize",-10}{table.DataSize,7}`
`{"IxSize",-10}{table.IndexSize,7}`
`{"AvgRL",-10}{table.AverageRowLength,7}`
`{"Count",-10}{table.TableRows,7}`";
			builder.AddFieldObject(table.Table ?? "Null", value, true);
		}

		await DiscordServerConstants.DatabaseMessage.TryEdit(builder.Build());
	}
}
