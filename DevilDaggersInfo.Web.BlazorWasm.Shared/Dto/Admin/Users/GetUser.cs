namespace DevilDaggersInfo.Web.BlazorWasm.Shared.Dto.Admin.Users;

public class GetUser : IGetDto
{
	public int Id { get; init; }

	public string? Name { get; init; }

	[Display(Name = "Admin")]
	public bool IsAdmin { get; init; }

	[Display(Name = "Custom LBs")]
	public bool IsCustomLeaderboardsMaintainer { get; init; }

	[Display(Name = "Donations")]
	public bool IsDonationsMaintainer { get; init; }

	[Display(Name = "Mods")]
	public bool IsModsMaintainer { get; init; }

	[Display(Name = "Players")]
	public bool IsPlayersMaintainer { get; init; }

	[Display(Name = "Spawnsets")]
	public bool IsSpawnsetsMaintainer { get; init; }
}
