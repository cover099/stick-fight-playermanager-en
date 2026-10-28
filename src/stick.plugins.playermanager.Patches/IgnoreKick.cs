using HarmonyLib;
using Steamworks;
using UnityEngine;
using stick.plugins.playermanager.Recorder;

namespace stick.plugins.playermanager.Patches;

[HarmonyPatch(typeof(P2PPackageHandler), "CheckMessageType")]
internal class IgnoreKick
{
	public static bool Prefix(byte[] data, P2PPackageHandler.MsgType type, CSteamID steamIdRemote)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Invalid comparison between Unknown and I4
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if ((int)type != 38)
		{
			return true;
		}
		PlayerInfo playerInfo = new PlayerInfo(steamIdRemote);
		Debug.LogWarning((object)("Received Kick Message, from: " + playerInfo));
		PlayerManager.LocalChat.Talk("Blocked kick sent by: " + playerInfo.CurrentName + " (Blacklisted)");
		return false;
	}
}
