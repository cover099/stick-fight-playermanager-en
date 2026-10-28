using stick.plugins.playermanager.Recorder;

namespace stick.plugins.playermanager.Gui.InternalWidgets;

public static class KickButton
{
	public static IPlayerRowWidget Create()
	{
		return new PlayerRowButtonWidget
		{
			Name = "Kick",
			Condition = (PlayerInfo player) => !PlayerManager.IsHost(player) && !PlayerManager.IsLocalPlayer(player),
			GetText = (PlayerInfo _) => "Kick!",
			OnClick = (PlayerInfo player) =>
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				PlayerManager.KickPlayer(player.SteamId, (MultiplayerManager.KickResponse)0);
			},
			Width = 100f
		};
	}
}
