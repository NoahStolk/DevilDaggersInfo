namespace DevilDaggersInfo.Web.BlazorWasm.Shared.Dto.Public.Players;

public class GetPlayerForSettings : IGetDto<int>
{
	public int Id { get; init; }

	public string? CountryCode { get; init; }

	public GetPlayerSettings Settings { get; init; } = null!;
}
