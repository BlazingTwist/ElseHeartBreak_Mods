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

		[HarmonyPatch(methodName: "LateUpdate")]
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> LateUpdate_Transpiler(IEnumerable<CodeInstruction> codeInstructions) {
			List<CodeInstruction> instructions = codeInstructions.ToList();

			FieldInfo field_greatCamera = AccessTools.DeclaredField(typeof(RunGameWorld), "_greatCamera");
			MethodInfo getter_avatarShell = AccessTools.DeclaredPropertyGetter(typeof(RunGameWorld), "avatarShell");
			MethodInfo method_UpdateCameraLookDirection = SymbolExtensions.GetMethodInfo(() => CameraControls.UpdateCameraLookDirection(null, null));

			CodeReplacementPatch patch = new CodeReplacementPatch(
					expectedMatches: 1,
					targetInstructions: new[] {
							InstructionMask.MatchOpCode(OpCodes.Ldarg_0),
							InstructionMask.MatchOpCode(OpCodes.Call),
							InstructionMask.MatchOpCode(OpCodes.Ldnull),
							InstructionMask.MatchOpCode(OpCodes.Call),
							InstructionMask.MatchOpCode(OpCodes.Brfalse),
							InstructionMask.MatchOpCode(OpCodes.Ldarg_0),
							InstructionMask.MatchOpCode(OpCodes.Ldfld),
							InstructionMask.MatchOpCode(OpCodes.Ldarg_0),
							InstructionMask.MatchOpCode(OpCodes.Call),
							InstructionMask.MatchOpCode(OpCodes.Callvirt),
							InstructionMask.MatchOpCode(OpCodes.Callvirt),
							InstructionMask.MatchOpCode(OpCodes.Ldarg_0),
							InstructionMask.MatchOpCode(OpCodes.Ldfld),
							InstructionMask.MatchOpCode(OpCodes.Ldarg_0),
							InstructionMask.MatchOpCode(OpCodes.Call),
							InstructionMask.MatchOpCode(OpCodes.Callvirt),
							InstructionMask.MatchOpCode(OpCodes.Callvirt),
							InstructionMask.MatchOpCode(OpCodes.Callvirt),
							InstructionMask.MatchOpCode(OpCodes.Br),
					},
					insertInstructions: new[] {
							new CodeInstruction(OpCodes.Ldarg_0),
							new CodeInstruction(OpCodes.Ldfld, field_greatCamera),
							new CodeInstruction(OpCodes.Ldarg_0),
							new CodeInstruction(OpCodes.Call, getter_avatarShell),
							new CodeInstruction(OpCodes.Call, method_UpdateCameraLookDirection),
					}
			);
			patch.ApplySafe(instructions, LoggerProvider.GetLogger());

			return instructions;
		}

	}

}
