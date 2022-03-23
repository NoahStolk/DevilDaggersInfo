namespace DevilDaggersInfo.Web.BlazorWasm.Shared.Dto.Public.CustomEntries;

public readonly record struct GetScoreState<T>(T Value, T ValueDifference = default)
	where T : struct;
