using System;
using System.Collections.Generic;
using System.Linq;
using HeartLibs.heartlibs;
using GameWorld2;
using TingTing;
using UnityEngine;

namespace FirstPersonCamera.impl {

	public static class CharacterMovement {

		private static bool wasMoving;
		private static bool isMoving;
		private static readonly List<Ting> walkTransitionsInRoom = new List<Ting>();
		private static readonly List<Ting> ignoreTransitionsUntilSteppedOut = new List<Ting>();

		// TODO verify that 'Lodge_Room1_DoorToRoom2' does not break the story
		private static readonly Dictionary<string, float> specialTransitionDepthOffsets = new Dictionary<string, float> {
				{ "DorisGardens_East_DoorToLodge", 1.75f }, // Tree-Door is unusually thick
				{ "Plaza_PortalToBurrowsNorth3", 1.4f }, // Shoe-Store entrance
		};

		private const float defaultTransitionDepthOffset = 1.2f;

		public static void OnRoomChanged(string newRoom, RoomRunner roomRunner) {
			walkTransitionsInRoom.Clear();
			ignoreTransitionsUntilSteppedOut.Clear();
			Room room = roomRunner.GetRoomUnsafe(newRoom);
			if (room == null) {
				return;
			}

			Type portalType = typeof(Portal);
			Type doorType = typeof(Door);
			walkTransitionsInRoom.AddRange(
					room.GetTings()
							.Where(ting => {
								Type tingType = ting.GetType();
								return tingType == portalType || tingType == doorType;
							})
			);
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

			Transform agentTransform = agent.transform;
			if (isMoving) {
				if (!character.IsDoingAction(ActionName.Walking)) {
					character.StartAction(ActionName.Walking, null, 99999f, 99999f);
				}

				character.running = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
				agent.Move(offset * Time.deltaTime * agent.speed);
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
			CheckSteppedThroughWalkTransition(character, agentTransform.position, agentTransform.forward);
		}

		private static Vector3 GetMovementInput(Vector3 forward, Vector3 right) {
			Vector3 offset = Vector3.zero;
			isMoving = false;
			if (Input.GetKey(KeyCode.W)) {
				isMoving = true;
				offset += forward;
			}
			if (Input.GetKey(KeyCode.S)) {
				isMoving = true;
				offset -= forward;
			}

			if (Input.GetKey(KeyCode.A)) {
				isMoving = true;
				offset -= right;
			}
			if (Input.GetKey(KeyCode.D)) {
				isMoving = true;
				offset += right;
			}
			return offset.normalized;
		}

		private static void CheckStopMoving(Character character, NavMeshAgent agent) {
			if (!wasMoving) {
				return;
			}

			character.position = new WorldCoordinate(character.room.name, MimanHelper.Vector3ToTilePoint(agent.transform.position));
			wasMoving = false;
		}

		private static void CheckSteppedThroughWalkTransition(Character character, Vector3 characterPosition, Vector3 characterForward) {
			IEnumerable<Ting> steppedThroughTransitions = walkTransitionsInRoom.Where(t =>
					ignoreTransitionsUntilSteppedOut.IndexOf(t) < 0 && IsTransitionInRange(characterPosition, t)
			);
			foreach (Ting transition in steppedThroughTransitions) {
				// TODO remove this message once all transitions have been tested
				LoggerProvider.GetLogger().LogWarning("stepping through transition, name: " + transition.name);
				character.InteractWith(transition);
				ignoreTransitionsUntilSteppedOut.Add(transition);
				break;
			}

			ignoreTransitionsUntilSteppedOut.RemoveAll(t => !IsTransitionInRange(characterPosition, t));
		}

		private static bool IsTransitionInRange(Vector3 characterPosition, Ting transition) {
			Vector3 tPos = MimanHelper.TilePositionToVector3(transition.position.localPosition);
			Vector3 tForward = MathHelper.DirectionToVector(transition.direction);
			float absDepthToTransition = Mathf.Abs(new Plane(tForward, tPos).GetDistanceToPoint(characterPosition));
			float maxDepthOffset = specialTransitionDepthOffsets.GetOrDefault(transition.name, defaultTransitionDepthOffset);
			if (absDepthToTransition > maxDepthOffset) {
				return false;
			}
			Vector3 tRight = Vector3.Cross(Vector3.up, tForward).normalized;
			float absWidthToTransition = Mathf.Abs(new Plane(tRight, tPos).GetDistanceToPoint(characterPosition));
			return absWidthToTransition < 1.75f;
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
