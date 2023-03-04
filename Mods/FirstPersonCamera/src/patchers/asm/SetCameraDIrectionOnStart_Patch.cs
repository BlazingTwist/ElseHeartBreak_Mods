using HarmonyLib;

namespace FirstPersonCamera.patchers.asm {

	[HarmonyPatch(declaringType: typeof(SetCameraDIrectionOnStart))]
	public class SetCameraDirectionOnStart_Patch {

		[HarmonyPatch(methodName: "Update")]
		[HarmonyPrefix]
		private static bool Update_Prefix(SetCameraDIrectionOnStart __instance) {
			// unfortunately this does not make the camera face away from the door,
			// instead it makes the camera point in whichever direction the designer deemed the most visually appealing
			// not very useful for a first person camera.
			UnityEngine.Object.Destroy(__instance.gameObject);
			return false;
		}

	}

}
