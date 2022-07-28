using DevilDaggersInfo.Common;
using DevilDaggersInfo.Common.Utils;
using DevilDaggersInfo.Razor.Core.Canvas;
using DevilDaggersInfo.Razor.Core.CanvasChart.Data;
using DevilDaggersInfo.Razor.Core.CanvasChart.Options.LineChart;
using DevilDaggersInfo.Razor.Core.CanvasChart.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DevilDaggersInfo.Razor.Core.CanvasChart.Components;

public partial class LineChart
{
	private Canvas2d? _context;
	private object? _canvasReference;
	private ChartHighlighter? _highlighter;

	private int _canvasWidth;
	private int _canvasHeight;

	private double _canvasMouseX;
	private double _canvasMouseY;

	private bool _shouldRender = true;

	private double ChartMouseX => _canvasMouseX - Options.ChartMarginXInPx;

	private double ChartWidth => _canvasWidth - Options.ChartMarginXInPx * 2;
	private double ChartHeight => _canvasHeight - Options.ChartMarginYInPx * 2;

	[Inject]
	public IJSRuntime JsRuntime { get; set; } = null!;

	[Parameter]
	[EditorRequired]
	public string UniqueName { get; set; } = null!;

	[Parameter]
	[EditorRequired]
	public List<LineDataSet> DataSets { get; set; } = null!;

	[Parameter]
	[EditorRequired]
	public LineChartDataOptions DataOptions { get; set; } = null!;

	[Parameter]
	public LineChartOptions Options { get; set; } = new();

