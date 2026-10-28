using System;
using System.Collections.Generic;

namespace stick.plugins.playermanager.Gui;

public class HeaderRow : IHeaderRow
{
	public string Name { get; set; }

	public Func<bool> Condition { get; set; }

	public List<IHeaderWidget> Children { get; set; } = new List<IHeaderWidget>();

	IList<IHeaderWidget> IHeaderRow.Children => Children;

	public bool ShouldDraw()
	{
		if (Condition != null)
		{
			return Condition();
		}
		return true;
	}
}
