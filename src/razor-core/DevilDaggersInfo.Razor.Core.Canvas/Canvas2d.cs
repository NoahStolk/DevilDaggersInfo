using Microsoft.JSInterop;

namespace DevilDaggersInfo.Razor.Core.Canvas;

/// <summary>
/// Provides invoking methods to the 2D context of the HTML5 canvas.
/// </summary>
public class Canvas2d : Canvas
{
	private string? _fillStyle;
	private string? _strokeStyle;
	private string? _font;
	private TextAlign? _textAlign;
	private TextBaseline? _textBaseline;
	private float? _lineWidth;
	private LineCap? _lineCap;
	private float? _miterLimit;
	private float? _globalAlpha;
	private CompositeOperation? _globalCompositeOperation;
	private bool? _imageSmoothingEnabled;
	private float? _shadowBlur;
	private string? _shadowColor;
	private float? _shadowOffsetX;
	private float? _shadowOffsetY;

	public Canvas2d(IJSUnmarshalledRuntime runtime, string id)
		: base(runtime, id)
	{
	}

	public string FillStyle
	{
		get => _fillStyle ??= InvokeReturn<string>("c2d.getFillStyle");
		set
		{
			if (_fillStyle == value)
				return;

			_fillStyle = value;
			Invoke("c2d.setFillStyle", value);
		}
	}

	public string StrokeStyle
	{
		get => _strokeStyle ??= InvokeReturn<string>("c2d.getStrokeStyle");
		set
		{
			if (_strokeStyle == value)
				return;

			_strokeStyle = value;
			Invoke("c2d.setStrokeStyle", value);
		}
	}

	public string Font
	{
		get => _font ??= InvokeReturn<string>("c2d.getFont");
		set
		{
			if (_font == value)
				return;

			_font = value;
			Invoke("c2d.setFont", value);
		}
	}

	public TextAlign TextAlign
	{
		get
		{
			if (_textAlign == null)
			{
				string str = InvokeReturn<string>("c2d.getTextAlign");
				Enum.TryParse(str, true, out TextAlign ret);
				_textAlign = ret;
			}

			return _textAlign.Value;
		}
		set
		{
			if (_textAlign == value)
				return;

			_textAlign = value;
			Invoke("c2d.setTextAlign", value.ToString().ToLower());
		}
	}

	public TextBaseline TextBaseline
	{
		get
		{
			if (_textBaseline == null)
			{
				string str = InvokeReturn<string>("c2d.getTextBaseline");
				Enum.TryParse(str, true, out TextBaseline ret);
				_textBaseline = ret;
			}

			return _textBaseline.Value;
		}
		set
		{
			if (_textBaseline == value)
				return;

			_textBaseline = value;
			Invoke("c2d.setTextBaseline", value.ToString().ToLower());
		}
	}

	public float LineWidth
	{
		get
		{
			if (_lineWidth == null)
			{
				string str = InvokeReturn<string>("c2d.getLineWidth");
				_ = float.TryParse(str, out float width);
				_lineWidth = width;
			}

			return _lineWidth.Value;
		}
		set
		{
			if (_lineWidth == value)
				return;

			_lineWidth = value;
			Invoke("c2d.setLineWidth", value);
		}
	}

	public LineCap LineCap
	{
		get
		{
			if (_lineCap == null)
			{
				string str = InvokeReturn<string>("c2d.getLineCap");
				Enum.TryParse(str, true, out LineCap ret);
				_lineCap = ret;
			}

			return _lineCap.Value;
		}
		set
		{
			if (_lineCap == value)
				return;

			_lineCap = value;
			Invoke("c2d.setLineCap", value.ToString().ToLower());
		}
	}

	public float MiterLimit
	{
		get
		{
			if (_miterLimit == null)
			{
				string str = InvokeReturn<string>("c2d.getMiterLimit");
				_ = float.TryParse(str, out float ga);
				_miterLimit = ga;
			}

			return _miterLimit.Value;
		}
		set
		{
			if (_miterLimit == value)
				return;

			_miterLimit = value;
			Invoke("c2d.setMiterLimit", value);
		}
	}

