using HarmonyLib;
using Steamworks;
using UnityEngine;
using stick.plugins.playermanager.Recorder;

namespace stick.plugins.playermanager.Patches;

// Stops other players removing you from a lobby (config: General/BlockKicks). Two ways to do it:
//  - the kick message (vanilla Kick, PlayerManager's Kick button): always ignored;
//  - a "client init refused" message (PlayerManager's blacklist / force kick): your game treats it
//    as a failed join and leaves. Ignored once you're in the game, and from anyone but the lobby
//    owner. A host refusing you while you're joining (e.g. lobby full) still goes through, so a
//    refused join fails normally instead of hanging.
// Merged in from the standalone AntiKick plugin.
[HarmonyPatch(typeof(P2PPackageHandler), "CheckMessageType")]
internal class IgnoreKick
{
	// True once the host has let us into the current lobby.
	internal static bool InGame;

	public static bool Prefix(byte[] data, P2PPackageHandler.MsgType type, CSteamID steamIdRemote)
	{
		if (type == P2PPackageHandler.MsgType.KickPlayer)
		{
			Blocked("kick", steamIdRemote);
			return false;
		}
		if (type == P2PPackageHandler.MsgType.ClientInit && data != null && data.Length > 0)
		{
			if (data[0] == 1)
			{
				InGame = true;
				return true;
			}
			CSteamID owner = SteamMatchmaking.GetLobbyOwner(MatchmakingHandler.Instance.CurrentLobby);
			if (InGame || steamIdRemote != owner)
			{
				Blocked("forced removal (refused join)", steamIdRemote);
				return false;
			}
		}
		return true;
	}

	private static void Blocked(string what, CSteamID sender)
	{
		PlayerInfo playerInfo = new PlayerInfo(sender);
		Debug.LogWarning((object)("Blocked a " + what + " from: " + playerInfo));
		PlayerManager.LocalChat?.Talk("Blocked " + what + " sent by: " + playerInfo.CurrentName);
	}
}

// Leaving or switching lobbies: the next lobby has to let us in again.
[HarmonyPatch]
internal class IgnoreKickReset
{
	[HarmonyPostfix]
	[HarmonyPatch(typeof(MultiplayerManager), nameof(MultiplayerManager.OnDisconnected))]
	private static void DisconnectedPostfix()
	{
		IgnoreKick.InGame = false;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(MatchmakingHandler), nameof(MatchmakingHandler.JoinServer))]
	private static void JoinPrefix()
	{
		IgnoreKick.InGame = false;
	}
}
