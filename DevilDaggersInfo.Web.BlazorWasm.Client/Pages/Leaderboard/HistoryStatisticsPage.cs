using DevilDaggersInfo.Web.BlazorWasm.Client.Core.CanvasChart.Data;
using DevilDaggersInfo.Web.BlazorWasm.Client.Core.CanvasChart.Options;
using DevilDaggersInfo.Web.BlazorWasm.Client.Core.CanvasChart.Options.LineChart;
using DevilDaggersInfo.Web.BlazorWasm.Shared.Dto.Public.LeaderboardHistoryStatistics;
using DevilDaggersInfo.Web.BlazorWasm.Shared.Dto.Public.Players;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DevilDaggersInfo.Web.BlazorWasm.Client.Pages.Leaderboard;

public partial class HistoryStatisticsPage
{
	private readonly LineChartOptions _playersLineChartOptions = new()
	{
		HighlighterTitle = "Date",
		HighlighterTitleValueNumberFormat = "0", // TODO: Date.
		HighlighterKeys = new() { "Players" },
		GridOptions = new()
		{
			MinimumRowHeightInPx = 50,
		},
		ChartMarginXInPx = 60,
	};

	private readonly LineChartOptions _entrancesLineChartOptions = new()
	{
		HighlighterTitle = "Date",
		HighlighterTitleValueNumberFormat = "0", // TODO: Date.
		HighlighterKeys = new() { "Top 10 Score", "Top 100 Score" },
		GridOptions = new()
		{
			MinimumRowHeightInPx = 50,
		},
	};

	private readonly LineChartOptions _accuracyLineChartOptions = new()
	{
		HighlighterTitle = "Date",
		HighlighterTitleValueNumberFormat = "0", // TODO: Date.
		HighlighterKeys = new() { "Global Accuracy" },
		GridOptions = new()
		{
			MinimumRowHeightInPx = 50,
		},
	};

	private readonly List<LineDataSet> _playersData = new();
	private readonly List<LineDataSet> _entrancesData = new();
	private readonly List<LineDataSet> _accuracyData = new();

	private DataOptions? _playersOptions;
	private DataOptions? _entrancesOptions;
	private DataOptions? _accuracyOptions;

	private List<GetLeaderboardHistoryStatistics>? _statistics;

	[Inject]
	public IJSRuntime JsRuntime { get; set; } = null!;

	protected override async Task OnInitializedAsync()
	{
		_statistics = await Http.GetLeaderboardHistoryStatistics();

		if (_statistics.Count == 0)
			return;

		DateTime minX = _statistics.Select(hs => hs.DateTime).Min();
		DateTime maxX = DateTime.Now;

		RegisterTotalPlayers();
		void RegisterTotalPlayers()
		{
			IEnumerable<int> totalPlayers = _statistics.Select(hs => hs.TotalPlayers);
			const double scale = 50000.0;
			double minY = Math.Floor(totalPlayers.Min() / scale) * scale;
			double maxY = Math.Ceiling(totalPlayers.Max() / scale) * scale;

			List<LineData> set = _statistics.Select(hs => new LineData((hs.DateTime.Ticks - minX.Ticks), hs.TotalPlayers)).ToList();
			_playersOptions = new(0, null, (maxX - minX).Ticks, minY, scale, maxY);
			_playersData.Add(new("#f00", false, false, false, set, (ds, d) => new List<MarkupString> { new($"<span style='color: {ds.Color}; text-align: right;'>{d.Y.ToString("0")}</span>") }));
		}

		RegisterEntrances();
		void RegisterEntrances()
		{
			IEnumerable<double> top10Entrances = _statistics.Select(hs => hs.Top10Entrance);
			IEnumerable<double> top100Entrances = _statistics.Select(hs => hs.Top100Entrance);
			const double scale = 100.0;
			double minY = Math.Floor(top100Entrances.Min() / scale) * scale;
			double maxY = Math.Ceiling(top10Entrances.Max() / scale) * scale;
			_entrancesOptions = new(0, null, (maxX - minX).Ticks, minY, scale, maxY);

			List<LineData> top10Set = _statistics.Select(hs => new LineData((hs.DateTime.Ticks - minX.Ticks), hs.Top10Entrance)).ToList();
			_entrancesData.Add(new("#800", false, false, false, top10Set, (ds, d) => new List<MarkupString> { new($"<span style='color: {ds.Color}; text-align: right;'>{d.Y.ToString("0.0000")}</span>") }));

			List<LineData> top100Set = _statistics.Select(hs => new LineData((hs.DateTime.Ticks - minX.Ticks), hs.Top100Entrance)).ToList();
			_entrancesData.Add(new("#f00", false, false, false, top100Set, (ds, d) => new List<MarkupString> { new($"<span style='color: {ds.Color}; text-align: right;'>{d.Y.ToString("0.0000")}</span>") }));
		}

		RegisterAccuracy();
		void RegisterAccuracy()
		{
			Func<ulong, ulong, double> accuracyConverter = static (hit, fired) => fired == 0 ? 0 : hit / (double)fired * 100;

			IEnumerable<double> accuracy = _statistics.Select(hs => accuracyConverter(hs.DaggersHitGlobal, hs.DaggersFiredGlobal));
			const double scale = 5.0;
			double minY = Math.Floor(accuracy.Min() / scale) * scale;
			double maxY = Math.Ceiling(accuracy.Max() / scale) * scale;

			List<LineData> set = _statistics.Select(hs => new LineData((hs.DateTime.Ticks - minX.Ticks), accuracyConverter(hs.DaggersHitGlobal, hs.DaggersFiredGlobal))).ToList();
			_accuracyOptions = new(0, null, (maxX - minX).Ticks, minY, scale, maxY);
			_accuracyData.Add(new("#f80", false, false, false, set, (ds, d) => new List<MarkupString> { new($"<span style='color: {ds.Color}; text-align: right;'>{d.Y.ToString("0.00")}%</span>") }));
		}
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
			await JsRuntime.InvokeAsync<object>("init");
	}
}
