using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BTHarmonyUtils.InstructionSearch;
using BTHarmonyUtils.TranspilerUtils;
using FirstPersonCamera.impl;
using GameWorld2;
using HarmonyLib;
using HeartLibs.heartlibs;
using JetBrains.Annotations;
using TingTing;

namespace FirstPersonCamera.patchers {

	[HarmonyPatch(declaringType: typeof(Door))]
	public class Door_Patch {

		[HarmonyPatch(methodName: nameof(Door.API_Goto))]
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> API_Goto_Transpiler(IEnumerable<CodeInstruction> codeInstructions) {
			List<CodeInstruction> instructions = codeInstructions.ToList();

			MethodInfo setter_direction = AccessTools.PropertySetter(typeof(Ting), nameof(Ting.direction));
			FieldInfo field_user = AccessTools.DeclaredField(typeof(Door), "_user");
			MethodInfo method_gotoHandler = SymbolExtensions.GetMethodInfo(() => API_Goto_Handler(null, null, null));

			CodeReplacementPatch patch = new CodeReplacementPatch(
					expectedMatches: 1,
					prefixInstructions: new[] {
							InstructionMask.MatchOpCode(OpCodes.Ldarg_0),
							InstructionMask.MatchOpCode(OpCodes.Ldfld),
							InstructionMask.MatchOpCode(OpCodes.Ldloc_0),
							InstructionMask.MatchOpCode(OpCodes.Callvirt),
							InstructionMask.MatchInstruction(OpCodes.Callvirt, setter_direction),
					},
					insertInstructions: new[] {
							new CodeInstruction(OpCodes.Ldarg_0),
							new CodeInstruction(OpCodes.Ldloc_0),
							new CodeInstruction(OpCodes.Ldarg_0),
							new CodeInstruction(OpCodes.Ldfld, field_user),
							new CodeInstruction(OpCodes.Call, method_gotoHandler),
					}
			);
			patch.ApplySafe(instructions, LoggerProvider.GetLogger());

			return instructions;
		}

		[UsedImplicitly]
		private static void API_Goto_Handler(Door doorGoneIn, Door doorComeOut, Character user) {
			if (user == null || user.name != WorldOwner.instance.world.settings.avatarName) {
				return;
			}
			CameraControls.SetTransitionDirection(doorGoneIn.direction, doorComeOut.direction);
		}

	}

}
