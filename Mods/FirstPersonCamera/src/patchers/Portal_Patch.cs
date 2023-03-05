using System.Reflection;
using System.Reflection.Emit;
using BTHarmonyUtils.InstructionSearch;
using BTHarmonyUtils.MidFixPatch;
using FirstPersonCamera.impl;
using GameWorld2;
using HarmonyLib;
using TingTing;

namespace FirstPersonCamera.patchers {

	[HarmonyPatch(declaringType: typeof(Portal))]
	public class Portal_Patch {

		private static MidFixInstructionMatcher WalkThrough_Matcher() {
			MethodInfo setter_direction = AccessTools.PropertySetter(typeof(Ting), nameof(Ting.direction));
			return new MidFixInstructionMatcher(
					expectedMatches: 1,
					prefixInstructionSequence: new[] {
							InstructionMask.MatchOpCode(OpCodes.Ldarg_0),
							InstructionMask.MatchOpCode(OpCodes.Call),
							InstructionMask.MatchOpCode(OpCodes.Callvirt),
							InstructionMask.MatchInstruction(OpCodes.Callvirt, setter_direction),
					}
			);
		}

		[HarmonyPatch(methodName: nameof(Portal.WalkThrough))]
		[BTHarmonyMidFix(nameof(WalkThrough_Matcher))]
		private static void WalkThrough_MidFix(Portal __instance, Character pCharacter) {
			if (pCharacter == null || pCharacter.name != WorldOwner.instance.world.settings.avatarName) {
				return;
			}
			Portal portalComeOut = __instance.targetPortal;
			CameraControls.SetTransitionDirection(__instance.direction, portalComeOut.direction);
		}

	}

}
