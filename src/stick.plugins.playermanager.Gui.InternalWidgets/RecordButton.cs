using UnityEngine;
using stick.plugins.playermanager.Recorder;

namespace stick.plugins.playermanager.Gui.InternalWidgets;

public static class RecordButton
{
	public static IPlayerRowWidget Create()
	{
		return new PlayerRowButtonWidget
		{
			Name = "Record",
			GetText = (PlayerInfo player) => player.RecordStatus switch
			{
				PlayerRecordStatus.NotRecorded => "Record", 
				PlayerRecordStatus.Recorded => "Blacklist", 
				PlayerRecordStatus.Blacklisted => "Cancel", 
				_ => "Unknown", 
			},
			OnClick = (PlayerInfo player) =>
			{
				switch (player.RecordStatus)
				{
				case PlayerRecordStatus.NotRecorded:
					PlayerManager.playerRecorder.RecordPlayer(player);
					break;
				case PlayerRecordStatus.Recorded:
					PlayerManager.blackListRecorder.RecordPlayer(player);
					break;
				case PlayerRecordStatus.Blacklisted:
					PlayerManager.playerRecorder.CancelRecordPlayer(player);
					PlayerManager.blackListRecorder.CancelRecordPlayer(player);
					break;
				default:
					Debug.LogError((object)$"Unknown record status: {player.RecordStatus}");
					return;
				}
				PlayerManager.OnLobbyMembersChanged();
			},
			Width = 100f
		};
	}
}
