using Steamworks;
using UnityEngine;
using stick.plugins.playermanager.Recorder;

namespace stick.plugins.playermanager.Gui.InternalWidgets;

public static class AddFriendButton
{
	public static IPlayerRowWidget Create()
	{
		return new PlayerRowButtonWidget
		{
			Name = "Add Friend",
			GetText = (PlayerInfo player) => "Add Friend",
			Condition = (PlayerInfo player) => !PlayerManager.IsLocalPlayer(player) && !player.IsFriend,
			OnClick = (PlayerInfo player) =>
			{
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				GUIUtility.systemCopyBuffer = player.ToString();
				SteamFriends.ActivateGameOverlayToUser("friendadd", player.SteamId);
			},
			Width = 100f
		};
	}
}
