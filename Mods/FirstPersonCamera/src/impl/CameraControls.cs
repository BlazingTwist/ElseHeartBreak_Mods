using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using BepInEx.Logging;
using HeartLibs.heartlibs;
using JetBrains.Annotations;
using UnityEngine;

namespace FirstPersonCamera.impl {

	public static class CameraControls {

		private static readonly ManualLogSource logger = Logger.CreateLogSource(nameof(CameraControls));

		private static CharacterShell playerShell;
		private static Material[] defaultMaterials;
		private static Material transparentMaterial;
		private static bool isFirstPerson;
		private static SkinnedMeshRenderer renderer;
		private static Transform headBone;

		[UsedImplicitly]
		public static float getMinCameraTilt() {
			return -89.5f;
		}

		[UsedImplicitly]
		public static float getMaxCameraTilt() {
			return 89.5f;
		}

		[UsedImplicitly]
		public static Vector3 GetFixCameraPosition(Transform cameraPoint, Transform targetPoint) {
			Vector3 cameraPosition = cameraPoint.position;
			Vector3 screenPosition = targetPoint.position;
			Vector3 fromScreenToCamera = cameraPosition - screenPosition;
			Vector3 screenOffset = Vector3.Scale(fromScreenToCamera, new Vector3(0.4f, 0f, 0.4f));
			return screenPosition + screenOffset;
		}

		public static Vector3 GetCameraPoint(CharacterShell character) {
			return character.transform.position + (Vector3.up * 0.4f);
		}

		public static bool IsFirstPerson() {
			return isFirstPerson;
		}

		public static void UpdateCameraLookDirection(GreatCamera camera, Shell avatar) {
			if (camera == null || avatar == null) {
				return;
			}
			OrbitNewCameraState cameraState = camera.orbit;
			Quaternion lookRotation = Quaternion.Euler(cameraState.tilt, cameraState.currentAngle, 0f);
			Vector3 lookForward = lookRotation * Vector3.forward;
			Vector3 lookRight = lookRotation * Vector3.right;
			if (isFirstPerson && headBone != null) {
				cameraState.position = headBone.position;
			} else {
				float radiusOffset = cameraState.radius / 10f;
				cameraState.position = avatar.lookTargetPoint
						+ (Vector3.up * (4f + radiusOffset))
						+ (lookForward * (-cameraState.radius))
						+ (lookRight * (-radiusOffset));
			}
			cameraState.lookTarget = cameraState.position + (lookForward * 25f);
			camera.UpdateStates(Time.deltaTime);
		}

		public static void ControlCamera(GreatCamera camera, CharacterShell character) {
			CrossHair.SetRoamingCursorLocked(!Input.GetKey(KeyCode.LeftAlt));
			UpdatePlayerShell(character);

			if (!Mathf.Approximately(Input.mouseScrollDelta.y, 0f)) {
				AddRadius(camera.orbit, -Input.mouseScrollDelta.y);
			}

			const float lookSensitivity = 0.03f * 7f;
			const float cameraDragRate = 140f;
			if (!Input.GetKey(KeyCode.LeftAlt) || Input.GetMouseButton(1)) {
				camera.Input_Drag(Input.GetAxis("Horizontal") * lookSensitivity, Input.GetAxis("Vertical") * lookSensitivity);
			} else if (Input.GetKey(KeyCode.LeftArrow)) {
				camera.Input_Drag(Time.deltaTime * -cameraDragRate, 0f);
			} else if (Input.GetKey(KeyCode.RightArrow)) {
				camera.Input_Drag(Time.deltaTime * cameraDragRate, 0f);
			}
			if (Input.GetKey(KeyCode.UpArrow)) {
				camera.Input_Drag(0f, Time.deltaTime * -cameraDragRate);
			} else if (Input.GetKey(KeyCode.DownArrow)) {
				camera.Input_Drag(0f, Time.deltaTime * cameraDragRate);
			}

			if (isFirstPerson && !character.character.IsSeatedOrBedded()) {
				character.transform.forward = Vector3.Scale(camera.transform.forward, new Vector3(1f, 0f, 1f)).normalized;
			}
		}

		private static void UpdatePlayerShell(CharacterShell shell) {
			if (shell == null) {
				logger.LogWarning("Character was null!");
				return;
			}
			if (playerShell == shell) {
				return;
			}

			playerShell = shell;
			UpdateMaterial();
			if (isFirstPerson) {
				MakeHeadTransparent();
			}
		}

		[SuppressMessage("ReSharper", "Unity.PreferAddressByIdToGraphicsParams")]
		private static void UpdateMaterial() {
			renderer = playerShell.GetComponentInChildren<SkinnedMeshRenderer>();
			if (renderer == null) {
				logger.LogWarning("Unable to find SkinnedMeshRenderer!");
				return;
			}

			foreach (Transform bone in renderer.bones) {
				if (bone.name == "head_bn") {
					headBone = bone;
					break;
				}
			}

			Shader transparentShader = Shader.Find("Transparent/Diffuse");
			if (transparentShader == null) {
				logger.LogError("unable to find 'Transparent/Diffuse' shader.");
			}

			defaultMaterials = renderer.materials;
			if (transparentMaterial == null) {
				transparentMaterial = new Material(defaultMaterials[0]);
				transparentMaterial.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.One);
				transparentMaterial.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				transparentMaterial.SetInt("_ZWrite", 0);
				transparentMaterial.DisableKeyword("_ALPHATEST_ON");
				transparentMaterial.DisableKeyword("_ALPHABLEND_ON");
				transparentMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
				transparentMaterial.renderQueue = 3000;
				transparentMaterial.shader = transparentShader;
				transparentMaterial.color = new Color(0f, 0f, 0f, 0f);
				Object.DontDestroyOnLoad(transparentMaterial);
			}
		}

		private static void AddRadius(OrbitNewCameraState orbit, float offset) {
			SetRadius(orbit, offset + orbit.radius);
		}

		private static void SetRadius(OrbitNewCameraState orbit, float radius) {
			orbit.radius = Mathf.Clamp(radius, 0f, 10f);
			CheckIsFirstPerson(radius);
		}

		private static void CheckIsFirstPerson(float radius) {
			if (isFirstPerson && radius > 0f) {
				isFirstPerson = false;
				MakeHeadVisible();
			} else if (radius <= 0f) {
				isFirstPerson = true;
				MakeHeadTransparent();
			}
		}

		[UsedImplicitly]
		public static List<Material> AdjustMaterialList(List<Material> materials, CharacterShell character) {
			if (playerShell == null || character != playerShell || !isFirstPerson) {
				return materials;
			}

			materials[2] = transparentMaterial;
			materials[5] = transparentMaterial;
			materials[6] = transparentMaterial;
			return materials;
		}

		private static void MakeHeadTransparent() {
			Material[] materials = renderer.materials;
			materials[2] = transparentMaterial;
			materials[5] = transparentMaterial;
			materials[6] = transparentMaterial;
			renderer.materials = materials;
		}

		private static void MakeHeadVisible() {
			Material[] materials = renderer.materials;
			materials[2] = defaultMaterials[2];
			materials[5] = defaultMaterials[5];
			materials[6] = defaultMaterials[6];
			renderer.materials = materials;
		}

	}

}
