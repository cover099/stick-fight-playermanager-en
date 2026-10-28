using System.Reflection;
using HarmonyLib;

namespace stick.plugins.playermanager.Patches;

[HarmonyPatch(typeof(NetworkPlayer), "TakeLocalControl")]
internal class GetChatManager
{
	private static readonly FieldInfo FieldMLocalChatManager = AccessTools.Field(typeof(NetworkPlayer), "mLocalChatManager");

	public static void Postfix(NetworkPlayer __instance)
	{
		object? value = FieldMLocalChatManager.GetValue(null);
		PlayerManager.LocalChat = (ChatManager)((value is ChatManager) ? value : null);
	}
}
