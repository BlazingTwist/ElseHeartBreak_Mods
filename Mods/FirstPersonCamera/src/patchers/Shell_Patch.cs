using HarmonyLib;

namespace FirstPersonCamera.patchers {

	[HarmonyPatch(declaringType: typeof(Shell))]
	public class Shell_Patch {

		[HarmonyPatch(methodName: nameof(Shell.SnapShellToTingPosition))]
		[HarmonyPrefix]
		private static bool SnapShellToTingPosition_Prefix(Shell __instance) {
			CharacterShell characterShell = __instance as CharacterShell;
			if (characterShell != null && characterShell.character.name == WorldOwner.instance.world.settings.avatarName) {
				float distance = (characterShell.transform.position - MimanHelper.TilePositionToVector3(characterShell.ting.localPoint)).magnitude;
				if (distance < 1.5) {
					// skip player snapping for minor distances
					return false;
				}
			}
			return true;
		}

	}

}
