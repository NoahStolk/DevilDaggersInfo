﻿using DevilDaggersInfo.Web.BlazorWasm.Server.Extensions;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;

namespace DevilDaggersInfo.Web.BlazorWasm.Server.HostedServices.DdInfoDiscordBot
{
	public static class Commands
	{
		public static Dictionary<string, Action<MessageCreateEventArgs>> Actions { get; } = new()
		{
			{ ".bot", async (e) => await e.Channel.SendMessageAsyncSafe("Hi.") },
		};
	}
}
