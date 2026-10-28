using HarmonyLib;
using Steamworks;
using UnityEngine;

namespace stick.plugins.playermanager.Patches;

[HarmonyPatch(typeof(MultiplayerManager), "AddClientToList")]
internal class ApplyBlacklist
{
	public static bool Prefix(P2PPackageHandler ___mPacketHandler, ref CSteamID newClient)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if (PlayerManager.blackListRecorder.IsPlayerRecorded(newClient))
		{
			Debug.Log((object)"Hey this guy is in the blacklist, refused");
			Debug.Log((object)("His name : " + ((object)newClient/*cast due to constrained. prefix*/).ToString() + SteamFriends.GetFriendPersonaName(newClient)));
			byte[] array = new byte[1];
			___mPacketHandler.SendP2PPacketToUser(newClient, array, (P2PPackageHandler.MsgType)5, (EP2PSend)2, 0);
			return false;
		}
		return true;
	}
}