	public float GlobalAlpha
	{
		get
		{
			if (_globalAlpha == null)
			{
				string str = InvokeReturn<string>("c2d.getGlobalAlpha");
				_ = float.TryParse(str, out float ga);
				_globalAlpha = ga;
			}

			return _globalAlpha.Value;
		}
		set
		{
			if (_globalAlpha == value)
				return;

			_globalAlpha = value;
			Invoke("c2d.setGlobalAlpha", value);
		}
	}

	public CompositeOperation GlobalCompositeOperation
	{
		get
		{
			if (_globalCompositeOperation == null)
			{
				string? str = InvokeReturn<string>("c2d.getGlobalCompositeOperation")?.Replace('-', '_');
				Enum.TryParse(str, true, out CompositeOperation ret);
				_globalCompositeOperation = ret;
			}

			return _globalCompositeOperation.Value;
		}
		set
		{
			if (_globalCompositeOperation == value)
				return;

			_globalCompositeOperation = value;
			Invoke("c2d.setGlobalCompositeOperation", value.ToString().ToLower().Replace('_', '-'));
		}
	}

	public bool ImageSmoothingEnabled
	{
		get => _imageSmoothingEnabled ??= InvokeReturn<bool>("c2d.getImageSmoothingEnabled");
		set
		{
			if (_imageSmoothingEnabled == value)
				return;

			_imageSmoothingEnabled = value;
			Invoke("c2d.setImageSmoothingEnabled", value ? 1 : 0);
		}
	}

	public float ShadowBlur
	{
		get
		{
			if (_shadowBlur == null)
			{
				string str = InvokeReturn<string>("c2d.getShadowBlur");
				_ = float.TryParse(str, out float shadowBlur);
				_shadowBlur = shadowBlur;
			}

			return _shadowBlur.Value;
		}
		set
		{
			if (_shadowBlur == value)
				return;
			_shadowBlur = value;
			Invoke("c2d.setShadowBlur", value);
		}
	}

	public string ShadowColor
	{
		get => _shadowColor ??= InvokeReturn<string>("c2d.getShadowColor");
		set
		{
			if (_shadowColor == value)
				return;

			_shadowColor = value;
			Invoke("c2d.setShadowColor", value);
		}
	}

	public float ShadowOffsetX
	{
		get
		{
			if (_shadowOffsetX == null)
			{
				string str = InvokeReturn<string>("c2d.getShadowOffsetX");
				_ = float.TryParse(str, out float sb);
				_shadowOffsetX = sb;
			}

			return _shadowOffsetX.Value;
		}
		set
		{
			if (_shadowOffsetX == value)
				return;

			_shadowOffsetX = value;
			Invoke("c2d.setShadowOffsetX", value);
		}
	}

	public float ShadowOffsetY
	{
		get
		{
			if (_shadowOffsetY == null)
			{
				string str = InvokeReturn<string>("c2d.getShadowOffsetY");
				_ = float.TryParse(str, out float sb);
				_shadowOffsetY = sb;
			}

			return _shadowOffsetY.Value;
		}
		set
		{
			if (_shadowOffsetY == value)
				return;

			_shadowOffsetY = value;
			Invoke("c2d.setShadowOffsetY", value);
		}
	}

	public void ClearRect(double x, double y, double width, double height)
		=> ClearRect((float)x, (float)y, (float)width, (float)height);

	// TODO: Use in .NET 7? [System.Runtime.InteropServices.JavaScript.JSImport("c2d.clearRect")]
	public void ClearRect(float x, float y, float width, float height)
		=> Invoke("c2d.clearRect", x, y, width, height);

	public void FillRect(double x, double y, double width, double height)
		=> FillRect((float)x, (float)y, (float)width, (float)height);

