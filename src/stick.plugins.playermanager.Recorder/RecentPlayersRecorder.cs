using System;
using System.Collections.Generic;
using System.IO;
using Steamworks;
using UnityEngine;

namespace stick.plugins.playermanager.Recorder;

public class RecentPlayersRecorder
{
	private const string FileName = "RecentPlayers.txt";

	private readonly Dictionary<CSteamID, long> _recent = new Dictionary<CSteamID, long>();

	private CSteamID _lastLobbyId;

	private readonly HashSet<CSteamID> _lastMembers = new HashSet<CSteamID>();

	private bool _hasLastSnapshot;

	public int MaxCount { get; set; } = 20;

	private string FilePath => Path.Combine(Application.dataPath, "RecentPlayers.txt");

	public RecentPlayersRecorder()
	{
		LoadFromFile();
	}

	public void UpdateFromLobbyMembers(IEnumerable<PlayerInfo> members)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		if (!PlayerManager.MatchmakingHandler.IsInsideLobby)
		{
			_hasLastSnapshot = false;
			return;
		}
		CSteamID currentLobbyID = PlayerManager.CurrentLobbyID;
		PlayerInfo localPlayer = PlayerManager.LocalPlayer;
		HashSet<CSteamID> hashSet = new HashSet<CSteamID>();
		foreach (PlayerInfo member in members)
		{
			if (member != null)
			{
				CSteamID steamId = member.SteamId;
				if (steamId.IsValid() && (localPlayer == null || !(member.SteamId == localPlayer.SteamId)))
				{
					hashSet.Add(member.SteamId);
				}
			}
		}
		if (_hasLastSnapshot && _lastLobbyId == currentLobbyID && _lastMembers.SetEquals(hashSet))
		{
			return;
		}
		long value = NowUnixSeconds();
		foreach (CSteamID item in hashSet)
		{
			_recent[item] = value;
		}
		_lastLobbyId = currentLobbyID;
		_lastMembers.Clear();
		_lastMembers.UnionWith(hashSet);
		_hasLastSnapshot = true;
		TrimToLimit();
		WriteBackToFile();
	}

	public void ApplyLimit()
	{
		int count = _recent.Count;
		TrimToLimit();
		if (_recent.Count != count)
		{
			WriteBackToFile();
		}
	}

	public List<PlayerInfo> GetRecentPlayers()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		List<KeyValuePair<CSteamID, long>> list = SortedEntries();
		List<PlayerInfo> list2 = new List<PlayerInfo>(list.Count);
		foreach (KeyValuePair<CSteamID, long> item in list)
		{
			CSteamID key = item.Key;
			if (key.IsValid())
			{
				list2.Add(new PlayerInfo(item.Key, string.Empty));
			}
		}
		return list2;
	}

	private void LoadFromFile()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		if (!File.Exists(FilePath))
		{
			return;
		}
		_recent.Clear();
		using StreamReader streamReader = new StreamReader(FilePath);
		string text;
		while ((text = streamReader.ReadLine()) != null)
		{
			if (!string.IsNullOrEmpty(text) && TryParseLine(text, out var steamId, out var timestamp) && steamId.IsValid() && (!_recent.TryGetValue(steamId, out var value) || value < timestamp))
			{
				_recent[steamId] = timestamp;
			}
		}
	}

	private void TrimToLimit()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		int num = MaxCount;
		if (num < 1)
		{
			num = 1;
		}
		if (_recent.Count > num)
		{
			List<KeyValuePair<CSteamID, long>> list = SortedEntries();
			for (int i = num; i < list.Count; i++)
			{
				_recent.Remove(list[i].Key);
			}
		}
	}

	private void WriteBackToFile()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		List<KeyValuePair<CSteamID, long>> list = SortedEntries();
		using StreamWriter streamWriter = new StreamWriter(FilePath, append: false);
		foreach (KeyValuePair<CSteamID, long> item in list)
		{
			streamWriter.WriteLine(((object)item.Key/*cast due to constrained. prefix*/).ToString() + " " + item.Value);
		}
	}

	private List<KeyValuePair<CSteamID, long>> SortedEntries()
	{
		List<KeyValuePair<CSteamID, long>> list = new List<KeyValuePair<CSteamID, long>>(_recent);
		list.Sort((KeyValuePair<CSteamID, long> a, KeyValuePair<CSteamID, long> b) => b.Value.CompareTo(a.Value));
		return list;
	}

	private static bool TryParseLine(string line, out CSteamID steamId, out long timestamp)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		steamId = CSteamID.Nil;
		timestamp = 0L;
		int num = line.IndexOf(' ');
		if (num <= 0 || num == line.Length - 1)
		{
			return false;
		}
		if (!ulong.TryParse(line.Substring(0, num), out var result))
		{
			return false;
		}
		if (!long.TryParse(line.Substring(num + 1), out var result2))
		{
			return false;
		}
		steamId = new CSteamID(result);
		timestamp = result2;
		return true;
	}

	private static long NowUnixSeconds()
	{
		DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		return (long)(DateTime.UtcNow - dateTime).TotalSeconds;
	}
}
