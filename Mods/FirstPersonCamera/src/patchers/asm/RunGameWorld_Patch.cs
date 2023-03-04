using System.Reflection;
using System.Reflection.Emit;
using BTHarmonyUtils.InstructionSearch;
using BTHarmonyUtils.MidFixPatch;
using HarmonyLib;
using UnityEngine;

namespace FirstPersonCamera.patchers.asm {

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

	}

}
