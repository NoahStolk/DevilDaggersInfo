using DevilDaggersInfo.Razor.Core.Canvas;
using DevilDaggersInfo.Razor.Core.CanvasChart.Data;
using DevilDaggersInfo.Razor.Core.CanvasChart.Options.BarChart;
using DevilDaggersInfo.Razor.Core.CanvasChart.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DevilDaggersInfo.Razor.Core.CanvasChart.Components;

public partial class BarChart
{
	private UnmarshalledCanvas2d? _context;
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
	public BarDataSet DataSet { get; set; } = null!;

	[Parameter]
	[EditorRequired]
	public BarChartDataOptions DataOptions { get; set; } = null!;

	[Parameter]
	[EditorRequired]
	public IEnumerable<string> XScaleTexts { get; set; } = null!;

	[Parameter]
	public BarChartOptions Options { get; set; } = new();

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

		_context = new UnmarshalledCanvas2d((IJSUnmarshalledRuntime)JsRuntime, $"{UniqueName}-canvas");

		Render();
	}

	private void Render()
	{
		if (_context == null || _highlighter == null)
			return;

		_highlighter.Width = Options.HighlighterWidth;

		// Clear canvas.
		_context.ClearRect(0, 0, _canvasWidth, _canvasHeight);

		// Determine grid.
		List<double> xScales = Enumerable.Range(0, DataSet.Data.Count).Select(i => (double)i).ToList();
		List<double> yScales = ScaleUtils.CalculateScales(ChartHeight, DataOptions.MinY, DataOptions.MaxY, DataOptions.StepY, Options.GridOptions.MinimumRowHeightInPx, false);
		double fullBarWidth = 1 / (double)DataSet.Data.Count * ChartWidth;

		// Set backgrounds.
		_context.FillStyle = Options.CanvasBackgroundColor;
		_context.FillRect(0, 0, _canvasWidth, _canvasHeight);
		_context.FillStyle = Options.ChartBackgroundColor;
		_context.FillRect(Options.ChartMarginXInPx, Options.ChartMarginYInPx, ChartWidth, ChartHeight);

		// Render graphics.
		RenderGrid();
		RenderSideBars();
		RenderBars();

		void RenderGrid()
		{
			_context.LineWidth = Options.GridOptions.LineThickness;
			_context.StrokeStyle = Options.GridOptions.LineColor;

			_context.BeginPath();
			for (int i = 0; i < xScales.Count; i++)
			{
				double xScalePosition = i / (double)xScales.Count * ChartWidth;
				_context.MoveTo(Options.ChartMarginXInPx + xScalePosition, Options.ChartMarginYInPx);
				_context.LineTo(Options.ChartMarginXInPx + xScalePosition, _canvasHeight - Options.ChartMarginYInPx);
			}

			for (int i = 0; i < yScales.Count; i++)
			{
				double yScalePosition = LerpUtils.RevLerp(DataOptions.MinY, DataOptions.MaxY, yScales[i]) * ChartHeight;
				_context.MoveTo(Options.ChartMarginXInPx, Options.ChartMarginYInPx + yScalePosition);
				_context.LineTo(_canvasWidth - Options.ChartMarginXInPx, Options.ChartMarginYInPx + yScalePosition);
			}

			_context.Stroke();
		}

		void RenderSideBars()
		{
			const int paddingX = 4;

			_context.StrokeStyle = Options.ScaleXOptions.TextColor;
			_context.Font = Options.ScaleXOptions.Font;
			_context.TextAlign = TextAlign.Center;

			int skippedXScaleTexts = fullBarWidth switch
			{
				< 3 => 5,
				< 6 => 4,
				< 10 => 3,
				< 20 => 2,
				_ => 1,
			};
			List<string> xScaleTexts = XScaleTexts.ToList();
			for (int i = 0; i < xScales.Count; i++)
			{
				if (i >= xScaleTexts.Count || i % skippedXScaleTexts != 0)
					continue;

				double xScaleValue = xScales[i];
				double xScalePosition = LerpUtils.RevLerp(0, DataSet.Data.Count, xScaleValue) * ChartWidth + 1 / (double)DataSet.Data.Count * ChartWidth * 0.5;
				_context.Save();
				_context.Translate(Options.ChartMarginXInPx + xScalePosition, Options.ChartMarginYInPx * 1.5 + ChartHeight);
				_context.Rotate((360 - 45) * (MathF.PI / 180));
				_context.StrokeText(xScaleTexts[i], 0, 0);
				_context.Restore();
			}

			_context.StrokeStyle = Options.ScaleYOptions.TextColor;
			_context.Font = Options.ScaleYOptions.Font;
			_context.TextAlign = TextAlign.Right;
			for (int i = 0; i < yScales.Count; i++)
			{
				double yScaleValue = yScales[i];
				double yScalePosition = LerpUtils.RevLerp(DataOptions.MinY, DataOptions.MaxY, yScaleValue) * ChartHeight;
				_context.StrokeText(yScaleValue.ToString(Options.ScaleYOptions.NumberFormat), Options.ChartMarginXInPx - paddingX, Options.ChartMarginYInPx + ChartHeight - yScalePosition);
			}
		}

		void RenderBars()
		{
			if (DataSet.Data.Count == 0)
				return;

			for (int i = 0; i < DataSet.Data.Count; i++)
			{
				BarData data = DataSet.Data[i];
				double percX = i / (double)DataSet.Data.Count;
				double percY = LerpUtils.RevLerp(DataOptions.MinY, DataOptions.MaxY, data.Y);
				double x = percX * ChartWidth;
				double y = ChartHeight - percY * ChartHeight;

				double padding = fullBarWidth * 0.05;

				_context.FillStyle = data.Color;
				_context.FillRect(x + Options.ChartMarginXInPx + padding, y + Options.ChartMarginYInPx, fullBarWidth - padding * 2, ChartHeight - y);
			}
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

		int index = Math.Clamp((int)(ChartMouseX / ChartWidth * DataSet.Data.Count), 0, DataSet.Data.Count - 1);

		HighlighterValues.Clear();
		HighlighterValues.AddRange(DataSet.ToHighlighterValue(DataSet, index));

		_highlighter.ChangeState();
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
}
