namespace DevilDaggersInfo.Api.Ddcl.CustomLeaderboards;

public readonly record struct GetScoreState<T>(T Value, T ValueDifference = default)
	where T : struct;
