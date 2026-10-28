using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Steamworks;
using UnityEngine;
using stick.plugins.playermanager.Gui;
using stick.plugins.playermanager.Gui.InternalWidgets;
using stick.plugins.playermanager.Patches;
using stick.plugins.playermanager.Recorder;

namespace stick.plugins.playermanager;

[BepInPlugin("stick.plugins.playermanager", "PlayerManager", "5.1.0")]
[BepInProcess("StickFight.exe")]
public class PlayerManager : BaseUnityPlugin
{
	internal static ConfigEntry<bool> _blockKicks;

	internal static ConfigEntry<KeyCode> _lobbyInfoToggleKeyCode;

	internal static ConfigEntry<bool> ShowKickButton;

	internal static ConfigEntry<bool> ShowTransButton;

	internal static ConfigEntry<bool> ShowRecordButton;

	internal static ConfigEntry<bool> ShowNameToggleButton;

	internal static ConfigEntry<bool> ShowRoomPublicityLabel;

	internal static ConfigEntry<bool> ShowRoomPublicityButton;

	internal static ConfigEntry<bool> ShowMuteButton;

	internal static ConfigEntry<bool> ShowAddFriendButton;

	internal static ConfigEntry<float> GuiScale;

	internal static ConfigEntry<int> RecentPlayersCount;

	private static readonly CallResult<LobbyMatchList_t> LobbyMatchListCallResult = CallResult<LobbyMatchList_t>.Create((CallResult<LobbyMatchList_t>.APIDispatchDelegate)null);

	public static PlayerRecorder playerRecorder = new PlayerRecorder();

	public static BlackListRecorder blackListRecorder = new BlackListRecorder();

	public static RecentPlayersRecorder recentPlayersRecorder = new RecentPlayersRecorder();

	public static List<PlayerInfo> lobbyMemberList = new List<PlayerInfo>();

	public static List<CSteamID> lobbyDataList = new List<CSteamID>();

	public static HashSet<CSteamID> enteredLobbyList = new HashSet<CSteamID>();

	public static ELobbyDistanceFilter lobbyDistanceFilter = (ELobbyDistanceFilter)1;

	private static readonly object LobbyMemberLock = new object();

	public static PlayerInfo LocalPlayer;

	public static HashSet<ushort> MutedPlayers = new HashSet<ushort>();

	public static ChatManager LocalChat
	{
		get
		{
			if (!((UnityEngine.Object)(object)field == (UnityEngine.Object)null))
			{
				return field;
			}
			return null;
		}
		set;
	}

	public static MultiplayerManager MultiplayerManager => UnityEngine.Object.FindObjectOfType<MultiplayerManager>();

	public static MatchmakingHandler MatchmakingHandler => MatchmakingHandler.Instance;

	public static MethodInfo RawChangeLobbyTypeMI => AccessTools.Method(typeof(MatchmakingHandler), "ChangeLobbyType", (Type[])null, (Type[])null);

