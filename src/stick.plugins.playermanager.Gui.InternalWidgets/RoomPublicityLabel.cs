namespace stick.plugins.playermanager.Gui.InternalWidgets;

public static class RoomPublicityLabel
{
	public static IHeaderWidget Create()
	{
		return new HeaderLabelWidget
		{
			Name = "RoomPublicityLabel",
			Content = () => "Lobby type: " + (PlayerManager.IsPublic ? "Public" : "Friends Only"),
			Width = 200f
		};
	}
}
