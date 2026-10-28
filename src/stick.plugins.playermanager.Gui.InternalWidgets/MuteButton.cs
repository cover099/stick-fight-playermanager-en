using stick.plugins.playermanager.Recorder;

namespace stick.plugins.playermanager.Gui.InternalWidgets;

public static class MuteButton
{
	public static IPlayerRowWidget Create()
	{
		return new PlayerRowButtonWidget
		{
			Name = "mute",
			Condition = (PlayerInfo player) => !PlayerManager.IsLocalPlayer(player),
			GetText = (PlayerInfo player) => (!PlayerManager.IsMuted(player)) ? "Mute" : "Unmute",
			OnClick = (PlayerInfo player) =>
			{
				if (PlayerManager.IsMuted(player))
				{
					PlayerManager.UnmutePlayer(player);
				}
				else
				{
					PlayerManager.MutePlayer(player);
				}
			},
			Width = 100f
		};
	}
}
