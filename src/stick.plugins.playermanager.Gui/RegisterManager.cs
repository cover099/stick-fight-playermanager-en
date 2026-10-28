using System.Collections.Generic;
using UnityEngine;

namespace stick.plugins.playermanager.Gui;

public static class RegisterManager
{
	public static List<IHeaderRow> HeaderRows { get; } = new List<IHeaderRow>();

	public static List<IPlayerRowWidget> PlayerRowWidgets { get; } = new List<IPlayerRowWidget>();

	public static void RegisterHeaderRow(IHeaderRow row)
	{
		HeaderRows.Add(row);
		Debug.Log((object)("[PlayerManager] Registered header row: " + row.Name));
	}

	public static void RegisterPlayerRow(IPlayerRowWidget widget)
	{
		PlayerRowWidgets.Add(widget);
		Debug.Log((object)("[PlayerManager] Registered player row widget: " + widget.Name));
	}

	public static bool UnregisterHeaderRow(string name)
	{
		int num = HeaderRows.RemoveAll((IHeaderRow r) => r.Name == name);
		if (num > 0)
		{
			Debug.Log((object)("[PlayerManager] Removed header row: " + name));
		}
		return num > 0;
	}

	public static bool UnregisterPlayerRow(string name)
	{
		int num = PlayerRowWidgets.RemoveAll((IPlayerRowWidget w) => w.Name == name);
		if (num > 0)
		{
			Debug.Log((object)("[PlayerManager] Removed player row widget: " + name));
		}
		return num > 0;
	}

	public static void ClearHeaderRows()
	{
		HeaderRows.Clear();
		Debug.Log((object)"[PlayerManager] Cleared all header rows");
	}

	public static void ClearPlayerRowWidgets()
	{
		PlayerRowWidgets.Clear();
		Debug.Log((object)"[PlayerManager] Cleared all player row widgets");
	}
}
