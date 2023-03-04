using HarmonyLib;

namespace FirstPersonCamera.patchers {

	[HarmonyPatch(declaringType: typeof(HideGroup))]
	public class HideGroup_Patch {
		
		[HarmonyPatch(methodType: MethodType.Getter, methodName: "ShouldShow")]
		[HarmonyPrefix]
		private static bool ShouldShow_Prefix(out bool __result) {
			__result = true;
			return false;
		}

		[HarmonyPatch(methodName: "Update")]
		[HarmonyPrefix]
		private static bool Update_Prefix(HideGroup __instance) {
			if (__instance.currentState == HideGroup.State.HIDING) {
				__instance.ShowWithNoFade();
			}
			return false;
		}

		[HarmonyPatch(methodName: nameof(HideGroup.HideWithNoFade))]
		private static bool HideWithNoFade_Prefix() {
			return false;
		}

	}

}
