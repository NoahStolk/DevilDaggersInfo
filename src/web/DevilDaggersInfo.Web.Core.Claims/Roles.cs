namespace DevilDaggersInfo.Web.Core.Claims;

public static class Roles
{
	public const string Admin = nameof(Admin);
	public const string CustomLeaderboards = nameof(CustomLeaderboards);
	public const string Mods = nameof(Mods);
	public const string Players = nameof(Players);
	public const string Spawnsets = nameof(Spawnsets);

	public static string[] All { get; } = new[]
	{
		Admin,
		CustomLeaderboards,
		Mods,
		Players,
		Spawnsets,
	};
}
