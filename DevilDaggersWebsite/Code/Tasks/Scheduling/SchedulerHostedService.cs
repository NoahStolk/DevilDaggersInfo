﻿using DevilDaggersWebsite.Code.Tasks.Cron;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevilDaggersWebsite.Code.Tasks.Scheduling
{
	public class SchedulerHostedService : HostedService
	{
		public event EventHandler<UnobservedTaskExceptionEventArgs> UnobservedTaskException;

		private readonly List<SchedulerTaskWrapper> _scheduledTasks = new List<SchedulerTaskWrapper>();

		public SchedulerHostedService(IEnumerable<IScheduledTask> scheduledTasks)
		{
			DateTime referenceTime = DateTime.UtcNow;

			foreach (IScheduledTask scheduledTask in scheduledTasks)
			{
				_scheduledTasks.Add(new SchedulerTaskWrapper
				{
					Schedule = CrontabSchedule.Parse(scheduledTask.Schedule),
					Task = scheduledTask,
					NextRunTime = referenceTime
				});
			}
		}

		protected override async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				await ExecuteOnceAsync(cancellationToken);

				await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
			}
		}

		private async Task ExecuteOnceAsync(CancellationToken cancellationToken)
		{
			TaskFactory taskFactory = new TaskFactory(TaskScheduler.Current);
			DateTime referenceTime = DateTime.UtcNow;

			List<SchedulerTaskWrapper> tasksThatShouldRun = _scheduledTasks.Where(t => t.ShouldRun(referenceTime)).ToList();

			foreach (SchedulerTaskWrapper taskThatShouldRun in tasksThatShouldRun)
			{
				taskThatShouldRun.Increment();

				await taskFactory.StartNew(
					async () =>
					{
						try
						{
							await taskThatShouldRun.Task.ExecuteAsync(cancellationToken);
						}
						catch (Exception ex)
						{
							UnobservedTaskExceptionEventArgs args = new UnobservedTaskExceptionEventArgs(
								ex as AggregateException ?? new AggregateException(ex));

							UnobservedTaskException?.Invoke(this, args);

							if (!args.Observed)
							{
								throw;
							}
						}
					},
					cancellationToken);
			}
		}

		private class SchedulerTaskWrapper
		{
			public CrontabSchedule Schedule { get; set; }
			public IScheduledTask Task { get; set; }

			public DateTime LastRunTime { get; set; }
			public DateTime NextRunTime { get; set; }

			public void Increment()
			{
				LastRunTime = NextRunTime;
				NextRunTime = Schedule.GetNextOccurrence(NextRunTime);
			}

			public bool ShouldRun(DateTime currentTime)
			{
				return NextRunTime < currentTime && LastRunTime != NextRunTime;
			}
		}
	}
}