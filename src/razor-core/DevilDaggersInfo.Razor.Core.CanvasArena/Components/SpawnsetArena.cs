using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Core.Spawnset.Enums;
using DevilDaggersInfo.Razor.Core.Canvas;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DevilDaggersInfo.Razor.Core.CanvasArena.Components;

public partial class SpawnsetArena
{
	private int _canvasSize;
	private float _tileSize;

	private UnmarshalledCanvasArena? _context;
	private object? _canvasReference;

	private double _canvasMouseX;
	private double _canvasMouseY;

	private SpawnsetArenaHoverInfo _spawnsetArenaHoverInfo = null!;

	[Inject]
	public IJSRuntime JsRuntime { get; set; } = null!;

	[Parameter]
	[EditorRequired]
	public SpawnsetBinary SpawnsetBinary { get; set; } = null!;

	[Parameter]
	[EditorRequired]
	public float CurrentTime { get; set; }

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			await JsRuntime.InvokeAsync<object>("initArena");
			await JsRuntime.InvokeAsync<object>("registerArena", DotNetObjectReference.Create(this));
			await JsRuntime.InvokeAsync<object>("arenaInitialResize");
		}

		_context = new UnmarshalledCanvasArena((IJSUnmarshalledRuntime)JsRuntime, "arena-canvas");

		Render();
	}

	[JSInvokable]
	public void OnResize(double wrapperSize)
	{
		_canvasSize = (int)wrapperSize;
		_tileSize = _canvasSize / 51f;

		Render();
	}

	private void Render()
	{
		if (_context == null)
			return;

		_context.ClearRect(0, 0, _canvasSize, _canvasSize);
		for (int i = 0; i < SpawnsetBinary.ArenaDimension; i++)
		{
			for (int j = 0; j < SpawnsetBinary.ArenaDimension; j++)
			{
				float tileHeight = SpawnsetBinary.ArenaTiles[i, j];
				if (tileHeight < -1)
					continue;

				float actualTileHeight = SpawnsetBinary.GetActualTileHeight(i, j, CurrentTime);
				Color color = Color.GetColorFromHeight(actualTileHeight);
				if (color.R == 0 && color.G == 0 && color.B == 0)
					continue;

				_context.DrawTile(i, j, color.R, color.G, color.B, _tileSize);
			}
		}

		const int tileUnit = 4; // Tiles are 4 units in width/length in the game.
		float shrinkEndTime = SpawnsetBinary.GetShrinkEndTime();
		float shrinkRadius = shrinkEndTime == 0 ? SpawnsetBinary.ShrinkStart : Math.Max(SpawnsetBinary.ShrinkStart - CurrentTime / shrinkEndTime * (SpawnsetBinary.ShrinkStart - SpawnsetBinary.ShrinkEnd), SpawnsetBinary.ShrinkEnd);
		if (shrinkRadius > 0 && shrinkRadius <= 100)
		{
			_context.StrokeStyle = "#f08";
			_context.LineWidth = 1;
			_context.BeginPath();
			_context.Circle(_canvasSize / 2f, _canvasSize / 2f, shrinkRadius / tileUnit * _tileSize);
			_context.Stroke();
		}

		if (SpawnsetBinary.GameMode == GameMode.Race)
			RenderRaceDagger();

		RenderPlayer();

		void RenderRaceDagger()
		{
			(_, float? y, _) = SpawnsetBinary.GetRaceDaggerTilePosition();
			if (!y.HasValue)
				return;

			float daggerCenterX = _canvasSize / 2 + SpawnsetBinary.RaceDaggerPosition.X / tileUnit * _tileSize;
			float daggerCenterY = _canvasSize / 2 + SpawnsetBinary.RaceDaggerPosition.Y / tileUnit * _tileSize;

			_context.BeginPath();
			_context.MoveTo(daggerCenterX, daggerCenterY + 6);
			_context.LineTo(daggerCenterX + 4, daggerCenterY - 6);
			_context.LineTo(daggerCenterX + 1, daggerCenterY - 6);
			_context.LineTo(daggerCenterX + 1, daggerCenterY - 10);
			_context.LineTo(daggerCenterX - 1, daggerCenterY - 10);
			_context.LineTo(daggerCenterX - 1, daggerCenterY - 6);
			_context.LineTo(daggerCenterX - 4, daggerCenterY - 6);
			_context.ClosePath();

			_context.FillStyle = "#444";
			_context.Fill();

			_context.StrokeStyle = "#fff";
			_context.Stroke();
		}

		void RenderPlayer()
		{
			float playerCenterX = _canvasSize / 2f;
			float playerCenterY = _canvasSize / 2f;

			_context.BeginPath();
			_context.MoveTo(playerCenterX, playerCenterY + 3);
			_context.LineTo(playerCenterX + 3, playerCenterY);
			_context.LineTo(playerCenterX, playerCenterY - 3);
			_context.LineTo(playerCenterX - 3, playerCenterY);
			_context.ClosePath();

			_context.FillStyle = "#f00";
			_context.Fill();

			_context.StrokeStyle = "#fff";
			_context.Stroke();
		}
	}

	[JSInvokable]
	public async ValueTask OnMouseMove(int mouseX, int mouseY)
	{
		BoundingClientRect canvasBoundingClientRect = await JsRuntime.InvokeAsync<BoundingClientRect>("getBoundingClientRect", _canvasReference);

		_canvasMouseX = mouseX - canvasBoundingClientRect.Left;
		_canvasMouseY = mouseY - canvasBoundingClientRect.Top;

		int x = Math.Clamp((int)(_canvasMouseX / _tileSize), 0, 50);
		int y = Math.Clamp((int)(_canvasMouseY / _tileSize), 0, 50);
		float height = SpawnsetBinary.ArenaTiles[x, y];
		float actualHeight = SpawnsetBinary.GetActualTileHeight(x, y, CurrentTime);

		_spawnsetArenaHoverInfo.Update(x, y, height, actualHeight);
	}
}
