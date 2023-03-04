using HeartLibs.heartlibs;
using GameWorld2;
using TingTing;
using UnityEngine;

namespace FirstPersonCamera.impl {

	public static class CharacterMovement {

		private static bool wasMoving;
		private static bool isMoving;

		private static Vector3 GetMovementInput(Vector3 forward, Vector3 right) {
			Vector3 offset = Vector3.zero;
			isMoving = false;
			if (Input.GetKey(KeyCode.I)) {
				isMoving = true;
				offset += forward;
			}
			if (Input.GetKey(KeyCode.K)) {
				isMoving = true;
				offset -= forward;
			}

			if (Input.GetKey(KeyCode.J)) {
				isMoving = true;
				offset -= right;
			}
			if (Input.GetKey(KeyCode.L)) {
				isMoving = true;
				offset += right;
			}
			return offset.normalized;
		}

		public static void ControlMovement(PlayerRoamingState state, GameViewControls controls, CharacterShell shell, NavMeshAgent agent) {
			Character character = shell.character;
			if (!character.IsDoingAction(ActionName.None) && !character.IsDoingAction(ActionName.Walking)) {
				// when doing a non-walking action, wait until finished.
				CheckStopMoving(character, agent);
				return;
			}
			if (character.conversationTarget != null) {
				// wait for conversation to end
				CheckStopMoving(character, agent);
				return;
			}

			Vector3 cameraForward = Vector3.Scale(controls.camera.transform.forward, new Vector3(1f, 0f, 1f)).normalized;
			Vector3 cameraRight = Vector3.Cross(Vector3.up, cameraForward);
			Vector3 offset = GetMovementInput(cameraForward, cameraRight);

			if (isMoving && !wasMoving) {
				if (HasToPrepareForWalk(character)) {
					isMoving = false;
					return;
				}
			}

			if (isMoving) {
				if (!character.IsDoingAction(ActionName.Walking)) {
					character.StartAction(ActionName.Walking, null, 99999f, 99999f);
				}

				character.running = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
				agent.Move(offset * Time.deltaTime * agent.speed);
				Transform agentTransform = agent.transform;
				if (!CameraControls.IsFirstPerson()) {
					float maxDegreesPerTick = agent.angularSpeed * Time.deltaTime * 5f;
					agentTransform.rotation = Quaternion.RotateTowards(
							agentTransform.rotation,
							Quaternion.LookRotation(offset, Vector3.up),
							maxDegreesPerTick
					);
				}
				character.position = new WorldCoordinate(character.room.name, MimanHelper.Vector3ToTilePoint(agentTransform.position));
			} else if (wasMoving && character.IsDoingAction(ActionName.Walking)) {
				character.StopAction();
				character.running = false;
				CheckStopMoving(character, agent);
			}
			wasMoving = isMoving;
		}

		private static void CheckStopMoving(Character character, NavMeshAgent agent) {
			if (!wasMoving) {
				return;
			}

			character.position = new WorldCoordinate(character.room.name, MimanHelper.Vector3ToTilePoint(agent.transform.position));
			wasMoving = false;
		}

		private static bool HasToPrepareForWalk(Character character) {
			if (character.sitting) {
				if (character.seat != null) {
					character.GetUpFromSeat();
				} else {
					character.sitting = false;
				}
			} else if (character.laying) {
				if (character.bed != null) {
					character.GetUpFromBed();
				} else {
					character.laying = false;
				}
			} else {
				character.seat = null;
				character.bed = null;
			}

			return !character.IsDoingAction(ActionName.None);
		}

	}

}
