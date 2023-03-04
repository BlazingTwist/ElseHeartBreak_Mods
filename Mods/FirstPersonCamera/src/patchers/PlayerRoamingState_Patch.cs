using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using BTHarmonyUtils.InstructionSearch;
using BTHarmonyUtils.MidFixPatch;
using FirstPersonCamera.impl;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace FirstPersonCamera.patchers {

	[HarmonyPatch(declaringType: typeof(PlayerRoamingState))]
	public class PlayerRoamingState_Patch {

		[UsedImplicitly]
		public static GameViewControls GetControls(PlayerRoamingState state) {
			throw new NotImplementedException("Harmony Patch not applied.");
		}

		[HarmonyPatch(declaringType: typeof(PlayerRoamingState_Patch), methodName: nameof(GetControls))]
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> GetControls_Transpiler(IEnumerable<CodeInstruction> _) {
			MethodInfo method_getControls = AccessTools.PropertyGetter(typeof(PlayerRoamingState), "controls");
			return new[] {
					new CodeInstruction(OpCodes.Ldarg_0),
					new CodeInstruction(OpCodes.Callvirt, method_getControls),
					new CodeInstruction(OpCodes.Ret),
			};
		}

		[HarmonyPatch(methodName: nameof(PlayerRoamingState.ControlCamera))]
		[HarmonyPrefix]
		private static bool ControlCamera_Prefix() {
			return false;
		}

		private static MidFixInstructionMatcher OnUpdate_Matcher() {
			MethodInfo method_controlCamera = AccessTools.DeclaredMethod(typeof(PlayerRoamingState), nameof(PlayerRoamingState.ControlCamera));
			return new MidFixInstructionMatcher(
					expectedMatches: 1,
					prefixInstructionSequence: new[] {
							InstructionMask.MatchInstruction(OpCodes.Call, method_controlCamera),
					}
			);
		}

		[HarmonyPatch(methodName: nameof(PlayerRoamingState.OnUpdate))]
		[BTHarmonyMidFix(nameof(OnUpdate_Matcher))]
		private static void OnUpdate_MidFix(PlayerRoamingState __instance, CharacterShell ____avatarShell) {
			NavMeshAgent agent = CharacterShell_Patch.GetNavMeshAgent(____avatarShell);
			GameViewControls controls = GetControls(__instance);

			CameraControls.ControlCamera(controls.camera, ____avatarShell);
			CharacterMovement.ControlMovement(__instance, controls, ____avatarShell, agent);
		}

		[HarmonyPatch(methodName: "OnRoomChanged")]
		[HarmonyPostfix]
		private static void OnRoomChanged_Postfix(PlayerRoamingState __instance, string pRoomName) {
			CharacterMovement.OnRoomChanged(pRoomName, GetControls(__instance).world.roomRunner);
		}

	}

}
