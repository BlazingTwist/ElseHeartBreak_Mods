using GameWorld2;
using JetBrains.Annotations;
using TingTing;

namespace HeartLibs.heartlibs {

	[PublicAPI]
	public static class CharacterExtension {

		public static void StartAction(this Character character, ActionName action, Ting otherObject, float timeUntilTrigger, float actionDuration) {
			character.StartAction(action.Value, otherObject, timeUntilTrigger, actionDuration);
		}

		public static bool IsDoingAction(this Character character, ActionName action) {
			return character.actionName == action.Value;
		}

		public static bool IsSeatedOrBedded(this Character character) {
			// for some reason characters can get seated without setting the 'sitting' flag...
			return character.sitting || character.laying || character.seat != null || character.bed != null;
		}

	}

}
