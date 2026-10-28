using System.Collections.Generic;
using stick.plugins.playermanager.Gui.InternalWidgets;

namespace stick.plugins.playermanager.Gui;

public static class DefaultWidgets
{
	public static void RegisterAll()
	{
		List<IHeaderWidget> list = new List<IHeaderWidget>();
		if (PlayerManager.ShowRoomPublicityLabel.Value)
		{
			list.Add(RoomPublicityLabel.Create());
		}
		if (PlayerManager.ShowRoomPublicityButton.Value)
		{
			list.Add(RoomPublicityButton.Create());
		}
		if (list.Count > 0)
		{
			RegisterManager.RegisterHeaderRow(new HeaderRow
			{
				Name = "RoomPublicity",
				Condition = () => PlayerManager.IsHost(PlayerManager.LocalPlayer),
				Children = list
			});
		}
		RegisterManager.RegisterPlayerRow(PlayerInfoButton.Create());
		if (PlayerManager.ShowRecordButton.Value)
		{
			RegisterManager.RegisterPlayerRow(RecordButton.Create());
		}
		if (PlayerManager.ShowKickButton.Value)
		{
			RegisterManager.RegisterPlayerRow(KickButton.Create());
		}
		if (PlayerManager.ShowNameToggleButton.Value)
		{
			RegisterManager.RegisterPlayerRow(NameToggleButton.Create());
		}
		if (PlayerManager.ShowTransButton.Value)
		{
			RegisterManager.RegisterPlayerRow(TransButton.Create());
		}
		if (PlayerManager.ShowMuteButton.Value)
		{
			RegisterManager.RegisterPlayerRow(MuteButton.Create());
		}
		if (PlayerManager.ShowAddFriendButton.Value)
		{
			RegisterManager.RegisterPlayerRow(AddFriendButton.Create());
		}
	}
}
