using HarmonyLib;

namespace stick.plugins.playermanager.Patches;

[HarmonyPatch(typeof(NetworkPlayer), "SyncClientChat")]
internal class MutePatch
{
	public static bool Prefix(NetworkPlayer __instance)
	{
		if (PlayerManager.MutedPlayers.Contains(__instance.NetworkSpawnID))
		{
			return false;
		}
		return true;
	}
}
