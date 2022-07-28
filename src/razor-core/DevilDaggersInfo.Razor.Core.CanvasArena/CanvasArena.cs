using DevilDaggersInfo.Razor.Core.Canvas;
using Microsoft.JSInterop;

namespace DevilDaggersInfo.Razor.Core.CanvasArena;

public class CanvasArena : Canvas2d
{
	public CanvasArena(IJSUnmarshalledRuntime runtime, string id)
		: base(runtime, id)
	{
	}

	public void DrawTile(int x, int y, int r, int g, int b, float tileSize)
		=> Invoke("window.drawTile", x, y, r, g, b, tileSize);
}
