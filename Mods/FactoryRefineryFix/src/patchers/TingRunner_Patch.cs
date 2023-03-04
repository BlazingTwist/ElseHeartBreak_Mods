using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BTHarmonyUtils.InstructionSearch;
using BTHarmonyUtils.TranspilerUtils;
using HarmonyLib;
using HeartLibs.heartlibs;
using JetBrains.Annotations;
using TingTing;

namespace FactoryRefineryFix.patchers {

	[HarmonyPatch(declaringType: typeof(TingRunner))]
	public class TingRunner_Patch {

		[UsedImplicitly]
		private static void RemoveTingFromTile(Ting ting) {
			ting.tile.RemoveOccupant(ting);
		}

		[HarmonyPatch(methodName: nameof(TingRunner.RemoveTing))]
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> RemoveTing_Transpiler(IEnumerable<CodeInstruction> codeInstructions) {
			List<CodeInstruction> instructions = codeInstructions.ToList();

			MethodInfo method_GetTing = AccessTools.DeclaredMethod(
					typeof(TingRunner), nameof(TingRunner.GetTing), new[] { typeof(string) }, Type.EmptyTypes
			);
			MethodInfo method_RemoveTingFromTile = SymbolExtensions.GetMethodInfo(() => RemoveTingFromTile(null));

			CodeReplacementPatch patch = new CodeReplacementPatch(
					expectedMatches: 1,
					prefixInstructions: new[] {
							InstructionMask.MatchOpCode(OpCodes.Ldarg_1),
							InstructionMask.MatchInstruction(OpCodes.Call, method_GetTing),
					},
					insertInstructions: new[] {
							new CodeInstruction(OpCodes.Dup),
							new CodeInstruction(OpCodes.Call, method_RemoveTingFromTile),
					}
			);
			patch.ApplySafe(instructions, LoggerProvider.GetLogger());

			return instructions;
		}

	}

}