	public static bool IsPublic
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Invalid comparison between Unknown and I4
			return (int)MatchmakingHandler.LobbyType == 2;
		}
	}

	public static CSteamID CurrentLobbyID
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			return MatchmakingHandler.CurrentLobby;
		}
	}

	public static PlayerInfo LobbyOwner
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			return new PlayerInfo(SteamMatchmaking.GetLobbyOwner(CurrentLobbyID));
		}
	}

	private IEnumerator InitAfterSteam()
	{
		yield return (object)new WaitForSeconds(2f);
		while (!SteamManager.Initialized)
		{
			yield return null;
		}
		LocalPlayer = new PlayerInfo(SteamUser.GetSteamID());
	}

	private void Awake()
	{
		InitializeConfiguration();
		recentPlayersRecorder.MaxCount = RecentPlayersCount.Value;
		recentPlayersRecorder.ApplyLimit();
		ApplyPatches();
		DefaultWidgets.RegisterAll();
	}

	private void Start()
	{
		((MonoBehaviour)this).StartCoroutine(InitAfterSteam());
	}

	private void Update()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		HandleInput();
		if (SteamManager.Initialized && LocalPlayer == null)
		{
			LocalPlayer = new PlayerInfo(SteamUser.GetSteamID());
		}
	}

	private void OnGUI()
	{
		if (PlayerManagerGUI.ShowLobbyInfo)
		{
			PlayerManagerGUI.DrawLobbyInfoGUI();
		}
	}

	private void InitializeConfiguration()
	{
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Expected Obj, but got Unknown
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Expected Obj, but got Unknown
		_blockKicks = ((BaseUnityPlugin)this).Config.Bind<bool>("General", "BlockKicks", true, "Block kick requests from other players");
		_lobbyInfoToggleKeyCode = ((BaseUnityPlugin)this).Config.Bind<KeyCode>("LobbyInfo", "ToggleKeyCode", (KeyCode)283, "Hotkey to open/close the lobby info window");
		ShowKickButton = ((BaseUnityPlugin)this).Config.Bind<bool>("GUI", "ShowKickButton", true, "Show the Kick button");
		ShowTransButton = ((BaseUnityPlugin)this).Config.Bind<bool>("GUI", "ShowTransButton", true, "Show the Make Host button");
		ShowRecordButton = ((BaseUnityPlugin)this).Config.Bind<bool>("GUI", "ShowRecordButton", true, "Show the Record/Blacklist button");
		ShowNameToggleButton = ((BaseUnityPlugin)this).Config.Bind<bool>("GUI", "ShowNameToggleButton", true, "Show the old/current name toggle button");
		ShowMuteButton = ((BaseUnityPlugin)this).Config.Bind<bool>("GUI", "ShowMuteButton", true, "Show the Mute button");
		ShowRoomPublicityLabel = ((BaseUnityPlugin)this).Config.Bind<bool>("GUI", "ShowRoomPublicityLabel", true, "Show the lobby type label");
		ShowRoomPublicityButton = ((BaseUnityPlugin)this).Config.Bind<bool>("GUI", "ShowRoomPublicityButton", true, "Show the Toggle Visibility button");
		ShowAddFriendButton = ((BaseUnityPlugin)this).Config.Bind<bool>("GUI", "ShowAddFriendButton", true, "Show the Add Friend button");
		GuiScale = ((BaseUnityPlugin)this).Config.Bind<float>("GUI", "GuiScale", 1f, new ConfigDescription("Overall window scale (1.0 = same as older versions at 1440p, about 1/5 of the screen width; about 1.22 = 1/4 of the screen width)", (AcceptableValueBase)(object)new AcceptableValueRange<float>(0.5f, 2f), new object[0]));
		RecentPlayersCount = ((BaseUnityPlugin)this).Config.Bind<int>("GUI", "RecentPlayersCount", 20, new ConfigDescription("Max number of players kept in the recently played with list", (AcceptableValueBase)(object)new AcceptableValueRange<int>(1, 100), new object[0]));
	}

	private void ApplyPatches()
	{
		if (_blockKicks.Value)
		{
			Harmony.CreateAndPatchAll(typeof(IgnoreKick), (string)null);
		}
		Harmony.CreateAndPatchAll(typeof(ApplyBlacklist), (string)null);
		Harmony.CreateAndPatchAll(typeof(ListenForChange), (string)null);
		Harmony.CreateAndPatchAll(typeof(MutePatch), (string)null);
		Harmony.CreateAndPatchAll(typeof(DistanceFilterPatch), (string)null);
		Harmony.CreateAndPatchAll(typeof(GetChatManager), (string)null);
	}

	private void HandleInput()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		if (Input.GetKeyDown(_lobbyInfoToggleKeyCode.Value))
		{
			ToggleLobbyInfoGUI();
		}
	}

	private void ToggleLobbyInfoGUI()
	{
		if (PlayerManagerGUI.ShowLobbyInfo)
		{
			PlayerManagerGUI.ShowLobbyInfo = false;
			RemoveRecordButton.ClearPending();
			Debug.Log((object)"Lobby info window closed");
		}
		else
		{
			RequestLobbyList();
			OnLobbyMembersChanged();
			PlayerManagerGUI.ShowLobbyInfo = true;
			Debug.Log((object)"Lobby info window opened");
		}
	}

	public static void TransferLobbyOwnership(PlayerInfo targetPlayer)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (targetPlayer != null && targetPlayer != LocalPlayer)
		{
			SteamMatchmaking.SetLobbyOwner(CurrentLobbyID, targetPlayer.SteamId);
		}
	}

	public static ushort GetNetworkSpawnIDBySteamID(CSteamID targetSteamID)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		ConnectedClientData[] connectedClients = MultiplayerManager.ConnectedClients;
		for (ushort num = 0; num < connectedClients.Length; num++)
		{
			if (connectedClients[num] != null && connectedClients[num].ClientID == targetSteamID)
			{
				return num;
			}
		}
		return ushort.MaxValue;
	}

	public static bool IsMuted(PlayerInfo player)
	{
		if (player != null)
		{
			return MutedPlayers.Contains(player.NetworkSpawnID);
		}
		return false;
	}

	public static void MutePlayer(PlayerInfo player)
	{
		if (player != null)
		{
			MutedPlayers.Add(player.NetworkSpawnID);
			Debug.Log((object)$"Player {player} muted");
		}
	}

	public static void UnmutePlayer(PlayerInfo player)
	{
		if (player != null && MutedPlayers.Contains(player.NetworkSpawnID))
		{
			MutedPlayers.Remove(player.NetworkSpawnID);
			Debug.Log((object)$"Player {player} unmuted");
		}
	}

	public static bool IsHost(PlayerInfo player)
	{
		return player.Equals(LobbyOwner);
	}

	public static bool IsLocalPlayer(PlayerInfo player)
	{
		return player.Equals(LocalPlayer);
	}

	public static void ToggleLobbyPublicity()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (MultiplayerManager.IsServer)
		{
			ELobbyType val = (ELobbyType)(IsPublic ? 1 : 2);
			MatchmakingHandler.SetNewLobbyType(val);
			RawChangeLobbyTypeMI.Invoke(MatchmakingHandler, new object[1] { val });
		}
	}

	public static void RequestLobbyList()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		Debug.Log((object)"Requesting lobby list...");
		SteamMatchmaking.AddRequestLobbyListDistanceFilter(lobbyDistanceFilter);
		SteamAPICall_t val = SteamMatchmaking.RequestLobbyList();
		LobbyMatchListCallResult.Set(val, (CallResult<LobbyMatchList_t>.APIDispatchDelegate)OnRequestLobbyList);
	}

	private static void OnRequestLobbyList(LobbyMatchList_t result, bool ioFailure)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		Debug.Log((object)("Lobby list received: " + result.m_nLobbiesMatching + " lobbies"));
		lobbyDataList = new List<CSteamID>();
		enteredLobbyList = new HashSet<CSteamID>();
		uint nLobbiesMatching = result.m_nLobbiesMatching;
		for (uint num = 0u; num < nLobbiesMatching; num++)
		{
			CSteamID lobbyByIndex = SteamMatchmaking.GetLobbyByIndex((int)num);
			if (SteamMatchmaking.GetLobbyData(lobbyByIndex, StickFightConstants.VERSION_KEY) == StickFightConstants.VERSION_VALUE)
			{
				lobbyDataList.Add(lobbyByIndex);
				continue;
			}
			CSteamID val = lobbyByIndex;
			Debug.Log((object)("Filtered out lobby with incompatible version: " + ((object)val/*cast due to constrained. prefix*/).ToString()));
			val = lobbyByIndex;
			Debug.Log((object)("Lobby " + ((object)val/*cast due to constrained. prefix*/).ToString() + " version: " + SteamMatchmaking.GetLobbyData(lobbyByIndex, StickFightConstants.VERSION_KEY)));
		}
	}

	public static void JoinSpecificServer(CSteamID lobby)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		Traverse.Create((object)MatchmakingHandler.Instance).Field("mCurrentConnectionType").SetValue((object)(ConnectionType)1);
		MatchmakingHandler.Instance.JoinServer(lobby, (CallResult<LobbyEnter_t>.APIDispatchDelegate)GameManager.Instance.mMultiplayerManager.OnServerJoined);
		OnLobbyMembersChanged();
	}

	public static void KickPlayer(CSteamID clientId, MultiplayerManager.KickResponse response)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Expected I4, but got Unknown
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		byte[] array = new byte[1] { (byte)(int)response };
		P2PPackageHandler.Instance.SendP2PPacketToUser(clientId, array, (P2PPackageHandler.MsgType)38, (EP2PSend)2, 0);
		// QOL-Mod drops kick messages on the receiving side unless they come from its whitelist.
		// Also send "client init refused" (what the blacklist sends to blocked joiners): the
		// receiving game treats it as a failed join and leaves the lobby, and QOL doesn't block it.
		P2PPackageHandler.Instance.SendP2PPacketToUser(clientId, new byte[1], (P2PPackageHandler.MsgType)5, (EP2PSend)2, 0);
	}

	public static void OnLobbyMembersChanged()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		Debug.Log((object)"Lobby member list updated");
		lock (LobbyMemberLock)
		{
			lobbyMemberList.Clear();
			int lobbyMemberLimit = SteamMatchmaking.GetLobbyMemberLimit(CurrentLobbyID);
			for (int i = 0; i < lobbyMemberLimit; i++)
			{
				PlayerInfo playerInfo = new PlayerInfo(SteamMatchmaking.GetLobbyMemberByIndex(CurrentLobbyID, i));
				playerRecorder.CheckAndMerge(playerInfo);
				blackListRecorder.CheckAndMerge(playerInfo);
				lobbyMemberList.Add(playerInfo);
			}
		}
		recentPlayersRecorder.UpdateFromLobbyMembers(lobbyMemberList);
		PlayerManagerGUI.MarkRecentDisplayDirty();
	}

	static PlayerManager()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
	}
}