	public void FillRect(float x, float y, float width, float height)
		=> Invoke("c2d.fillRect", x, y, width, height);

	public void StrokeRect(double x, double y, double width, double height)
		=> StrokeRect((float)x, (float)y, (float)width, (float)height);

	public void StrokeRect(float x, float y, float width, float height)
		=> Invoke("c2d.strokeRect", x, y, width, height);

	public void FillText(string text, double x, double y, double? maxWidth = null)
	{
		float? fmw = null;
		if (maxWidth != null)
			fmw = (float)maxWidth.Value;
		FillText(text, (float)x, (float)y, fmw);
	}

	public void FillText(string text, float x, float y, float? maxWidth = null)
	{
		if (maxWidth == null)
			Invoke("c2d.fillText", text, x, y);
		else
			Invoke("c2d.fillText1", text, x, y, maxWidth.Value);
	}

	public void StrokeText(string text, double x, double y, double? maxWidth = null)
	{
		float? fmw = null;
		if (maxWidth != null)
			fmw = (float)maxWidth.Value;
		StrokeText(text, (float)x, (float)y, fmw);
	}

	public void StrokeText(string text, float x, float y, float? maxWidth = null)
	{
		if (maxWidth == null)
			Invoke("c2d.strokeText", text, x, y);
		else
			Invoke("c2d.strokeText1", text, x, y, maxWidth.Value);
	}

	public float MeasureText(string text)
	{
		string ret = InvokeReturn<string, string>("c2d.measureText", text);
		_ = float.TryParse(ret, out float width);
		return width;
	}

	public float[] GetLineDash()
	{
		string str = InvokeReturn<string>("c2d.getLineDash");
		if (string.IsNullOrEmpty(str))
			return Array.Empty<float>();

		string[] values = str.Split(',');
		int count = 0;
		foreach (string value in values)
		{
			if (float.TryParse(value, out _))
				count++;
		}

		if (count == 0)
			return Array.Empty<float>();

		float[] lineDash = new float[count];
		count = 0;
		foreach (string ds in values)
		{
			if (float.TryParse(ds, out float tmp))
			{
				lineDash[count] = tmp;
				count++;
			}
		}

		return lineDash;
	}

	public void SetLineDash(float[] segments)
		=> Invoke("c2d.setLineDash", segments);

	public void BeginPath()
		=> Invoke("c2d.beginPath");

	public void ClosePath()
		=> Invoke("c2d.closePath");

	public bool IsPointInPath(double x, double y, bool evenodd = false)
		=> IsPointInPath((float)x, (float)y, evenodd);

	public bool IsPointInPath(float x, float y, bool evenodd = false)
		=> InvokeReturn<float, float, int, bool>("c2d.isPointInPath", x, y, evenodd ? 1 : 0);

	public bool IsPointInStroke(double x, double y)
		=> IsPointInStroke((float)x, (float)y);

	public bool IsPointInStroke(float x, float y)
		=> InvokeReturn<float, float, bool>("c2d.isPointInStroke", x, y);

	public void MoveTo(double x, double y)
		=> MoveTo((float)x, (float)y);

	public void MoveTo(float x, float y)
		=> Invoke("c2d.moveTo", x, y);

	public void LineTo(double x, double y)
		=> LineTo((float)x, (float)y);

	public void LineTo(float x, float y)
		=> Invoke("c2d.lineTo", x, y);

	public void BezierCurveTo(double cp1X, double cp1Y, double cp2X, double cp2Y, double x, double y)
		=> BezierCurveTo((float)cp1X, (float)cp1Y, (float)cp2X, (float)cp2Y, (float)x, (float)y);

	public void BezierCurveTo(float cp1X, float cp1Y, float cp2X, float cp2Y, float x, float y)
		=> Invoke("c2d.bezierCurveTo", cp1X, cp1Y, cp2X, cp2Y, x, y);

	public void QuadraticCurveTo(double cpx, double cpy, double x, double y)
		=> QuadraticCurveTo((float)cpx, (float)cpy, (float)x, (float)y);

