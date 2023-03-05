using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx.Logging;
using BTHarmonyUtils.InstructionSearch;
using BTHarmonyUtils.TranspilerUtils;
using FirstPersonCamera.impl;
using HarmonyLib;
using UnityEngine;

namespace FirstPersonCamera.patchers {

	[HarmonyPatch(declaringType: typeof(GreatCamera))]
	public class GreatCamera_Patch {

		private static readonly ManualLogSource logger = Logger.CreateLogSource(nameof(GreatCamera_Patch));

		[HarmonyPatch(methodName: "Update")]
		[HarmonyPrefix]
		private static bool Update_Prefix() {
			return false;
		}

		[HarmonyPatch(methodName: nameof(GreatCamera.UpdateStates))]
		[HarmonyPostfix]
		private static void UpdateStates_Postfix(GreatCamera __instance) {
			// TODO kinda jank doing this every frame
			Camera camera = Camera.main;
			if (camera != null) {
				camera.fieldOfView = 90f;
				camera.nearClipPlane = 0.15f;
			}
		}

		[HarmonyPatch(methodName: nameof(GreatCamera.EnterFixedCamera))]
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> EnterFixedCamera_Transpiler(IEnumerable<CodeInstruction> codeInstructions) {
			List<CodeInstruction> instructions = codeInstructions.ToList();

			MethodInfo getter_position = AccessTools.PropertyGetter(typeof(Transform), nameof(Transform.position));
			MethodInfo method_GetFixCameraPosition = SymbolExtensions.GetMethodInfo(() => CameraControls.GetFixCameraPosition(null, null));

			CodeReplacementPatch patch = new CodeReplacementPatch(
					expectedMatches: 1,
					prefixInstructions: new[] {
							InstructionMask.MatchOpCode(OpCodes.Ldarg_1),
					},
					targetInstructions: new[] {
							InstructionMask.MatchInstruction(OpCodes.Callvirt, getter_position),
					},
					insertInstructions: new[] {
							new CodeInstruction(OpCodes.Ldarg_2),
							new CodeInstruction(OpCodes.Call, method_GetFixCameraPosition)
					}
			);
			patch.ApplySafe(instructions, logger);

			return instructions;
		}

		// uncomment this if you want Input_SetRotation to work with FPC
		// currently that behaviour is unwanted
		/*[HarmonyPatch(methodName: nameof(GreatCamera.Input_SetRotation))]
		[HarmonyPostfix]
		private static void Input_SetRotation_Postfix(GreatCamera __instance, float pAngle) {
			__instance.orbit.currentAngle = pAngle;
		}*/

	}

}
