using System;
using HarmonyLib;
using Steamworks;

namespace stick.plugins.playermanager.Patches;

[HarmonyPatch(typeof(MatchmakingHandler))]
internal class ListenForChange
{
	[HarmonyPostfix]
	[HarmonyPatch("OnLobbyEnter", new Type[]
	{
		typeof(LobbyEnter_t),
		typeof(bool)
	})]
	public static void OnLobbyEnterPostfix()
	{
		PlayerManager.OnLobbyMembersChanged();
	}

	[HarmonyPostfix]
	[HarmonyPatch("OnLobbyDataUpdate", new Type[] { typeof(LobbyDataUpdate_t) })]
	public static void OnLobbyDataUpdatePostfix()
	{
		PlayerManager.OnLobbyMembersChanged();
	}

	[HarmonyPostfix]
	[HarmonyPatch("OnLobbyChatUpdate", new Type[] { typeof(LobbyChatUpdate_t) })]
	public static void OnLobbyChatUpdatePostfix()
	{
		PlayerManager.OnLobbyMembersChanged();
	}
}
