using System;
using UnityEngine;

namespace stick.plugins.playermanager.Gui;

public class HeaderLabelWidget : IHeaderWidget
{
	public string Name { get; set; }

	public Func<bool> Condition { get; set; }

	public Func<string> Content { get; set; }

	public float Width { get; set; }

	public GUIStyle Style { get; set; }

	public bool ShouldDraw()
	{
		if (Condition != null)
		{
			return Condition();
		}
		return true;
	}

	public void Draw(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		GUI.Label(rect, Content?.Invoke() ?? string.Empty, Style ?? GUIStyles.Label);
	}
}
