using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx.Logging;
using BTHarmonyUtils.InstructionSearch;
using BTHarmonyUtils.TranspilerUtils;
using FirstPersonCamera.impl;
using HarmonyLib;
using JetBrains.Annotations;
using TingTing;
using UnityEngine;

namespace FirstPersonCamera.patchers {

	[HarmonyPatch(declaringType: typeof(CharacterShell))]
	public class CharacterShell_Patch {

		private static readonly ManualLogSource logger = Logger.CreateLogSource(nameof(CharacterShell_Patch));

		[UsedImplicitly]
		public static NavMeshAgent GetNavMeshAgent(CharacterShell shell) {
			throw new NotImplementedException("Harmony Patch not applied.");
		}

		[HarmonyPatch(declaringType: typeof(CharacterShell_Patch), methodName: nameof(GetNavMeshAgent))]
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> GetNavMeshAgent_Transpiler(IEnumerable<CodeInstruction> _) {
			FieldInfo field_agent = AccessTools.DeclaredField(typeof(CharacterShell), "_agent");
			return new[] {
					new CodeInstruction(OpCodes.Ldarg_0),
					new CodeInstruction(OpCodes.Ldfld, field_agent),
					new CodeInstruction(OpCodes.Ret),
			};
		}

		[HarmonyPatch(methodType: MethodType.Getter, methodName: nameof(CharacterShell.lookTargetPoint))]
		[HarmonyPrefix]
		private static bool GetLookTargetPoint_Prefix(CharacterShell __instance, out Vector3 __result) {
			__result = CameraControls.GetCameraPoint(__instance);
			return false;
		}

		[HarmonyPatch(methodName: "ShellUpdate")]
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> ShellUpdate_Transpiler(IEnumerable<CodeInstruction> codeInstructions) {
			// this disables snapping the character when moving with subTile precision
			List<CodeInstruction> instructions = codeInstructions.ToList();

			MethodInfo setter_position = AccessTools.PropertySetter(typeof(Ting), nameof(Ting.position));

			CodeReplacementPatch patch = new CodeReplacementPatch(
					expectedMatches: 1,
					targetInstructions: new[] {
							InstructionMask.MatchOpCode(OpCodes.Ldarg_0),
							InstructionMask.MatchOpCode(OpCodes.Call),
							InstructionMask.MatchOpCode(OpCodes.Ldarg_0),
							InstructionMask.MatchOpCode(OpCodes.Call),
							InstructionMask.MatchOpCode(OpCodes.Callvirt),
							InstructionMask.MatchOpCode(OpCodes.Callvirt),
							InstructionMask.MatchOpCode(OpCodes.Ldloc_S),
							InstructionMask.MatchOpCode(OpCodes.Newobj),
							InstructionMask.MatchInstruction(OpCodes.Callvirt, setter_position),
					}
			);
			patch.ApplySafe(instructions, logger);

			return instructions;
		}

		[HarmonyPatch(methodName: "RefreshGhostParticles")]
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> RefreshGhostParticles_Transpiler(IEnumerable<CodeInstruction> codeInstructions) {
			List<CodeInstruction> instructions = codeInstructions.ToList();

			MethodInfo method_ToArray = AccessTools.Method(typeof(List<Material>), "ToArray");
			MethodInfo method_AdJustMaterialList = SymbolExtensions.GetMethodInfo(() => CameraControls.AdjustMaterialList(null, null));

			CodeReplacementPatch patch = new CodeReplacementPatch(
					expectedMatches: 1,
					prefixInstructions: new[] {
							InstructionMask.MatchOpCode(OpCodes.Ldloc_3),
					},
					postfixInstructions: new[] {
							InstructionMask.MatchInstruction(OpCodes.Callvirt, method_ToArray),
					},
					insertInstructions: new[] {
							new CodeInstruction(OpCodes.Ldarg_0),
							new CodeInstruction(OpCodes.Call, method_AdJustMaterialList),
					}
			);
			patch.ApplySafe(instructions, logger);

			return instructions;
		}

	}

}
