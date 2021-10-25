using DevilDaggersInfo.Web.BlazorWasm.Server.HostedServices.DdInfoDiscordBot;
using DSharpPlus.Entities;

namespace DevilDaggersInfo.Web.BlazorWasm.Server.HostedServices;

public class DiscordLogFlushBackgroundService : AbstractBackgroundService
{
	public DiscordLogFlushBackgroundService(BackgroundServiceMonitor backgroundServiceMonitor, ILogger<DiscordLogFlushBackgroundService> logger)
		: base(backgroundServiceMonitor, logger)
	{
	}

	// TODO: Refactor.
	public static List<DiscordEmbed> LogEntries { get; } = new();

	protected override TimeSpan Interval => TimeSpan.FromSeconds(2);

	protected override async Task ExecuteTaskAsync(CancellationToken stoppingToken)
	{
		DiscordChannel? channel = DevilDaggersInfoServerConstants.Channels[Channel.MonitoringLog].DiscordChannel;
		if (channel == null)
			return;

		while (LogEntries.Count > 0)
		{
			DiscordEmbed embed = LogEntries[0];
			await channel.SendMessageAsyncSafe(null, embed);
			LogEntries.RemoveAt(0);
		}
	}
}