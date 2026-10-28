using System.Collections.Generic;
using Steamworks;
using UnityEngine;
using stick.plugins.playermanager.Recorder;

namespace stick.plugins.playermanager.Gui.InternalWidgets;

public static class RemoveRecordButton
{
	private static bool _hasPending;

	private static bool _pendingIsBlacklist;

	private static CSteamID _pendingId;

	public static void ClearPending()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		_hasPending = false;
		_pendingId = CSteamID.Nil;
	}

	public static void ClearPendingIfGone(IList<PlayerInfo> entries)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (!_hasPending)
		{
			return;
		}
		for (int i = 0; i < entries.Count; i++)
		{
			if (entries[i].SteamId == _pendingId)
			{
				return;
			}
		}
		ClearPending();
	}

	public static bool Draw(Rect rect, PlayerInfo entry, bool isBlacklist)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		bool flag = _hasPending && _pendingIsBlacklist == isBlacklist && _pendingId == entry.SteamId;
		if (GUI.Button(rect, flag ? "Confirm?" : "Remove", GUIStyles.Button))
		{
			if (flag)
			{
				(isBlacklist ? ((RecorderBase)PlayerManager.blackListRecorder) : ((RecorderBase)PlayerManager.playerRecorder)).CancelRecordPlayer(entry);
				ClearPending();
				return true;
			}
			_hasPending = true;
			_pendingIsBlacklist = isBlacklist;
			_pendingId = entry.SteamId;
		}
		return false;
	}
}
