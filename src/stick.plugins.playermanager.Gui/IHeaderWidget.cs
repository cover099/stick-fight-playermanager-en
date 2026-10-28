using UnityEngine;

namespace stick.plugins.playermanager.Gui;

public interface IHeaderWidget
{
	string Name { get; }

	float Width { get; }

	bool ShouldDraw();

	void Draw(Rect rect);
}
