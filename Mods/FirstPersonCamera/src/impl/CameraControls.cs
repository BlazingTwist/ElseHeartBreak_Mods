using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using BepInEx.Logging;
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

		public static bool IsFirstPerson() {
			return isFirstPerson;
		}

		public static void UpdateCameraLookDirection(OrbitNewCameraState cameraState) {
			Vector3 lookDirection = Quaternion.Euler(cameraState.tilt, cameraState.currentAngle, 0f) * Vector3.forward;
			if (isFirstPerson && headBone != null) {
				cameraState.position = headBone.position;
			} else {
				cameraState.position = cameraState.lookTarget + new Vector3(0f, 4f, 0f) + (lookDirection * (-cameraState.radius));
			}
			cameraState.lookTarget = cameraState.position + (lookDirection * 25f);
		}

		public static void ControlCamera(GreatCamera camera, CharacterShell character) {
			UpdatePlayerShell(character);

			if (!Mathf.Approximately(Input.mouseScrollDelta.y, 0f)) {
				AddRadius(camera.orbit, -Input.mouseScrollDelta.y);
			}

			const float lookSensitivity = 0.03f * 7f;
			const float cameraDragRate = 140f;
			if (Input.GetMouseButton(1)) {
				camera.Input_Drag(Input.GetAxis("Horizontal") * lookSensitivity, Input.GetAxis("Vertical") * lookSensitivity);
			} else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) {
				camera.Input_Drag(Time.deltaTime * -cameraDragRate, 0f);
			} else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {
				camera.Input_Drag(Time.deltaTime * cameraDragRate, 0f);
			}
			if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) {
				camera.Input_Drag(0f, Time.deltaTime * -cameraDragRate);
			} else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) {
				camera.Input_Drag(0f, Time.deltaTime * cameraDragRate);
			}

			if (isFirstPerson && !(character.character.sitting || character.character.laying)) {
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
