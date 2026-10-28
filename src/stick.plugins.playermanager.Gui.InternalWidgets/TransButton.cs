using stick.plugins.playermanager.Recorder;

namespace stick.plugins.playermanager.Gui.InternalWidgets;

public static class TransButton
{
	public static IPlayerRowWidget Create()
	{
		return new PlayerRowButtonWidget
		{
			Name = "Trans Host",
			Condition = (PlayerInfo player) => PlayerManager.IsHost(PlayerManager.LocalPlayer) && !PlayerManager.IsLocalPlayer(player),
			GetText = (PlayerInfo _) => "Make Host",
			OnClick = (PlayerInfo player) =>
			{
				PlayerManager.TransferLobbyOwnership(player);
			},
			Width = 100f
		};
	}
}
