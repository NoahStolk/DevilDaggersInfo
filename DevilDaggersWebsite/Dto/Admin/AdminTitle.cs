﻿using System.Collections.Generic;

namespace DevilDaggersWebsite.Dto.Admin
{
	public class AdminTitle
	{
		public string Name { get; init; } = null!;
		public List<int>? PlayerIds { get; init; }
	}
}
