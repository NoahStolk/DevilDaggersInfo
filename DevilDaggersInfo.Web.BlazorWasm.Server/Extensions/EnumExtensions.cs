﻿using DevilDaggersInfo.Web.BlazorWasm.Server.HostedServices.DdInfoDiscordBot;
using DevilDaggersInfo.Web.BlazorWasm.Shared.Enums;
using DSharpPlus.Entities;

namespace DevilDaggersInfo.Web.BlazorWasm.Server.Extensions
{
	public static class EnumExtensions
	{
		public static char GetChar(this Currency currency) => currency switch
		{
			Currency.Eur => '€',
			Currency.Usd => '$',
			Currency.Aud => '$',
			Currency.Gbp => '£',
			Currency.Sgd => '$',
			_ => '?',
		};

		public static bool IsAscending(this CustomLeaderboardCategory category)
			=> category is CustomLeaderboardCategory.TimeAttack or CustomLeaderboardCategory.Speedrun;

		public static DiscordColor GetDiscordColor(this CustomLeaderboardDagger dagger) => dagger switch
		{
			CustomLeaderboardDagger.Leviathan => DiscordColors.Leviathan,
			CustomLeaderboardDagger.Devil => DiscordColors.Devil,
			CustomLeaderboardDagger.Golden => DiscordColors.Golden,
			CustomLeaderboardDagger.Silver => DiscordColors.Silver,
			CustomLeaderboardDagger.Bronze => DiscordColors.Bronze,
			_ => DiscordColors.Default,
		};
	}
}
