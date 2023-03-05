using FirstPersonCamera.impl;
using HarmonyLib;

namespace FirstPersonCamera.patchers {

	[HarmonyPatch(declaringType: typeof(TramRideState))]
	public class TramRideState_Patch {

		private static float previousRadius;

		[HarmonyPatch(methodName: nameof(TramRideState.OnUpdate))]
		[HarmonyPostfix]
		private static void OnUpdate_Postfix(GameViewControls ____controls) {
			CameraControls.ControlCamera(____controls.camera, null);
		}

		[HarmonyPatch(methodName: nameof(TramRideState.OnEnterBegin))]
		[HarmonyPostfix]
		private static void OnEnterBegin_Postfix(GameViewControls ____controls) {
			previousRadius = ____controls.camera.orbit.radius;
			____controls.camera.orbit.radius = 10f;
		}

		[HarmonyPatch(methodName: nameof(TramRideState.OnExitBegin))]
		[HarmonyPostfix]
		private static void OnExitBegin_Postfix(GameViewControls ____controls) {
			____controls.camera.orbit.radius = previousRadius;
		}

	}

}
