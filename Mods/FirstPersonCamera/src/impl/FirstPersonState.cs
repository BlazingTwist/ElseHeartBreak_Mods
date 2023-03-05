using JetBrains.Annotations;

namespace FirstPersonCamera.impl {

	public class FirstPersonState : PlayerRoamingState {

		public static bool stateCursorLock;

		[UsedImplicitly]
		public static PlayerRoamingState Instantiate(bool pStoryStart) {
			return new FirstPersonState(pStoryStart);
		}

		private static void LockCursor(bool lockCursor) {
			stateCursorLock = lockCursor;
			CrossHair.CheckCursorLock();
		}

		private FirstPersonState(bool pStoryStart) : base(pStoryStart) { }

		public override void OnEnterBegin() {
			base.OnEnterBegin();
			LockCursor(true);
		}

		public override void OnExitBegin() {
			base.OnExitBegin();
			LockCursor(false);
		}

		public override void OnPaused() {
			base.OnPaused();
			LockCursor(false);
		}

		public override void OnResumed() {
			base.OnResumed();
			LockCursor(true);
		}

		public override void OnLatestUpdate() {
			base.OnLatestUpdate();
			stateCursorLock = true; // state resume does not work, so we need this crap.
		}

	}

}
