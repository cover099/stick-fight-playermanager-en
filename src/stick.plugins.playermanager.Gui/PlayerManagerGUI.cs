using System.Collections.Generic;
using Steamworks;
using UnityEngine;
using stick.plugins.playermanager.Gui.InternalWidgets;
using stick.plugins.playermanager.Recorder;

namespace stick.plugins.playermanager.Gui;

public static class PlayerManagerGUI
{
	private enum RecordListTab
	{
		Records,
		Blacklist
	}

	private static Vector2 ScrollPosition;

	private static readonly LayoutCursor Cursor = new LayoutCursor();

	private static readonly HashSet<IPlayerRowWidget> IgnoreWidgets = new HashSet<IPlayerRowWidget>();

	private static float Scale = 1f;

	private static Vector2 RecentScrollPosition;

	private static Vector2 RecordListScrollPosition;

	private static Vector2 BlackListScrollPosition;

	private static RecordListTab CurrentTab = RecordListTab.Records;

	private static readonly List<PlayerInfo> RecentDisplayList = new List<PlayerInfo>();

	private static bool RecentDisplayDirty = true;

	private static readonly Dictionary<CSteamID, string> LobbyTextCache = new Dictionary<CSteamID, string>();

	private static float _lobbyTextCacheTime;

	private static readonly IPlayerRowWidget AddFriendWidget = AddFriendButton.Create();

	private static readonly IPlayerRowWidget RecordWidget = RecordButton.Create();

	private static readonly IPlayerRowWidget NameToggleWidget = NameToggleButton.Create();

