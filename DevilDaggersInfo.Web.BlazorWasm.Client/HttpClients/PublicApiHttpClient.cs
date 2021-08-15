﻿using DevilDaggersInfo.Web.BlazorWasm.Client.Utils;
using DevilDaggersInfo.Web.BlazorWasm.Shared.Dto;
using DevilDaggersInfo.Web.BlazorWasm.Shared.Dto.Public.CustomEntries;
using DevilDaggersInfo.Web.BlazorWasm.Shared.Dto.Public.CustomLeaderboards;
using DevilDaggersInfo.Web.BlazorWasm.Shared.Dto.Public.LeaderboardHistoryStatistics;
using DevilDaggersInfo.Web.BlazorWasm.Shared.Dto.Public.Leaderboards;
using DevilDaggersInfo.Web.BlazorWasm.Shared.Dto.Public.LeaderboardStatistics;
using DevilDaggersInfo.Web.BlazorWasm.Shared.Dto.Public.Players;
using DevilDaggersInfo.Web.BlazorWasm.Shared.Enums;
using DevilDaggersInfo.Web.BlazorWasm.Shared.Enums.Sortings.Public;
using System.Net.Http.Json;

namespace DevilDaggersInfo.Web.BlazorWasm.Client.HttpClients;

public class PublicApiHttpClient
{
	private readonly HttpClient _client;

	public PublicApiHttpClient(HttpClient client)
	{
		_client = client;
	}

	public async Task<GetCustomEntryData> GetCustomEntryDataById(int id)
	{
		return await _client.GetFromJsonAsync<GetCustomEntryData>($"api/custom-entries/{id}/data") ?? throw new JsonDeserializationException();
	}

	public async Task<GetCustomLeaderboard> GetCustomLeaderboardById(int id)
	{
		return await _client.GetFromJsonAsync<GetCustomLeaderboard>($"api/custom-leaderboards/{id}") ?? throw new JsonDeserializationException();
	}

	public async Task<Page<GetCustomLeaderboardOverview>> GetCustomLeaderboards(CustomLeaderboardCategory category, int pageIndex, int pageSize, CustomLeaderboardSorting sortBy, bool ascending)
	{
		Dictionary<string, object?> queryParameters = new()
		{
			{ nameof(category), category },
			{ nameof(pageIndex), pageIndex },
			{ nameof(pageSize), pageSize },
			{ nameof(sortBy), sortBy },
			{ nameof(ascending), ascending },
		};
		return await _client.GetFromJsonAsync<Page<GetCustomLeaderboardOverview>>(UrlBuilderUtils.BuildUrlWithQuery("api/custom-leaderboards", queryParameters)) ?? throw new JsonDeserializationException();
	}

	public async Task<List<GetLeaderboardHistoryStatistics>> GetLeaderboardHistoryStatistics()
	{
		return await _client.GetFromJsonAsync<List<GetLeaderboardHistoryStatistics>>("api/leaderboard-history-statistics") ?? throw new JsonDeserializationException();
	}

	public async Task<List<GetPlayerForLeaderboard>> GetPlayersForLeaderboard()
	{
		return await _client.GetFromJsonAsync<List<GetPlayerForLeaderboard>>("api/players/leaderboard") ?? throw new JsonDeserializationException();
	}

	public async Task<GetLeaderboardStatistics> GetLeaderboardStatistics()
	{
		return await _client.GetFromJsonAsync<GetLeaderboardStatistics>("api/leaderboard-statistics") ?? throw new JsonDeserializationException();
	}

	public async Task<GetLeaderboard> GetLeaderboard(int rankStart)
	{
		return await _client.GetFromJsonAsync<GetLeaderboard>(UrlBuilderUtils.BuildUrlWithQuery("api/leaderboards", new() { { nameof(rankStart), rankStart } })) ?? throw new JsonDeserializationException();
	}
}
