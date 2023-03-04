using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx.Logging;
using BTHarmonyUtils.InstructionSearch;
using BTHarmonyUtils.TranspilerUtils;
using GameWorld2;
using HarmonyLib;
using HeartLibs.heartlibs;
using JetBrains.Annotations;
using TingTing;

namespace FactoryRefineryFix.patchers {

	[HarmonyPatch(declaringType: typeof(Machine))]
	public class Machine_Patch {

		[UsedImplicitly]
		private static bool MachineCanAcceptTing(Ting ting) {
			return ting != null && !ting.isDeleted && !ting.isBeingHeld;
		}

		[HarmonyPatch(methodName: nameof(Machine.Update))]
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Update_Transpiler(IEnumerable<CodeInstruction> codeInstructions) {
			List<CodeInstruction> instructions = codeInstructions.ToList();
			ManualLogSource logger = LoggerProvider.GetLogger();

			MethodInfo method_MachineCanAcceptTing = SymbolExtensions.GetMethodInfo(() => MachineCanAcceptTing(null));

			InstructionSearcher continueLoopSearcher = new InstructionSearcher(
					expectedMatches: 1,
					searchMasks: new[] {
							new SearchMask(InstructionMask.MatchOpCode(OpCodes.Br), true),
							new SearchMask(InstructionMask.MatchOpCode(OpCodes.Ldloc_S), true),
							new SearchMask(InstructionMask.MatchOpCode(OpCodes.Isinst)),
							new SearchMask(InstructionMask.MatchOpCode(OpCodes.Stloc_0)),
							new SearchMask(InstructionMask.MatchOpCode(OpCodes.Br)),
					}
			);
			List<List<CodeInstruction>> matchedInstructions = continueLoopSearcher.DoSearchSafe(instructions, logger);
			CodeInstruction continueBranchInstruction = matchedInstructions[0][0];
			CodeInstruction loadTingLocalInstruction = matchedInstructions[0][1];
			object continueLabel = continueBranchInstruction.operand;

			CodeReplacementPatch patch = new CodeReplacementPatch(
					expectedMatches: 1,
					postfixInstructions: new[] {
							InstructionMask.MatchOpCode(OpCodes.Ldloc_S),
							InstructionMask.MatchOpCode(OpCodes.Isinst),
							InstructionMask.MatchOpCode(OpCodes.Stloc_0),
							InstructionMask.MatchOpCode(OpCodes.Br),
					},
					insertInstructions: new[] {
							loadTingLocalInstruction,
							new CodeInstruction(OpCodes.Call, method_MachineCanAcceptTing),
							new CodeInstruction(OpCodes.Brfalse, continueLabel),
					}
			);
			patch.ApplySafe(instructions, logger);

			return instructions;
		}

	}

}
