﻿using DevilDaggersWebsite.HostedServices.DdInfoDiscordBot;
using DevilDaggersWebsite.Singletons;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DevilDaggersWebsite.HostedServices
{
	public abstract class AbstractBackgroundService : BackgroundService
	{
		protected AbstractBackgroundService(IWebHostEnvironment environment, BackgroundServiceMonitor backgroundServiceMonitor, DiscordLogger discordLogger)
		{
			Environment = environment;
			BackgroundServiceMonitor = backgroundServiceMonitor;
			DiscordLogger = discordLogger;

			Name = GetType().Name;
		}

		protected IWebHostEnvironment Environment { get; }
		protected BackgroundServiceMonitor BackgroundServiceMonitor { get; }
		protected DiscordLogger DiscordLogger { get; }

		protected string Name { get; }

		protected virtual bool LogExceptions => true;

		protected abstract TimeSpan Interval { get; }

		protected abstract Task ExecuteTaskAsync(CancellationToken stoppingToken);

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			Begin();

			while (!stoppingToken.IsCancellationRequested)
			{
				BackgroundServiceMonitor.Update(Name, DateTime.UtcNow);

				try
				{
					await ExecuteTaskAsync(stoppingToken);
				}
				catch (Exception ex)
				{
					if (LogExceptions)
						await DiscordLogger.TryLog(Channel.MonitoringTask, Environment.EnvironmentName, $":x: Task execution for `{Name}` failed with exception: `{ex.Message}`");
				}

				if (Interval.TotalMilliseconds > 0)
					await Task.Delay(Interval, stoppingToken);
			}

			await End();
		}

		protected virtual void Begin()
			=> BackgroundServiceMonitor.Register(Name, Interval);

		protected virtual async Task End()
			=> await DiscordLogger.TryLog(Channel.MonitoringTask, Environment.EnvironmentName, $":x: Cancellation for `{Name}` was requested.");
	}
}