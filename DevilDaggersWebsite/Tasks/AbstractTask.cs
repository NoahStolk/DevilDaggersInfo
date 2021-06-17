﻿using DevilDaggersWebsite.Tasks.Scheduling;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace DevilDaggersWebsite.Tasks
{
	public abstract class AbstractTask : IScheduledTask
	{
		public DateTime LastTriggered { get; private set; }

		public abstract string Schedule { get; }

		protected abstract Task Execute();

		public async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			Stopwatch stopwatch = new();
			stopwatch.Start();

			LastTriggered = DateTime.UtcNow;

			await Execute();
		}
	}
}