	[Parameter]
	public List<MarkupString> HighlighterValues { get; set; } = new();

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			await JsRuntime.InvokeAsync<object>("initChart");
			await JsRuntime.InvokeAsync<object>("registerChart", DotNetObjectReference.Create(this), UniqueName);
			await JsRuntime.InvokeAsync<object>("chartInitialResize", DotNetObjectReference.Create(this));
		}

		_context = new Canvas2d((IJSUnmarshalledRuntime)JsRuntime, $"{UniqueName}-canvas");

		Render();
	}

	private void Render()
	{
		if (_context == null || _highlighter == null)
			return;

		_highlighter.Width = Options.HighlighterWidth;

		// Determine grid.
		List<double> xScales = ScaleUtils.CalculateScales(ChartWidth, DataOptions.MinX, DataOptions.MaxX, DataOptions.StepX, Options.GridOptions.MinimumColumnWidthInPx, DataOptions.AllowFractionalScales);
		List<double> yScales = ScaleUtils.CalculateScales(ChartHeight, DataOptions.MinY, DataOptions.MaxY, DataOptions.StepY, Options.GridOptions.MinimumRowHeightInPx, DataOptions.AllowFractionalScales);

		// Render graphics.
		_context.ClearRect(0, 0, _canvasWidth, _canvasHeight);
		RenderBackground();
		RenderGrid();
		RenderSideBars();
		foreach (LineDataSet dataSet in DataSets)
			RenderDataLine(dataSet);

		void RenderBackground()
		{
			_context.FillStyle = Options.CanvasBackgroundColor;
			_context.FillRect(0, 0, _canvasWidth, _canvasHeight);
			_context.FillStyle = Options.ChartBackgroundColor;
			_context.FillRect(Options.ChartMarginXInPx, Options.ChartMarginYInPx, ChartWidth, ChartHeight);

			if (Options.Backgrounds?.Count > 0)
			{
				double end = DataOptions.MinX;
				foreach (LineChartBackground background in Options.Backgrounds)
				{
					double endX = LerpUtils.RevLerp(DataOptions.MinX, DataOptions.MaxX, background.ChartEndXValue) * ChartWidth;
					if (endX < 0)
						continue;

					double startX = LerpUtils.RevLerp(DataOptions.MinX, DataOptions.MaxX, end) * ChartWidth;
					if (startX < 0)
						startX = 0;

					_context.FillStyle = background.Color;
					_context.FillRect(Options.ChartMarginXInPx + startX, Options.ChartMarginYInPx, endX - startX, ChartHeight);

					end = background.ChartEndXValue;
				}
			}
		}

		void RenderGrid()
		{
			_context.LineWidth = Options.GridOptions.LineThickness;
			_context.StrokeStyle = Options.GridOptions.LineColor;

			_context.BeginPath();
			for (int i = 0; i < xScales.Count; i++)
			{
				double xScalePosition = LerpUtils.RevLerp(DataOptions.MinX, DataOptions.MaxX, xScales[i]) * ChartWidth;
				_context.MoveTo(Options.ChartMarginXInPx + xScalePosition, Options.ChartMarginYInPx);
				_context.LineTo(Options.ChartMarginXInPx + xScalePosition, _canvasHeight - Options.ChartMarginYInPx);
			}

			for (int i = 0; i < yScales.Count; i++)
			{
				double yScalePosition = LerpUtils.RevLerp(DataOptions.MinY, DataOptions.MaxY, yScales[i]) * ChartHeight;
				if (DataOptions.ReverseY)
					yScalePosition = ChartHeight - yScalePosition;

				_context.MoveTo(Options.ChartMarginXInPx, Options.ChartMarginYInPx + yScalePosition);
				_context.LineTo(_canvasWidth - Options.ChartMarginXInPx, Options.ChartMarginYInPx + yScalePosition);
			}

			_context.Stroke();
		}

		void RenderSideBars()
		{
			const int paddingX = 4;
			const int paddingY = 16;

			_context.StrokeStyle = Options.ScaleXOptions.TextColor;
			_context.Font = Options.ScaleXOptions.Font;
			_context.TextAlign = TextAlign.Center;
			for (int i = 0; i < xScales.Count; i++)
			{
				double xScaleValue = xScales[i];
				double xScalePosition = LerpUtils.RevLerp(DataOptions.MinX, DataOptions.MaxX, xScaleValue) * ChartWidth;
				string xScale = RenderScale(Options.XScaleDisplayUnit, xScaleValue, Options.ScaleXOptions.NumberFormat);

				_context.StrokeText(xScale, Options.ChartMarginXInPx + xScalePosition, Options.ChartMarginYInPx + ChartHeight + paddingY);
			}

			_context.StrokeStyle = Options.ScaleYOptions.TextColor;
			_context.Font = Options.ScaleYOptions.Font;
			_context.TextAlign = TextAlign.Right;
			for (int i = 0; i < yScales.Count; i++)
			{
				double yScaleValue = yScales[i];
				double yScalePosition = LerpUtils.RevLerp(DataOptions.MinY, DataOptions.MaxY, yScaleValue) * ChartHeight;
				if (DataOptions.ReverseY)
					yScalePosition = ChartHeight - yScalePosition;
				string yScale = RenderScale(Options.YScaleDisplayUnit, yScaleValue, Options.ScaleYOptions.NumberFormat);

				_context.StrokeText(yScale, Options.ChartMarginXInPx - paddingX, Options.ChartMarginYInPx + ChartHeight - yScalePosition);
			}

			static string RenderScale(ScaleDisplayUnit scaleDisplayUnit, double value, string? numberFormat) => scaleDisplayUnit switch
			{
				ScaleDisplayUnit.TicksAsDate => new DateTime((long)value).ToString(StringFormats.DateFormat),
				ScaleDisplayUnit.MinutesAsTime => TimeUtils.MinutesToTimeString((int)value),
				ScaleDisplayUnit.TicksAsSeconds => TimeUtils.TicksToTimeString(value),
				_ => value.ToString(numberFormat),
			};
		}

		void RenderDataLine(LineDataSet dataSet)
		{
			if (dataSet.Data.Count < 1)
				return;

			List<LinePosition> linePositions = new();
			for (int i = 0; i < dataSet.Data.Count; i++)
			{
				LineData data = dataSet.Data[i];
				double percX = LerpUtils.RevLerp(DataOptions.MinX, DataOptions.MaxX, data.X);
				double percY = LerpUtils.RevLerp(DataOptions.MinY, DataOptions.MaxY, data.Y);
				if (DataOptions.ReverseY)
					percY = 1 - percY;

				double x = Options.ChartMarginXInPx + percX * ChartWidth;
				double y = Options.ChartMarginYInPx + ChartHeight - percY * ChartHeight;

				if (i == dataSet.Data.Count - 1)
				{
					linePositions.Add(new(x, y));
					if (dataSet.AppendEnd && percX != 1)
						linePositions.Add(new(Options.ChartMarginXInPx + ChartWidth, y));
				}
				else
				{
					if (i == 0)
					{
						if (dataSet.PrependStart && percX != 0)
						{
							linePositions.Add(new(Options.ChartMarginXInPx, y));
							linePositions.Add(new(x, y));
						}
						else
						{
							linePositions.Add(new(x, y));
						}
					}
					else
					{
						linePositions.Add(new(x, y));
					}

					if (dataSet.IsSteppedLine)
					{
						double percNextX = LerpUtils.RevLerp(DataOptions.MinX, DataOptions.MaxX, dataSet.Data[i + 1].X);
						double nextX = Options.ChartMarginXInPx + percNextX * ChartWidth;
						linePositions.Add(new(nextX, y));
					}
				}
			}

			_context.LineWidth = 1;
			_context.StrokeStyle = dataSet.Color;

			_context.BeginPath();

			for (int i = 0; i < linePositions.Count; i++)
			{
				LinePosition linePosition = linePositions[i];

				// Improve performance by skipping unnecessary line calls.
				if (i > 0 && i < linePositions.Count - 1 && linePosition.Y == linePositions[i - 1].Y && linePosition.Y == linePositions[i + 1].Y)
					continue;

				if (i == 0)
					_context.MoveTo(linePosition.X, linePosition.Y);
				else
					_context.LineTo(linePosition.X, linePosition.Y);
			}

			_context.Stroke();
		}
	}

	[JSInvokable]
	public void OnResize(double wrapperWidth, double wrapperHeight)
	{
		_canvasWidth = (int)wrapperWidth;
		_canvasHeight = (int)wrapperHeight;

		Render();
	}

	[JSInvokable]
	public async ValueTask OnMouseMove(int mouseX, int mouseY)
	{
		BoundingClientRect canvasBoundingClientRect = await JsRuntime.InvokeAsync<BoundingClientRect>("getBoundingClientRect", _canvasReference);

		_canvasMouseX = mouseX - canvasBoundingClientRect.Left;
		_canvasMouseY = mouseY - canvasBoundingClientRect.Top;

		UpdateHighlighter();
	}

	private void UpdateHighlighter()
	{
		if (_highlighter == null)
			return;

		_highlighter.Left = Math.Clamp(_canvasMouseX, Options.ChartMarginXInPx, Options.ChartMarginXInPx + ChartWidth - _highlighter.Width);
		_highlighter.Top = Math.Clamp(_canvasMouseY, Options.ChartMarginYInPx, Options.ChartMarginYInPx + ChartHeight);

		double xValue = ChartMouseX / ChartWidth * (DataOptions.MaxX - DataOptions.MinX);

		HighlighterValues.Clear();
		for (int i = 0; i < DataSets.Count; i++)
		{
			LineDataSet dataSet = DataSets[i];
			if (dataSet.Data.Count == 0 || dataSet.ToHighlighterValue == null)
				continue;

			LineData highlightedData = dataSet.Data.OrderByDescending(ld => ld.X).FirstOrDefault(ld => ld.X < xValue + DataOptions.MinX) ?? dataSet.Data[0];
			HighlighterValues.AddRange(dataSet.ToHighlighterValue(dataSet, highlightedData));
		}

		_highlighter.ChangeState();

#if false
		if (_context == null || Options.HighlighterLineOptions == null)
			return;

		_context.LineWidth = Options.HighlighterLineOptions.LineThickness;
		_context.StrokeStyle = Options.HighlighterLineOptions.LineColor;
		_context.BeginPath();
		_context.MoveTo(_canvasMouseX, Options.ChartMarginYInPx);
		_context.LineTo(_canvasMouseX, _canvasHeight - Options.ChartMarginYInPx);
		_context.Stroke();
#endif
	}

	protected override bool ShouldRender() => _shouldRender;

	private void SetHighlighterVisibility(bool visible)
	{
		// Prevent re-rendering the entire canvas.
		_shouldRender = false;
		if (_highlighter != null)
		{
			_highlighter.IsVisible = visible;
			_highlighter.ChangeState();
		}
	}

	private readonly record struct LinePosition(double X, double Y);
}
