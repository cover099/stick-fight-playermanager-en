using System;
using System.Runtime.CompilerServices;
using Steamworks;
using UnityEngine;

namespace stick.plugins.playermanager.Recorder;

public class PlayerInfo : IEquatable<PlayerInfo>
{
	private const float NameCacheTtl = 2f;

	private const float FriendCacheTtl = 2f;

	private string _currentNameCache;

	private float _currentNameCacheTime;

	private bool? _isFriendCache;

	private float _isFriendCacheTime;

	public CSteamID SteamId
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return field;
		}
		[CompilerGenerated]
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			field = value;
		}
	}

	public string RecordedName { get; set; }

	public bool DisplayNameRecorded { get; set; }

	public bool HasRecordedName
	{
		get
		{
			if (!string.IsNullOrEmpty(RecordedName))
			{
				return RecordedName != CurrentName;
			}
			return false;
		}
	}

	public bool IsFriend
	{
		get
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			if (_isFriendCache.HasValue && realtimeSinceStartup - _isFriendCacheTime < 2f)
			{
				return _isFriendCache.Value;
			}
			_isFriendCache = SteamFriends.HasFriend(SteamId, (EFriendFlags)4);
			_isFriendCacheTime = realtimeSinceStartup;
			return _isFriendCache.Value;
		}
	}

	public PlayerRecordStatus RecordStatus { get; set; }

	public ushort NetworkSpawnID
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return PlayerManager.GetNetworkSpawnIDBySteamID(SteamId);
		}
	}

	public string CurrentName
	{
		get
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			if (SteamManager.Initialized)
			{
				float realtimeSinceStartup = Time.realtimeSinceStartup;
				if (_currentNameCache != null && realtimeSinceStartup - _currentNameCacheTime < 2f)
				{
					return _currentNameCache;
				}
				_currentNameCache = SteamFriends.GetFriendPersonaName(SteamId);
				_currentNameCacheTime = realtimeSinceStartup;
				return _currentNameCache;
			}
			return RecordedName ?? string.Empty;
		}
	}

	public PlayerInfo(CSteamID steamId, string playerName)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		SteamId = steamId;
		RecordedName = playerName;
	}

	public PlayerInfo(CSteamID steamId)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		SteamId = steamId;
		RecordedName = SteamFriends.GetFriendPersonaName(steamId);
	}

	public PlayerInfo(string playerInfo)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrEmpty(playerInfo))
		{
			SteamId = CSteamID.Nil;
			RecordedName = string.Empty;
		}
		else
		{
			Parse(playerInfo, out var steamId, out var playerName);
			SteamId = new CSteamID(steamId);
			RecordedName = playerName;
		}
	}

	public static void Parse(string input, out ulong steamId, out string playerName)
	{
		int num = input.IndexOf(' ');
		if (num == -1)
		{
			steamId = ulong.Parse(input);
			playerName = string.Empty;
			return;
		}
		string s = input.Substring(0, num);
		string text = input.Substring(num + 1);
		steamId = ulong.Parse(s);
		playerName = text;
	}

	public static bool TryParse(string input, out ulong steamId, out string playerName)
	{
		steamId = 0uL;
		playerName = string.Empty;
		try
		{
			Parse(input, out steamId, out playerName);
			return true;
		}
		catch
		{
			return false;
		}
	}

	public void MergeWith(PlayerInfo other)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (other == null)
		{
			return;
		}
		if (SteamId != other.SteamId)
		{
			Debug.LogWarning((object)$"Tried to merge player info with different Steam IDs: {SteamId} and {other.SteamId}");
			return;
		}
		if (!string.IsNullOrEmpty(other.RecordedName))
		{
			RecordedName = other.RecordedName;
		}
		if (other.RecordStatus > RecordStatus)
		{
			RecordStatus = other.RecordStatus;
		}
	}

	public string ToInfoString()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (!string.IsNullOrEmpty(RecordedName))
		{
			return $"{SteamId} {RecordedName}";
		}
		return $"{SteamId} {CurrentName}";
	}

	public override string ToString()
	{
		return ToInfoString();
	}

	public bool Equals(PlayerInfo other)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (other != null)
		{
			return SteamId == other.SteamId;
		}
		return false;
	}

	public override int GetHashCode()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return ((object)SteamId/*cast due to constrained. prefix*/).GetHashCode();
	}
}
