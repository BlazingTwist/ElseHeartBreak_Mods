using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx.Logging;
using BTHarmonyUtils.TranspilerUtils;
using FirstPersonCamera.impl;
using HarmonyLib;
using static BTHarmonyUtils.InstructionSearch.InstructionMask;

namespace FirstPersonCamera.patchers.asm {

	[HarmonyPatch(declaringType: typeof(OrbitNewCameraState))]
	public class OrbitNewCameraState_Patch {

		private static readonly ManualLogSource logger = Logger.CreateLogSource(nameof(OrbitNewCameraState_Patch));

		[HarmonyPatch(methodName: nameof(OrbitNewCameraState.Move))]
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Move_Transpiler(IEnumerable<CodeInstruction> codeInstructions) {
			List<CodeInstruction> instructions = codeInstructions.ToList();

			FieldInfo field_minTilt = AccessTools.DeclaredField(typeof(OrbitNewCameraState), nameof(OrbitNewCameraState.minTilt));
			FieldInfo field_maxTilt = AccessTools.DeclaredField(typeof(OrbitNewCameraState), nameof(OrbitNewCameraState.maxTilt));
			MethodInfo method_getMinTilt = SymbolExtensions.GetMethodInfo(() => CameraControls.getMinCameraTilt());
			MethodInfo method_getMaxTilt = SymbolExtensions.GetMethodInfo(() => CameraControls.getMaxCameraTilt());

			CodeReplacementPatch minTiltPatch = new CodeReplacementPatch(
					expectedMatches: 1,
					targetInstructions: new[] {
							MatchOpCode(OpCodes.Ldarg_0),
							MatchInstruction(OpCodes.Ldfld, field_minTilt),
					},
					insertInstructions: new[] { new CodeInstruction(OpCodes.Call, method_getMinTilt) }
			);
			CodeReplacementPatch maxTiltPatch = new CodeReplacementPatch(
					expectedMatches: 1,
					targetInstructions: new[] {
							MatchOpCode(OpCodes.Ldarg_0),
							MatchInstruction(OpCodes.Ldfld, field_maxTilt),
					},
					insertInstructions: new[] { new CodeInstruction(OpCodes.Call, method_getMaxTilt) }
			);
			CodeReplacementPatch ignoreMovePatch = new CodeReplacementPatch(
					expectedMatches: 1,
					targetInstructions: new[] {
							MatchOpCode(OpCodes.Ldarg_0),
							MatchOpCode(OpCodes.Ldfld),
							MatchOpCode(OpCodes.Brtrue),
							MatchOpCode(OpCodes.Ret),
					}
			);

			minTiltPatch.ApplySafe(instructions, logger);
			maxTiltPatch.ApplySafe(instructions, logger);
			ignoreMovePatch.ApplySafe(instructions, logger);

			return instructions;
		}

		[HarmonyPatch(methodName: nameof(OrbitNewCameraState.Zoom))]
		[HarmonyPrefix]
		private static bool Zoom_Prefix() {
			return false;
		}

		[HarmonyPatch(methodName: "SnapTargetAngle")]
		[HarmonyPrefix]
		private static bool SnapTargetAngle_Prefix(OrbitNewCameraState __instance) {
			__instance.targetAngle = __instance.currentAngle;
			return false;
		}

		[HarmonyPatch(methodName: nameof(OrbitNewCameraState.Update))]
		[HarmonyPrefix]
		private static bool Update_Prefix(OrbitNewCameraState __instance) {
			CameraControls.UpdateCameraLookDirection(__instance);
			return false;
		}

	}

}
