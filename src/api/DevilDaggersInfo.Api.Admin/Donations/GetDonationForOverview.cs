namespace DevilDaggersInfo.Api.Admin.Donations;

public record GetDonationForOverview : IAdminOverviewGetDto
{
	public int Id { get; init; }

	public string PlayerName { get; init; } = null!;

	public int Amount { get; init; }

	public Currency Currency { get; init; }

	public int ConvertedEuroCentsReceived { get; init; }

	public DateTime DateReceived { get; init; }

	public string? Note { get; init; }

	public bool IsRefunded { get; init; }
}
