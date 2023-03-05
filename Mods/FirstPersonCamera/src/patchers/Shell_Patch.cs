using HarmonyLib;
using HeartLibs.heartlibs;

namespace FirstPersonCamera.patchers {

	[HarmonyPatch(declaringType: typeof(Shell))]
	public class Shell_Patch {

		[HarmonyPatch(methodName: nameof(Shell.SnapShellToTingPosition))]
		[HarmonyPrefix]
		private static bool SnapShellToTingPosition_Prefix(Shell __instance) {
			CharacterShell characterShell = __instance as CharacterShell;
			if (characterShell == null || characterShell.character.name != WorldOwner.instance.world.settings.avatarName) {
				return true;
			}

			// don't snap while walking
			if (characterShell.character.IsDoingAction(ActionName.Walking)) {
				return false;
			}

			// also don't snap for minor distances
			float distance = (characterShell.transform.position - MimanHelper.TilePositionToVector3(characterShell.ting.localPoint)).magnitude;
			return !(distance < 1.5);
		}

	}

}
