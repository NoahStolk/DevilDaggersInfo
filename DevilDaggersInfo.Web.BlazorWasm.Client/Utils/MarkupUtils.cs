using Microsoft.AspNetCore.Components;

namespace DevilDaggersInfo.Web.BlazorWasm.Client.Utils;

public static class MarkupUtils
{
	public static readonly MarkupString NoDataMarkup = new(@"<span class=""text-no-data"">N/A</span>");

	public static MarkupString DeathString(byte deathType, GameVersion gameVersion = GameVersion.V3_1)
	{
		Death? death = Deaths.GetDeathByLeaderboardType(gameVersion, deathType);
		string style = $"color: {death?.Color.HexCode ?? "#444"};";
		string name = death?.Name ?? "Unknown";

		return new(@$"<span style=""{style}"" class=""font-goethe text-lg"">{name}</span>");
	}

	public static MarkupString UpgradeString(Upgrade upgrade)
	{
		string style = $"color: {upgrade.Color.HexCode};";
		string name = upgrade.Name;

		return new(@$"<span style=""{style}"" class=""font-goethe text-lg"">{name}</span>");
	}

	public static MarkupString EnemyString(Enemy enemy, bool plural = false)
	{
		string style = $"color: {enemy.Color.HexCode};";
		string name = enemy.Name;

		return new(@$"<span style=""{style}"" class=""font-goethe text-lg"">{name}{(plural ? "s" : string.Empty)}</span>");
	}
}