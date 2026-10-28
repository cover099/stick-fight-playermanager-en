using System.Collections.Generic;

namespace stick.plugins.playermanager.Gui;

public interface IHeaderRow
{
	string Name { get; }

	IList<IHeaderWidget> Children { get; }

	bool ShouldDraw();
}
