using System.Runtime.Serialization;

namespace DevilDaggersInfo.Web.Shared.Enums;

public enum CustomLeaderboardsClient
{
	[EnumMember(Value = "DevilDaggersCustomLeaderboards")]
	DevilDaggersCustomLeaderboards = 0,

	[EnumMember(Value = "ddstats-rust")]
	DdstatsRust = 1,
}
