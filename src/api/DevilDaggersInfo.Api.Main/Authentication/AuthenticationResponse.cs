namespace DevilDaggersInfo.Api.Main.Authentication;

public record AuthenticationResponse
{
	public int Id { get; init; }

	public string Name { get; init; } = null!;

	public List<string> RoleNames { get; init; } = new();

	public int? PlayerId { get; init; }
}
