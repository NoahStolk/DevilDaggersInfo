﻿using DevilDaggersWebsite.Enumerators;

namespace DevilDaggersWebsite.Extensions
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
			=> category == CustomLeaderboardCategory.TimeAttack || category == CustomLeaderboardCategory.Speedrun;
	}
}
