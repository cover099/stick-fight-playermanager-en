using System;
using UnityEngine;
using stick.plugins.playermanager.Recorder;

namespace stick.plugins.playermanager.Gui;

public class PlayerRowLabelWidget : IPlayerRowWidget
{
	public string Name { get; set; }

	public Func<PlayerInfo, bool> Condition { get; set; }

	public Func<PlayerInfo, string> GetText { get; set; }

	public float Width { get; set; }

	public GUIStyle Style { get; set; }

	public bool ShouldDraw(PlayerInfo player)
	{
		if (Condition != null)
		{
			return Condition(player);
		}
		return true;
	}

	public void Draw(Rect rect, PlayerInfo player)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		GUI.Label(rect, GetText?.Invoke(player) ?? string.Empty, Style ?? GUIStyles.Label);
	}
}
