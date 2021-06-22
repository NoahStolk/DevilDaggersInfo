﻿using DevilDaggersWebsite.Enumerators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DevilDaggersWebsite.Dto
{
	public class AdminDonation : IAdminDto
	{
		public int PlayerId { get; init; }

		public int Amount { get; init; }

		public Currency Currency { get; init; }

		public int ConvertedEuroCentsReceived { get; init; }

		public DateTime DateReceived { get; init; }

		[StringLength(64)]
		public string? Note { get; init; }

		public bool IsRefunded { get; init; }

		public Dictionary<string, string> Log()
		{
			Dictionary<string, string> dictionary = new();
			dictionary.Add(nameof(PlayerId), PlayerId.ToString());
			dictionary.Add(nameof(Amount), Amount.ToString());
			dictionary.Add(nameof(Currency), Currency.ToString());
			dictionary.Add(nameof(ConvertedEuroCentsReceived), ConvertedEuroCentsReceived.ToString());
			dictionary.Add(nameof(DateReceived), DateReceived.ToString("dd MMM yyyy"));
			dictionary.Add(nameof(Note), Note ?? string.Empty);
			dictionary.Add(nameof(IsRefunded), IsRefunded.ToString());
			return dictionary;
		}
	}
}