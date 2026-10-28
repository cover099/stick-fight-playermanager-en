using stick.plugins.playermanager.Recorder;

namespace stick.plugins.playermanager.Gui.InternalWidgets;

public static class NameToggleButton
{
	public static IPlayerRowWidget Create()
	{
		return new PlayerRowButtonWidget
		{
			Name = "NameToggle",
			Condition = (PlayerInfo player) => player.HasRecordedName,
			GetText = (PlayerInfo player) => (!player.DisplayNameRecorded) ? "Show Old Name" : "Show Current Name",
			OnClick = (PlayerInfo player) =>
			{
				player.DisplayNameRecorded = !player.DisplayNameRecorded;
			},
			Width = 120f
		};
	}
}
