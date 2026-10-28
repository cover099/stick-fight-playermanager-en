using System.Collections.Generic;
using System.IO;
using Steamworks;
using UnityEngine;

namespace stick.plugins.playermanager.Recorder;

public abstract class RecorderBase
{
	protected Dictionary<CSteamID, PlayerInfo> playerDict = new Dictionary<CSteamID, PlayerInfo>();

	private List<PlayerInfo> _allPlayersCache;

	protected abstract string FileName { get; }

	protected abstract PlayerRecordStatus RecordStatus { get; }

	protected string FilePath => Path.Combine(Application.dataPath, FileName);

	protected RecorderBase()
	{
		RefreshPlayerRecordList();
	}

	public bool IsPlayerRecorded(PlayerInfo playerInfo)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (playerInfo != null)
		{
			return playerDict.ContainsKey(playerInfo.SteamId);
		}
		return false;
	}

	public bool IsPlayerRecorded(CSteamID playerId)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return playerDict.ContainsKey(playerId);
	}

	public void RecordPlayer(PlayerInfo playerInfo)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (playerInfo != null)
		{
			playerInfo.RecordStatus = RecordStatus;
			if (!playerDict.ContainsKey(playerInfo.SteamId))
			{
				playerDict.Add(playerInfo.SteamId, playerInfo);
			}
			AppendToFile(playerInfo);
			_allPlayersCache = null;
		}
	}

	public void CancelRecordPlayer(PlayerInfo playerInfo)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (playerInfo != null && playerDict.Remove(playerInfo.SteamId))
		{
			WriteBackToFile();
			_allPlayersCache = null;
		}
	}

	public List<PlayerInfo> GetAllPlayers()
	{
		if (_allPlayersCache == null)
		{
			_allPlayersCache = new List<PlayerInfo>(playerDict.Values);
			_allPlayersCache.Sort((PlayerInfo a, PlayerInfo b) => string.CompareOrdinal(a.CurrentName, b.CurrentName));
		}
		return _allPlayersCache;
	}

	public bool TryGetPlayer(CSteamID steamId, out PlayerInfo playerInfo)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return playerDict.TryGetValue(steamId, out playerInfo);
	}

	public void CheckAndMerge(PlayerInfo playerInfo)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (playerInfo != null && playerDict.TryGetValue(playerInfo.SteamId, out var value))
		{
			playerInfo.MergeWith(value);
		}
	}

	protected void RefreshPlayerRecordList()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if (!File.Exists(FilePath))
		{
			return;
		}
		playerDict.Clear();
		using (StreamReader streamReader = new StreamReader(FilePath))
		{
			string text;
			while ((text = streamReader.ReadLine()) != null)
			{
				if (!string.IsNullOrEmpty(text))
				{
					PlayerInfo playerInfo = new PlayerInfo(text)
					{
						RecordStatus = RecordStatus
					};
					if (!playerDict.ContainsKey(playerInfo.SteamId))
					{
						playerDict.Add(playerInfo.SteamId, playerInfo);
					}
				}
			}
		}
		_allPlayersCache = null;
	}

	protected void WriteBackToFile()
	{
		using StreamWriter streamWriter = new StreamWriter(FilePath, append: false);
		foreach (PlayerInfo value in playerDict.Values)
		{
			streamWriter.WriteLine(value.ToInfoString());
		}
	}

	private void AppendToFile(PlayerInfo playerInfo)
	{
		using StreamWriter streamWriter = new StreamWriter(FilePath, append: true);
		streamWriter.WriteLine(playerInfo);
	}
}
