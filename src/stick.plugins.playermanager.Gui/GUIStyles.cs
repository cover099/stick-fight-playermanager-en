using UnityEngine;

namespace stick.plugins.playermanager.Gui;

public static class GUIStyles
{
	public static GUIStyle Label;

	public static GUIStyle Button;

	public static GUIStyle ButtonPressed;

	public static GUIStyle YellowButton;

	public static GUIStyle RedButton;

	public static GUIStyle GreenButton;

	private static float _lastScale = -1f;

	public static void Initialize(float scale = 1f)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected Obj, but got Unknown
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected Obj, but got Unknown
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected Obj, but got Unknown
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Expected Obj, but got Unknown
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Expected Obj, but got Unknown
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Expected Obj, but got Unknown
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		if (!Mathf.Approximately(_lastScale, scale))
		{
			int fontSize = Mathf.RoundToInt(22f * scale);
			Label = new GUIStyle(GUI.skin.label)
			{
				fontSize = fontSize
			};
			Button = new GUIStyle(GUI.skin.button)
			{
				fontSize = fontSize,
				richText = false
			};
			ButtonPressed = new GUIStyle(GUI.skin.button)
			{
				fontSize = fontSize,
				richText = false
			};
			ButtonPressed.normal.textColor = Color.grey;
			YellowButton = new GUIStyle(GUI.skin.button)
			{
				fontSize = fontSize,
				richText = false
			};
			YellowButton.normal.textColor = Color.yellow;
			RedButton = new GUIStyle(GUI.skin.button)
			{
				fontSize = fontSize,
				richText = false
			};
			RedButton.normal.textColor = Color.red;
			GreenButton = new GUIStyle(GUI.skin.button)
			{
				fontSize = fontSize,
				richText = false
			};
			GreenButton.normal.textColor = Color.green;
			_lastScale = scale;
		}
	}
}
