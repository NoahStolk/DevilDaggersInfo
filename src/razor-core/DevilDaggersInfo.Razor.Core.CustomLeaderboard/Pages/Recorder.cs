using DevilDaggersInfo.Api.Ddcl.CustomLeaderboards;
using DevilDaggersInfo.Core.CustomLeaderboard.Enums;
using DevilDaggersInfo.Core.CustomLeaderboard.Memory;
using DevilDaggersInfo.Core.CustomLeaderboard.Services;
using Microsoft.AspNetCore.Components;

namespace DevilDaggersInfo.Razor.Core.CustomLeaderboard.Pages;

public partial class Recorder : IDisposable
{
	private Timer? _timer;

	private long? _marker;
	private bool _isRecording = true;
	private MainBlock _finalRecordedMainBlock;
	private GetUploadSuccess? _uploadSuccess;

	[Inject]
	public IClientConfiguration ClientConfiguration { get; set; } = null!;

	[Inject]
	public NetworkService NetworkService { get; set; } = null!;

	[Inject]
	public ReaderService ReaderService { get; set; } = null!;

	[Inject]
	public UploadService UploadService { get; set; } = null!;

	public void Dispose()
	{
		_timer?.Dispose();
	}

	protected override void OnInitialized()
	{
		_timer = new(
			callback: async _ =>
			{
				await Record();
				await InvokeAsync(StateHasChanged);
			},
			state: null,
			dueTime: 0,
			period: 50);
	}

	private async Task Record()
	{
		if (!_marker.HasValue)
		{
			_marker = await NetworkService.GetMarker(ClientConfiguration.GetOperatingSystem());
			if (!_marker.HasValue)
				return;
		}

		if (!ReaderService.FindWindow())
			return;

		if (!ReaderService.Initialize(_marker.Value))
			return;

		ReaderService.Scan();

		if (!_isRecording)
		{
			if (ReaderService.MainBlock.Time == ReaderService.MainBlockPrevious.Time || ReaderService.MainBlock.Status == (int)GameStatus.LocalReplay)
				return;

			_isRecording = true;
			_uploadSuccess = null;
		}

		bool justDied = !ReaderService.MainBlock.IsPlayerAlive && ReaderService.MainBlockPrevious.IsPlayerAlive;
		bool uploadRun = justDied && (ReaderService.MainBlock.GameMode == 0 || ReaderService.MainBlock.TimeAttackOrRaceFinished);
		if (!uploadRun)
			return;

		// TODO: We need to stop the timer here so it doesn't re-read memory while we're uploading the run...
		// When the validation is built, then the memory is re-read, and then we create the request, it will be invalid.
		if (!ReaderService.MainBlock.StatsLoaded)
		{
			await Task.Delay(TimeSpan.FromSeconds(0.5));
			return;
		}

		if (!ReaderService.IsReplayValid())
		{
			await Task.Delay(TimeSpan.FromSeconds(0.5));
			return;
		}

		_isRecording = false;

		string? errorMessage = ReaderService.ValidateRunLocally();
		if (errorMessage == null)
		{
			GetUploadSuccess? uploadSuccess = await UploadService.UploadRun();
			if (uploadSuccess != null)
			{
				_uploadSuccess = uploadSuccess;
				_finalRecordedMainBlock = ReaderService.MainBlock;
			}
			else
			{
				await Task.Delay(TimeSpan.FromSeconds(0.5));
			}
		}
		else
		{
			await Task.Delay(TimeSpan.FromSeconds(0.5));
		}
	}
}
