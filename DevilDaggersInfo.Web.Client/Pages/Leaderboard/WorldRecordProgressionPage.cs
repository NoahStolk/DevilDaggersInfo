using DevilDaggersInfo.Core.Wiki.Extensions;
using DevilDaggersInfo.Web.BlazorWasm.Client.Core.CanvasChart.Data;
using DevilDaggersInfo.Web.BlazorWasm.Client.Core.CanvasChart.Options.LineChart;
using DevilDaggersInfo.Web.BlazorWasm.Client.HttpClients;
using DevilDaggersInfo.Web.BlazorWasm.Client.Utils;
using DevilDaggersInfo.Web.BlazorWasm.Shared.Dto.Public.WorldRecords;
using DevilDaggersInfo.Web.BlazorWasm.Shared.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DevilDaggersInfo.Web.BlazorWasm.Client.Pages.Leaderboard;

public partial class WorldRecordProgressionPage
{
	private readonly LineChartOptions _lineChartOptions = new()
	{
		HighlighterKeys = new() { "Date", "Time", "Player", "Gems", "Kills", "Accuracy", "Death Type", "Game Version" },
		GridOptions = new()
		{
			MinimumRowHeightInPx = 50,
		},
		XScaleDisplayUnit = ScaleDisplayUnit.TicksAsDate,
		Backgrounds = LineChartUtils.GameVersionBackgrounds,
	};

	private readonly List<LineDataSet> _lineDataSets = new();

	private double _currentWorldRecord;
	private int _currentWorldRecordHolderId;
	private TimeSpan _totalTimeSinceFirstRecord;

	private LineChartDataOptions _dataOptions = LineChartDataOptions.Default;
	private GetWorldRecordDataContainer? _data;

	[Inject]
	public PublicApiHttpClient Http { get; set; } = null!;

	[Inject]
	public IJSRuntime JsRuntime { get; set; } = null!;

	protected override async Task OnInitializedAsync()
	{
		_data = await Http.GetWorldRecordData();
		GetWorldRecord currentWr = _data.WorldRecords.MaxBy(wr => wr.Entry.Time)!;
		_currentWorldRecord = currentWr.Entry.Time;
		_currentWorldRecordHolderId = currentWr.Entry.Id;

		_totalTimeSinceFirstRecord = DateTime.UtcNow - _data.WorldRecords.OrderBy(wr => wr.DateTime).First().DateTime;

		DateTime minX = new(2016, 1, 1);
		DateTime maxX = DateTime.UtcNow;
		GetWorldRecord firstWr = _data.WorldRecords[0];
		GetWorldRecord lastWr = _data.WorldRecords[^1];
		double minY = Math.Floor(firstWr.Entry.Time / 100.0) * 100;
		double maxY = Math.Ceiling(lastWr.Entry.Time / 100.0) * 100;

		List<LineData> set = _data.WorldRecords.Select((wr, i) => new LineData(wr.DateTime.Ticks, wr.Entry.Time, i)).ToList();
		_dataOptions = new(minX.Ticks, null, maxX.Ticks, minY, 100, maxY);
		_lineDataSets.Add(new("#f00", true, true, true, set, (ds, d) =>
		{
			GetWorldRecord? wr = _data.WorldRecords.Count <= d.Index ? null : _data.WorldRecords[d.Index];
			if (wr == null)
				return new();

			GameVersion? gameVersion = GameVersions.GetGameVersionFromDate(wr.DateTime);
			Dagger dagger = Daggers.GetDaggerFromSeconds(gameVersion ?? GameVersion.V1_0, wr.Entry.Time);
			return new()
			{
				new($"<span style='text-align: right;'>{wr.DateTime.ToString(FormatUtils.DateFormat)}</span>"),
				new($"<span style='text-align: right;' class='{dagger.Name.ToLower()}'>{wr.Entry.Time.ToString(FormatUtils.TimeFormat)}</span>"),
				new($"<span style='text-align: right;' class='{dagger.Name.ToLower()}'>{wr.Entry.Username}</span>"),
				new($"<span style='text-align: right;'>{wr.Entry.Gems}</span>"),
				new($"<span style='text-align: right;'>{wr.Entry.Kills}</span>"),
				new($"<span style='text-align: right;'>{(wr.Entry.DaggersFired == 0 ? 0 : wr.Entry.DaggersHit / (double)wr.Entry.DaggersFired).ToString(FormatUtils.AccuracyFormat)}</span>"),
				new($"<span style='text-align: right;'>{MarkupUtils.DeathString(wr.Entry.DeathType, gameVersion ?? GameVersion.V1_0)}</span>"),
				new($"<span style='text-align: right;'>{gameVersion.GetGameVersionString()}</span>"),
			};
		}));
	}

	private string GetHistoryDateString(DateTime dateTime)
	{
		int daysAgo = (int)Math.Round((DateTime.UtcNow - dateTime).TotalDays);
		return $"{dateTime:MMM dd} '{dateTime:yy} ({daysAgo} day{S(daysAgo)} ago)";
	}

	private static string S(int value) => value == 1 ? string.Empty : "s";
}
