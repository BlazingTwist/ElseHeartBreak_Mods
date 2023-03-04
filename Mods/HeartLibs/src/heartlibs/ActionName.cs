using JetBrains.Annotations;

namespace HeartLibs.heartlibs {

	[PublicAPI]
	public class ActionName {

		public static ActionName None => new ActionName("");
		public static ActionName Walking => new ActionName("Walking");
		public static ActionName Drinking => new ActionName("Drinking");
		public static ActionName TakingDrug => new ActionName("TakingDrug");
		public static ActionName TakingSnus => new ActionName("TakingSnus");
		public static ActionName SmokingCigarette => new ActionName("SmokingCigarette");
		public static ActionName LockedDoor => new ActionName("LockedDoor");
		public static ActionName UseDoorReallySoon => new ActionName("UseDoorReallySoon");
		public static ActionName WalkingThroughDoor => new ActionName("WalkingThroughDoor");
		public static ActionName WalkingThroughDoorPhase2 => new ActionName("WalkingThroughDoorPhase2");
		public static ActionName WalkingThroughPortal => new ActionName("WalkingThroughPortal");
		public static ActionName WalkingThroughPortalPhase2 => new ActionName("WalkingThroughPortalPhase2");
		public static ActionName WalkingThroughFence => new ActionName("WalkingThroughFence");
		public static ActionName UsingDoorWithKey => new ActionName("UsingDoorWithKey");
		public static ActionName PickingUp => new ActionName("PickingUp");
		public static ActionName Dropping => new ActionName("Dropping");
		public static ActionName DroppingFar => new ActionName("DroppingFar");
		public static ActionName PutHandItemIntoInventory => new ActionName("PutHandItemIntoInventory");
		public static ActionName TakeOutInventoryItem => new ActionName("TakeOutInventoryItem");
		public static ActionName LayingDown => new ActionName("LayingDown");
		public static ActionName FallingAsleep => new ActionName("FallingAsleep");
		public static ActionName FallingAsleepInChair => new ActionName("FallingAsleepInChair");
		public static ActionName FallAsleepFromStanding => new ActionName("FallAsleepFromStanding");
		public static ActionName GettingSeated => new ActionName("GettingSeated");
		public static ActionName GettingUpFromSeat => new ActionName("GettingUpFromSeat");
		public static ActionName GettingUpFromBed => new ActionName("GettingUpFromBed");
		public static ActionName PushingButtonOnHandItem => new ActionName("PushingButtonOnHandItem");
		public static ActionName StartingJukebox => new ActionName("StartingJukebox");
		public static ActionName PushingButton => new ActionName("PushingButton");
		public static ActionName UsingTv => new ActionName("UsingTv");
		public static ActionName TurnLeft => new ActionName("TurnLeft");
		public static ActionName TurnRight => new ActionName("TurnRight");
		public static ActionName KickingLamp => new ActionName("KickingLamp");
		public static ActionName Extracting => new ActionName("Extracting");
		public static ActionName UsingComputer => new ActionName("UsingComputer");
		public static ActionName SlurpingIntoComputer => new ActionName("SlurpingIntoComputer");
		public static ActionName Inspect => new ActionName("Inspect");
		public static ActionName ThrowingTingIntoTrashCan => new ActionName("ThrowingTingIntoTrashCan");
		public static ActionName PuttingTingIntoSendPipe => new ActionName("PuttingTingIntoSendPipe");
		public static ActionName PuttingTingIntoLocker => new ActionName("PuttingTingIntoLocker");
		public static ActionName GivingHandItem => new ActionName("GivingHandItem");
		public static ActionName BeingBothered => new ActionName("BeingBothered");
		public static ActionName Tasing => new ActionName("Tasing");
		public static ActionName GettingTased => new ActionName("GettingTased");
		public static ActionName Angry => new ActionName("Angry");
		public static ActionName UseSink => new ActionName("UseSink");
		public static ActionName RefillingDrink => new ActionName("RefillingDrink");
		public static ActionName Screwing => new ActionName("Screwing");
		public static ActionName Mixing => new ActionName("Mixing");
		public static ActionName UseStove => new ActionName("UseStove");
		public static ActionName TalkingInTelephone => new ActionName("TalkingInTelephone");
		public static ActionName ActivatingVendingMachine => new ActionName("ActivatingVendingMachine");
		public static ActionName Stealing => new ActionName("Stealing");

		public string Value { get; }

		private ActionName(string value) {
			Value = value;
		}

		public override string ToString() {
			return Value;
		}

	}

}
