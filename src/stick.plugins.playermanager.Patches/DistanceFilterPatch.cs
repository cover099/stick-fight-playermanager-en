using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace stick.plugins.playermanager.Patches;

[HarmonyPatch(typeof(MatchmakingHandler), "OnRandomServerJoinFailed")]
internal class DistanceFilterPatch
{
	public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected Obj, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected Obj, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected Obj, but got Unknown
		return new CodeMatcher(instructions, (ILGenerator)null).MatchForward(false, new CodeMatch[2]
		{
			new CodeMatch((OpCode?)OpCodes.Ldc_I4_1, (object)null, (string)null),
			new CodeMatch((OpCode?)OpCodes.Stloc_0, (object)null, (string)null)
		}).SetInstruction(new CodeInstruction(OpCodes.Ldsfld, (object)AccessTools.Field(typeof(PlayerManager), "lobbyDistanceFilter"))).InstructionEnumeration();
	}
}
