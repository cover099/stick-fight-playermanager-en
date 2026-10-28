using Steamworks;
using UnityEngine;
using stick.plugins.playermanager.Recorder;

namespace stick.plugins.playermanager.Gui.InternalWidgets;

public static class PlayerInfoButton
{
	public static GUIStyle GetStatusStyle(PlayerInfo player)
	{
		return player.RecordStatus switch
		{
			PlayerRecordStatus.Blacklisted => GUIStyles.RedButton, 
			PlayerRecordStatus.Recorded => GUIStyles.YellowButton, 
			_ => player.IsFriend ? GUIStyles.GreenButton : GUIStyles.Button, 
		};
	}

	public static IPlayerRowWidget Create()
	{
		return new PlayerRowButtonWidget
		{
			Name = "PlayerInfo",
			GetText = (PlayerInfo player) => player.ToString(),
			OnClick = (PlayerInfo player) =>
			{
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				GUIUtility.systemCopyBuffer = player.ToString();
				SteamFriends.ActivateGameOverlayToUser("steamid", player.SteamId);
			},
			Width = 500f,
			GetStyle = GetStatusStyle
		};
	}
}
