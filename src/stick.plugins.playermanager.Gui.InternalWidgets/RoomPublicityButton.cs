namespace stick.plugins.playermanager.Gui.InternalWidgets;

public static class RoomPublicityButton
{
	public static IHeaderWidget Create()
	{
		return new HeaderButtonWidget
		{
			Name = "RoomPublicityButton",
			Text = "Toggle Visibility",
			Action = () =>
			{
				PlayerManager.ToggleLobbyPublicity();
			},
			Width = 150f
		};
	}
}
