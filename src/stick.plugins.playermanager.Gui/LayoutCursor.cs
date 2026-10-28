using UnityEngine;

namespace stick.plugins.playermanager.Gui;

public class LayoutCursor
{
	private const float RawOffsetY = 20f;

	private const float RawLineHeight = 50f;

	private float _currentX;

	private int _currentLine;

	public float Scale { get; set; } = 1f;

	public float OffsetX { get; set; } = 5f;

	public float OffsetY => 20f * Scale;

	public float LineHeight => 50f * Scale;

	public float CurrentY => OffsetY + LineHeight * (float)_currentLine;

	public void Reset()
	{
		_currentX = OffsetX;
		_currentLine = 0;
	}

	public void BeginRow()
	{
		_currentLine++;
		_currentX = OffsetX;
	}

	public Rect NextColumn(float width, float height)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		Rect result = new Rect(_currentX, CurrentY, width * Scale, height * Scale);
		_currentX += width * Scale;
		return result;
	}

	public void EndRow()
	{
		_currentX = OffsetX;
	}

	public Rect NextLine(float width, float height)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		_currentLine++;
		_currentX = OffsetX;
		return new Rect(OffsetX, CurrentY, width * Scale, height * Scale);
	}
}
