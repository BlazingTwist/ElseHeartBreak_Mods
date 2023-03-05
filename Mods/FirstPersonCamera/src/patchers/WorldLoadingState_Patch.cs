using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BTHarmonyUtils.InstructionSearch;
using BTHarmonyUtils.TranspilerUtils;
using FirstPersonCamera.impl;
using HarmonyLib;
using HeartLibs.heartlibs;

namespace FirstPersonCamera.patchers {

	[HarmonyPatch(declaringType: typeof(WorldLoadingState))]
	public class WorldLoadingState_Patch {

		[HarmonyPatch(methodName: "LoadStuff")]
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> LoadStuff_Transpiler(IEnumerable<CodeInstruction> codeInstructions) {
			List<CodeInstruction> instructions = codeInstructions.ToList();

			ConstructorInfo constructor_roamingState = AccessTools.DeclaredConstructor(typeof(PlayerRoamingState), new[] { typeof(bool) });
			MethodInfo method_instantiateFPC = SymbolExtensions.GetMethodInfo(() => FirstPersonState.Instantiate(false));

			CodeReplacementPatch patch = new CodeReplacementPatch(
					expectedMatches: 1,
					targetInstructions: new[] {
							InstructionMask.MatchInstruction(OpCodes.Newobj, constructor_roamingState),
					},
					insertInstructions: new[] {
							new CodeInstruction(OpCodes.Call, method_instantiateFPC),
					}
			);
			patch.ApplySafe(instructions, LoggerProvider.GetLogger());

			return instructions;
		}

	}

}