	private static readonly PlayerRowButtonWidget NameButton = new PlayerRowButtonWidget
	{
		Name = "SidePanelPlayerName",
		GetText = (PlayerInfo player) => player.ToString(),
		OnClick = (PlayerInfo player) =>
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			GUIUtility.systemCopyBuffer = player.ToString();
			SteamFriends.ActivateGameOverlayToUser("steamid", player.SteamId);
		},
		GetStyle = PlayerInfoButton.GetStatusStyle
	};

	public static bool ShowLobbyInfo { get; set; }

	private static float PanelWidth => 525f * Scale;

	private static float SidePanelWidth => 825f * Scale + 20f;

	public static void DrawLobbyInfoGUI()
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		Scale = Mathf.Max(0.1f, (float)Screen.width / 2560f * PlayerManager.GuiScale.Value);
		GUIStyles.Initialize(Scale);
		Cursor.Scale = Scale;
		Cursor.Reset();
		if (PlayerManager.MatchmakingHandler.IsInsideLobby)
		{
			DrawCurrentLobbyInfo();
			DrawLobbySearchInfo();
		}
		else
		{
			GUI.Label(Cursor.NextLine(500f, 40f), "Not in a lobby", GUIStyles.Label);
			DrawLobbySearchInfo();
			DrawRightPanel();
		}
	}

	public static void MarkRecentDisplayDirty()
	{
		RecentDisplayDirty = true;
	}

	private static void DrawCurrentLobbyInfo()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		GUI.Label(Cursor.NextLine(2000f, 40f), $"Lobby: {PlayerManager.CurrentLobbyID}  Host: {PlayerManager.LobbyOwner.CurrentName}", GUIStyles.Label);
		foreach (IHeaderRow headerRow in RegisterManager.HeaderRows)
		{
			if (!headerRow.ShouldDraw())
			{
				continue;
			}
			Cursor.BeginRow();
			foreach (IHeaderWidget child in headerRow.Children)
			{
				if (child.ShouldDraw())
				{
					child.Draw(Cursor.NextColumn(child.Width, 40f));
				}
			}
			Cursor.EndRow();
		}
		IgnoreWidgets.Clear();
		CSteamID steamId;
		foreach (IPlayerRowWidget playerRowWidget in RegisterManager.PlayerRowWidgets)
		{
			bool flag = false;
			foreach (PlayerInfo lobbyMember in PlayerManager.lobbyMemberList)
			{
				steamId = lobbyMember.SteamId;
				if (steamId.IsValid() && playerRowWidget.ShouldDraw(lobbyMember))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				IgnoreWidgets.Add(playerRowWidget);
			}
		}
		foreach (PlayerInfo lobbyMember2 in PlayerManager.lobbyMemberList)
		{
			steamId = lobbyMember2.SteamId;
			if (!steamId.IsValid())
			{
				continue;
			}
			Cursor.BeginRow();
			foreach (IPlayerRowWidget playerRowWidget2 in RegisterManager.PlayerRowWidgets)
			{
				if (playerRowWidget2.ShouldDraw(lobbyMember2))
				{
					playerRowWidget2.Draw(Cursor.NextColumn(playerRowWidget2.Width, 40f), lobbyMember2);
				}
				else if (!IgnoreWidgets.Contains(playerRowWidget2))
				{
					Cursor.NextColumn(playerRowWidget2.Width, 40f);
				}
			}
			Cursor.EndRow();
		}
	}

	private static void DrawLobbySearchInfo()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		string areaFilterText = GetAreaFilterText();
		if (GUI.Button(Cursor.NextLine(400f, 40f), "Search range " + areaFilterText, GUIStyles.Button))
		{
			CycleLobbyDistanceFilter();
			PlayerManager.RequestLobbyList();
		}
		if (GUI.Button(new Rect(Cursor.OffsetX + 420f * Scale, Cursor.CurrentY, 100f * Scale, 40f * Scale), "Refresh", GUIStyles.Button))
		{
			PlayerManager.RequestLobbyList();
		}
		GUI.Label(Cursor.NextLine(500f, 40f), $"Lobbies found: {PlayerManager.lobbyDataList.Count}", GUIStyles.Label);
		DrawLobbyListScrollView();
	}

	private static string GetAreaFilterText()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected I4, but got Unknown
		ELobbyDistanceFilter lobbyDistanceFilter = PlayerManager.lobbyDistanceFilter;
		return (int)lobbyDistanceFilter switch
		{
			0 => "My region", 
			1 => "Nearby regions", 
			2 => "Same hemisphere", 
			3 => "Worldwide", 
			_ => "Unknown", 
		};
	}

	private static void CycleLobbyDistanceFilter()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		ELobbyDistanceFilter lobbyDistanceFilter = (((int)PlayerManager.lobbyDistanceFilter != 3) ? ((ELobbyDistanceFilter)(PlayerManager.lobbyDistanceFilter + 1)) : ((ELobbyDistanceFilter)0));
		PlayerManager.lobbyDistanceFilter = lobbyDistanceFilter;
	}

	private static void DrawLobbyListScrollView()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		float scale = Scale;
		ScrollPosition = GUI.BeginScrollView(new Rect(Cursor.OffsetX, Cursor.CurrentY + 40f * scale, 525f * scale, 405f * scale), ScrollPosition, new Rect(0f, 0f, 505f * scale, (float)PlayerManager.lobbyDataList.Count * 50f * scale));
		int count = PlayerManager.lobbyDataList.Count;
		if (count > 0)
		{
			float num = 50f * scale;
			int num2 = Mathf.Clamp(Mathf.FloorToInt(ScrollPosition.y / num), 0, count - 1);
			int num3 = Mathf.Clamp(Mathf.FloorToInt((ScrollPosition.y + 405f * scale) / num), 0, count - 1);
			for (int i = num2; i <= num3; i++)
			{
				CSteamID val = PlayerManager.lobbyDataList[i];
				GUIStyle val2 = (PlayerManager.enteredLobbyList.Contains(val) ? GUIStyles.ButtonPressed : GUIStyles.Button);
				if (GUI.Button(new Rect(Cursor.OffsetX, Cursor.LineHeight * (float)i, 500f * scale, 40f * scale), GetLobbyButtonText(val), val2) && !PlayerManager.MatchmakingHandler.IsInsideLobby)
				{
					PlayerManager.enteredLobbyList.Add(val);
					PlayerManager.JoinSpecificServer(val);
				}
			}
		}
		GUI.EndScrollView();
	}

	private static string GetLobbyButtonText(CSteamID lobbyId)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (realtimeSinceStartup - _lobbyTextCacheTime > 3f)
		{
			LobbyTextCache.Clear();
			_lobbyTextCacheTime = realtimeSinceStartup;
		}
		if (!LobbyTextCache.TryGetValue(lobbyId, out var value))
		{
			value = $"{lobbyId}  Players: {SteamMatchmaking.GetNumLobbyMembers(lobbyId)}";
			LobbyTextCache[lobbyId] = value;
		}
		return value;
	}

	private static void DrawRightPanel()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		Rect val = new Rect((float)Screen.width - 5f - SidePanelWidth, Cursor.OffsetY, SidePanelWidth, (float)Screen.height - 2f * Cursor.OffsetY);
		GUI.Box(val, GUIContent.none);
		DrawRecentPlayersArea(val);
		DrawRecordListArea(val);
	}

	private static void DrawRecentPlayersArea(Rect panelRect)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		EnsureRecentDisplayList();
		float scale = Scale;
		float num = panelRect.height / 2f;
		float num2 = 40f * scale;
		float num3 = 40f * scale;
		GUI.Label(new Rect(panelRect.x, panelRect.y, panelRect.width, num3), $"Recently played with ({RecentDisplayList.Count})", GUIStyles.Label);
		Rect val = new Rect(panelRect.x, panelRect.y + num3, panelRect.width - 5f * scale, num - num3);
		float num4 = 500f * scale;
		RecentScrollPosition = GUI.BeginScrollView(val, RecentScrollPosition, new Rect(0f, 0f, val.width, (float)RecentDisplayList.Count * num2));
		int count = RecentDisplayList.Count;
		if (count > 0)
		{
			int num5 = Mathf.Clamp(Mathf.FloorToInt(RecentScrollPosition.y / num2), 0, count - 1);
			int num6 = Mathf.Clamp(Mathf.FloorToInt((RecentScrollPosition.y + val.height) / num2), 0, count - 1);
			for (int i = num5; i <= num6; i++)
			{
				PlayerInfo player = RecentDisplayList[i];
				float num7 = (float)i * num2;
				float num8 = 0f;
				NameButton.Draw(new Rect(num8, num7, num4, num2 - 2f), player);
				num8 += num4;
				if (AddFriendWidget.ShouldDraw(player))
				{
					AddFriendWidget.Draw(new Rect(num8, num7, 100f * scale, num2 - 2f), player);
				}
				num8 += 100f * scale;
				if (RecordWidget.ShouldDraw(player))
				{
					RecordWidget.Draw(new Rect(num8, num7, 100f * scale, num2 - 2f), player);
				}
				num8 += 100f * scale;
				if (NameToggleWidget.ShouldDraw(player))
				{
					NameToggleWidget.Draw(new Rect(num8, num7, 120f * scale, num2 - 2f), player);
				}
			}
		}
		GUI.EndScrollView();
	}

	private static void DrawRecordListArea(Rect panelRect)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		float scale = Scale;
		float num = panelRect.height / 2f;
		float num2 = 40f * scale;
		float num3 = 40f * scale;
		float num4 = 20f;
		float num5 = panelRect.y + num;
		string text = ((CurrentTab == RecordListTab.Records) ? "Showing: recorded players (click to switch)" : "Showing: blacklist (click to switch)");
		if (GUI.Button(new Rect(panelRect.x, num5, panelRect.width, num3), text, GUIStyles.Button))
		{
			CurrentTab = ((CurrentTab == RecordListTab.Records) ? RecordListTab.Blacklist : RecordListTab.Records);
			RemoveRecordButton.ClearPending();
		}
		List<PlayerInfo> allPlayers = ((CurrentTab == RecordListTab.Records) ? ((RecorderBase)PlayerManager.playerRecorder) : ((RecorderBase)PlayerManager.blackListRecorder)).GetAllPlayers();
		RemoveRecordButton.ClearPendingIfGone(allPlayers);
		Rect val = new Rect(panelRect.x, num5 + num3, panelRect.width - 5f * scale, num - num3);
		float num6 = 500f * scale;
		Vector2 val2 = ((CurrentTab == RecordListTab.Records) ? RecordListScrollPosition : BlackListScrollPosition);
		val2 = GUI.BeginScrollView(val, val2, new Rect(0f, 0f, val.width, (float)allPlayers.Count * num2));
		if (CurrentTab == RecordListTab.Records)
		{
			RecordListScrollPosition = val2;
		}
		else
		{
			BlackListScrollPosition = val2;
		}
		bool isBlacklist = CurrentTab == RecordListTab.Blacklist;
		int count = allPlayers.Count;
		if (count > 0)
		{
			int num7 = Mathf.Clamp(Mathf.FloorToInt(val2.y / num2), 0, count - 1);
			int num8 = Mathf.Clamp(Mathf.FloorToInt((val2.y + val.height) / num2), 0, count - 1);
			for (int i = num7; i <= num8; i++)
			{
				PlayerInfo playerInfo = allPlayers[i];
				float num9 = (float)i * num2;
				NameButton.Draw(new Rect(0f, num9, num6, num2 - 2f), playerInfo);
				if (RemoveRecordButton.Draw(new Rect(val.width - 100f * scale - num4, num9, 100f * scale, num2 - 2f), playerInfo, isBlacklist))
				{
					MarkRecentDisplayDirty();
				}
			}
		}
		GUI.EndScrollView();
	}

	private static void EnsureRecentDisplayList()
	{
		if (!RecentDisplayDirty)
		{
			return;
		}
		RecentDisplayDirty = false;
		RecentDisplayList.Clear();
		foreach (PlayerInfo recentPlayer in PlayerManager.recentPlayersRecorder.GetRecentPlayers())
		{
			PlayerManager.playerRecorder.CheckAndMerge(recentPlayer);
			PlayerManager.blackListRecorder.CheckAndMerge(recentPlayer);
			RecentDisplayList.Add(recentPlayer);
		}
	}
}
