﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevilDaggersWebsite.Singletons
{
	public class ResponseTimeContainer
	{
		private const int _maxLinesPerMessage = 5;

		private readonly List<ResponseLog> _responseLogs = new();

		public void Add(string path, long responseTimeTicks, DateTime dateTime)
			=> _responseLogs.Add(new(path, responseTimeTicks, dateTime));

		public List<string> CreateLogs(DateTime startDateTime, DateTime endDateTime)
		{
			StringBuilder sb = new($"```Response times\n{startDateTime:yyyy-MM-dd HH:mm:ss}\n{endDateTime:yyyy-MM-dd HH:mm:ss}\n\n");

			if (_responseLogs.Count == 0)
			{
				sb.Append("Nobody accessed the website during this period.```");
				return new() { sb.ToString() };
			}

			List<string> logs = new();

			sb.AppendFormat("{0,-50}", "Path")
				.AppendFormat("{0,10}", "Requests")
				.AppendFormat("{0,30}", "Average response time")
				.AppendFormat("{0,20}", "Min time")
				.AppendFormat("{0,20}", "Max time")
				.AppendLine();
			int i = 0;
			foreach (IGrouping<string, ResponseLog> group in _responseLogs.GroupBy(rl => rl.Path.ToLower()).OrderBy(rl => rl.Key))
			{
				int count = group.Count();
				double averageResponseTimeTicks = group.Average(rl => rl.ResponseTimeTicks);
				long minResponseTimeTicks = group.Min(rl => rl.ResponseTimeTicks);
				long maxResponseTimeTicks = group.Max(rl => rl.ResponseTimeTicks);
				sb.AppendFormat("{0,-50}", group.Key)
					.AppendFormat("{0,10}", count)
					.AppendFormat("{0,30}", GetFormattedTime(averageResponseTimeTicks))
					.AppendFormat("{0,20}", GetFormattedTime(minResponseTimeTicks))
					.AppendFormat("{0,20}", GetFormattedTime(maxResponseTimeTicks))
					.AppendLine();

				i++;

				if (i >= _maxLinesPerMessage)
				{
					sb.AppendLine("```");
					logs.Add(sb.ToString());

					sb.Clear();
					sb.AppendLine("```");
					i = 0;
				}
			}

			return logs;
		}

		public void Clear()
			=> _responseLogs.Clear();

		private static string GetFormattedTime(double ticks)
		{
			if (ticks >= TimeSpan.TicksPerSecond)
				return $"{ticks / (float)TimeSpan.TicksPerSecond:0.00} s";

			if (ticks >= TimeSpan.TicksPerMillisecond)
				return $"{ticks / (float)TimeSpan.TicksPerMillisecond:0.0} ms";

			return $"{ticks / 10f:0} μs";
		}

		private static string GetFormattedTime(long ticks)
		{
			if (ticks >= TimeSpan.TicksPerSecond)
				return $"{ticks / (float)TimeSpan.TicksPerSecond:0.00} s";

			if (ticks >= TimeSpan.TicksPerMillisecond)
				return $"{ticks / (float)TimeSpan.TicksPerMillisecond:0.0} ms";

			return $"{ticks / 10f:0} μs";
		}

		private class ResponseLog
		{
			public ResponseLog(string path, long responseTimeTicks, DateTime dateTime)
			{
				Path = path;
				ResponseTimeTicks = responseTimeTicks;
				DateTime = dateTime;
			}

			public string Path { get; }

			public long ResponseTimeTicks { get; }

			public DateTime DateTime { get; }
		}
	}
}