	public void QuadraticCurveTo(float cpx, float cpy, float x, float y)
		=> Invoke("c2d.quadraticCurveTo", cpx, cpy, x, y);

	public void Arc(double x, double y, double radius, double startAngle, double endAngle, bool anticlockwise = false)
		=> Arc((float)x, (float)y, (float)radius, (float)startAngle, (float)endAngle, anticlockwise);

	public void Arc(float x, float y, float radius, float startAngle, float endAngle, bool anticlockwise = false)
		=> Invoke("c2d.arc", x, y, radius, startAngle, endAngle, anticlockwise ? 1 : 0);

	public void Circle(double x, double y, double radius)
		=> Circle((float)x, (float)y, (float)radius);

	public void Circle(float x, float y, float radius)
		=> Arc(x, y, radius, 0.0f, (float)(Math.PI * 2));

	public void ArcTo(double x1, double y1, double x2, double y2, double radius)
		=> ArcTo((float)x1, (float)y1, (float)x2, (float)y2, (float)radius);

	public void ArcTo(float x1, float y1, float x2, float y2, float radius)
		=> Invoke("c2d.arcTo", x1, y1, x2, y2, radius);

	public void Rect(double x, double y, double width, double height)
		=> Rect((float)x, (float)y, (float)width, (float)height);

	public void Rect(float x, float y, float width, float height)
		=> Invoke("c2d.rect", x, y, width, height);

	public void Ellipse(double x, double y, double radiusX, double radiusY, double rotation = 0, double startAngle = 0, double endAngle = Math.PI * 2, bool anticlockwise = false)
		=> Ellipse((float)x, (float)y, (float)radiusX, (float)radiusY, (float)rotation, (float)startAngle, (float)endAngle, anticlockwise);

	public void Ellipse(float x, float y, float radiusX, float radiusY, float rotation = 0f, float startAngle = 0f, float endAngle = (float)(Math.PI * 2), bool anticlockwise = false)
		=> Invoke("c2d.ellipse", x, y, radiusX, radiusY, rotation, startAngle, endAngle, anticlockwise ? 1 : 0);

	public void Fill()
		=> Invoke("c2d.fill");

	public void Stroke()
		=> Invoke("c2d.stroke");

	public void Clip()
		=> Invoke("c2d.clip");

	public void Rotate(double angle)
		=> Rotate((float)angle);

	public void Rotate(float angle)
		=> Invoke("c2d.rotate", angle);

	public void RotateAt(double x, double y, double angle)
		=> RotateAt((float)x, (float)y, (float)angle);

	public void RotateAt(float x, float y, float angle)
		=> Invoke("c2d.rotateAt", x, y, angle);

	public void Scale(double x, double y)
		=> Scale((float)x, (float)y);

	public void Scale(float x, float y)
		=> Invoke("c2d.scale", x, y);

	public void Translate(double x, double y)
		=> Translate((float)x, (float)y);

	public void Translate(float x, float y)
		=> Invoke("c2d.translate", x, y);

	public void Transform(double m11, double m12, double m21, double m22, double dx, double dy)
		=> Transform((float)m11, (float)m12, (float)m21, (float)m22, (float)dx, (float)dy);

	public void Transform(float m11, float m12, float m21, float m22, float dx, float dy)
		=> Invoke("c2d.transform", m11, m12, m21, m22, dx, dy);

	public void SetTransform(double m11, double m12, double m21, double m22, double dx, double dy)
		=> SetTransform((float)m11, (float)m12, (float)m21, (float)m22, (float)dx, (float)dy);

	public void SetTransform(float m11, float m12, float m21, float m22, float dx, float dy)
		=> Invoke("c2d.setTransform", m11, m12, m21, m22, dx, dy);

	public void ResetTransform()
		=> Invoke("c2d.resetTransform");

	public void Save()
		=> Invoke("c2d.save");

	public void Restore()
		=> Invoke("c2d.restore");
}
