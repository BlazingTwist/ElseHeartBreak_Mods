using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx.Logging;
using BTHarmonyUtils.InstructionSearch;
using BTHarmonyUtils.TranspilerUtils;
using HarmonyLib;

namespace FirstPersonCamera.patchers.asm {

	[HarmonyPatch(declaringType: typeof(DialogueSubstate))]
	public class DialogueSubstate_Patch {

		private static readonly ManualLogSource logger = Logger.CreateLogSource(nameof(DialogueSubstate_Patch));

		[HarmonyPatch(methodType: MethodType.Constructor,
				argumentTypes: new[] { typeof(GameViewControls), typeof(string), typeof(DialogueSubstate.IsShowingInventory) })]
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Constructor_Transpiler(IEnumerable<CodeInstruction> codeInstructions) {
			List<CodeInstruction> instructions = codeInstructions.ToList();

			MethodInfo method_setTilt = AccessTools.DeclaredMethod(typeof(GreatCamera), nameof(GreatCamera.Input_SetTilt));

			CodeReplacementPatch patch = new CodeReplacementPatch(
					expectedMatches: 1,
					targetInstructions: new[] {
							InstructionMask.MatchOpCode(OpCodes.Ldarg_0),
							InstructionMask.MatchOpCode(OpCodes.Ldfld),
							InstructionMask.MatchOpCode(OpCodes.Callvirt),
							InstructionMask.MatchOpCode(OpCodes.Ldc_R4),
							InstructionMask.MatchInstruction(OpCodes.Callvirt, method_setTilt),
					}
			);
			patch.ApplySafe(instructions, logger);

			return instructions;
		}

	}

}
