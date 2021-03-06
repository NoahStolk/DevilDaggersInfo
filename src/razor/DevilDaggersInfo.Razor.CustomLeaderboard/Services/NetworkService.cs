using DevilDaggersInfo.Api.Ddcl;
using DevilDaggersInfo.Api.Ddcl.CustomLeaderboards;
using DevilDaggersInfo.Api.Ddcl.ProcessMemory;
using DevilDaggersInfo.Api.Ddcl.Tools;
using DevilDaggersInfo.Razor.CustomLeaderboard.HttpClients;
using DevilDaggersInfo.Razor.CustomLeaderboard.Models;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Json;

namespace DevilDaggersInfo.Razor.CustomLeaderboard.Services;

public class NetworkService
{
	private readonly ILogger<NetworkService> _logger;
	private readonly IClientConfiguration _clientConfiguration;

	private readonly DdclApiHttpClient _apiClient;

	public NetworkService(ILogger<NetworkService> logger, IClientConfiguration clientConfiguration)
	{
		_logger = logger;
		_clientConfiguration = clientConfiguration;

		_apiClient = new(new() { BaseAddress = new(clientConfiguration.GetHostBaseUrl()) });
	}

	public async Task<GetUpdate?> GetUpdate()
	{
		const int maxAttempts = 5;
		for (int i = 0; i < maxAttempts; i++)
		{
			try
			{
				return await _apiClient.GetUpdates(_clientConfiguration.GetToolPublishMethod(), _clientConfiguration.GetToolBuildType());
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error while trying to retrieve tool (attempt {attempt} out of {maxAttempts}).", i + 1, maxAttempts);
				await Task.Delay(TimeSpan.FromSeconds(1));
			}
		}

		return null;
	}

	public async Task<long?> GetMarker(SupportedOperatingSystem supportedOperatingSystem)
	{
		try
		{
			GetMarker marker = await _apiClient.GetMarker(supportedOperatingSystem);
			return marker.Value;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error while trying to get marker.");
			return null;
		}
	}

	public async Task<bool> CheckIfLeaderboardExists(byte[] survivalHashMd5)
	{
		const int maxAttempts = 5;
		for (int i = 0; i < maxAttempts; i++)
		{
			try
			{
				HttpResponseMessage hrm = await _apiClient.CustomLeaderboardExistsBySpawnsetHash(survivalHashMd5);
				if (hrm.StatusCode == HttpStatusCode.OK)
					return true;

				if (hrm.StatusCode == HttpStatusCode.NotFound)
					return false;

				string error = await hrm.Content.ReadAsStringAsync();
				throw new HttpRequestException($"Unexpected status code '{hrm.StatusCode}': {error}"); // TODO: Do not retry.
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error while trying to check for existing leaderboard (attempt {attempt} out of {maxAttempts}).", i + 1, maxAttempts);
				await Task.Delay(TimeSpan.FromSeconds(1));
			}
		}

		return false;
	}

	public async Task<ResponseWrapper<GetUploadSuccess>> SubmitScore(AddUploadRequest uploadRequest)
	{
		try
		{
			HttpResponseMessage hrm = await _apiClient.SubmitScoreForDdcl(uploadRequest);
			if (hrm.IsSuccessStatusCode)
			{
				GetUploadSuccess success = await hrm.Content.ReadFromJsonAsync<GetUploadSuccess>() ?? throw new InvalidOperationException($"Could not deserialize the response as '{nameof(GetUploadSuccess)}'.");
				return new(success);
			}

			string error = await hrm.Content.ReadAsStringAsync();
			return new(error);
		}
		catch (Exception ex)
		{
			const string message = "Error trying to submit score";
			_logger.LogError(ex, message);
			return new(message);
		}
	}

	public async Task<ResponseWrapper<GetCustomLeaderboard>> GetLeaderboard(byte[] hash)
	{
		const int maxAttempts = 5;
		for (int i = 0; i < maxAttempts; i++)
		{
			try
			{
				GetCustomLeaderboard lb = await _apiClient.GetCustomLeaderboardBySpawnsetHash(hash);
				return new(lb);
			}
			catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
			{
				return new(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error while trying to retrieve leaderboard (attempt {attempt} out of {maxAttempts}).", i + 1, maxAttempts);
				await Task.Delay(TimeSpan.FromSeconds(1));
			}
		}

		return new("Couldn't retrieve leaderboard after 5 attempts.");
	}

	public async Task<ResponseWrapper<Page<GetCustomLeaderboardForOverview>>> GetLeaderboardOverview(CustomLeaderboardCategory category, int pageIndex, int pageSize, int selectedPlayerId, bool onlyFeatured)
	{
		const int maxAttempts = 5;
		for (int i = 0; i < maxAttempts; i++)
		{
			try
			{
				Page<GetCustomLeaderboardForOverview> overview = await _apiClient.GetCustomLeaderboardOverview(category, pageIndex, pageSize, selectedPlayerId, onlyFeatured);
				return new(overview);
			}
			catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
			{
				return new(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error while trying to retrieve leaderboard overview (attempt {attempt} out of {maxAttempts}).", i + 1, maxAttempts);
				await Task.Delay(TimeSpan.FromSeconds(1));
			}
		}

		return new("Couldn't retrieve leaderboard overview after 5 attempts.");
	}

	public async Task<byte[]?> GetReplay(int customEntryId)
	{
		try
		{
			return (await _apiClient.GetCustomEntryReplayBufferById(customEntryId)).Data;
		}
		catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
		{
			_logger.LogWarning(ex, "Custom entry ID {id} does not have a replay.", customEntryId);
			return null;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error while trying to download replay.");
			return null;
		}
	}

	public async Task<byte[]?> GetSpawnset(int spawnsetId)
	{
		try
		{
			return (await _apiClient.GetSpawnsetBufferById(spawnsetId)).Data;
		}
		catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
		{
			_logger.LogWarning(ex, "Spawnset {id} was not found.", spawnsetId);
			return null;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error while trying to download spawnset.");
			return null;
		}
	}
}
