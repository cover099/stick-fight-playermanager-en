using System;
using UnityEngine;

namespace stick.plugins.playermanager.Gui;

public class HeaderButtonWidget : IHeaderWidget
{
	public string Name { get; set; }

	public Func<bool> Condition { get; set; }

	public string Text { get; set; }

	public Action Action { get; set; }

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
		if (GUI.Button(rect, Text ?? string.Empty, Style ?? GUIStyles.Button))
		{
			Action?.Invoke();
		}
	}
}
