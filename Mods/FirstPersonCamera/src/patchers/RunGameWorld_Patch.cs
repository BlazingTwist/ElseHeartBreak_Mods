using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BTHarmonyUtils.InstructionSearch;
using BTHarmonyUtils.MidFixPatch;
using BTHarmonyUtils.TranspilerUtils;
using FirstPersonCamera.impl;
using HarmonyLib;
using HeartLibs.heartlibs;
using UnityEngine;

namespace FirstPersonCamera.patchers {

	[HarmonyPatch(declaringType: typeof(RunGameWorld))]
	public class RunGameWorld_Patch {

		private static MidFixInstructionMatcher OnRoomHasChanged_Matcher() {
			FieldInfo field_autoZoom = AccessTools.DeclaredField(typeof(MainMenu), nameof(MainMenu.autoZoom));
			return new MidFixInstructionMatcher(
					expectedMatches: 1,
					postfixInstructionSequence: new[] {
							InstructionMask.MatchInstruction(OpCodes.Ldsfld, field_autoZoom),
					}
			);
		}

		[HarmonyPatch(methodName: "OnRoomHasChanged")]
		[BTHarmonyMidFix(nameof(OnRoomHasChanged_Matcher))]
		private static bool OnRoomHasChanged_MidFix() {
			return false;
		}

		[HarmonyPatch(methodName: "Awake")]
		[HarmonyPostfix]
		private static void Awake_Postfix(GameViewControls ____gameViewControls) {
			Transform mapScreenArm = ____gameViewControls.camera.transform.FindChild("MapScreenArm");
			mapScreenArm.localPosition = new Vector3(0f, 0f, -4f);
		}

		[HarmonyPatch(methodName: "Update")]
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Update_Transpiler(IEnumerable<CodeInstruction> codeInstructions) {
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
