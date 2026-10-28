using System;
using UnityEngine;
using stick.plugins.playermanager.Recorder;

namespace stick.plugins.playermanager.Gui;

public class PlayerRowButtonWidget : IPlayerRowWidget
{
	public string Name { get; set; }

	public Func<PlayerInfo, bool> Condition { get; set; }

	public Func<PlayerInfo, string> GetText { get; set; }

	public Action<PlayerInfo> OnClick { get; set; }

	public float Width { get; set; }

	public GUIStyle Style { get; set; }

	public Func<PlayerInfo, GUIStyle> GetStyle { get; set; }

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
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		GUIStyle val = GetStyle?.Invoke(player) ?? Style ?? GUIStyles.Button;
		if (GUI.Button(rect, GetText?.Invoke(player) ?? string.Empty, val))
		{
			OnClick?.Invoke(player);
		}
	}
}
