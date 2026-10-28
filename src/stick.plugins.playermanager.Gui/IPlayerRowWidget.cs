using UnityEngine;
using stick.plugins.playermanager.Recorder;

namespace stick.plugins.playermanager.Gui;

public interface IPlayerRowWidget
{
	string Name { get; }

	float Width { get; }

	bool ShouldDraw(PlayerInfo player);

	void Draw(Rect rect, PlayerInfo player);
}
