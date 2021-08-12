﻿using DevilDaggersWebsite.BlazorWasm.Server.Entities;
using DevilDaggersWebsite.BlazorWasm.Shared.Dto.Admin.Donations;

namespace DevilDaggersWebsite.BlazorWasm.Server.Converters.Admin
{
	public static class DonationConverters
	{
		public static GetDonation ToGetDonation(this DonationEntity donation) => new()
		{
			Id = donation.Id,
			Amount = donation.Amount,
			ConvertedEuroCentsReceived = donation.ConvertedEuroCentsReceived,
			Currency = donation.Currency,
			DateReceived = donation.DateReceived,
			IsRefunded = donation.IsRefunded,
			Note = donation.Note,
			PlayerName = donation.Player.PlayerName,
		};
	}
}
