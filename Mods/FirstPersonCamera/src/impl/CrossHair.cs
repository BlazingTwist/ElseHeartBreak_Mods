using System;
using System.IO;
using HeartLibs.heartlibs;
using UnityEngine;
using UnityEngine.UI;

namespace FirstPersonCamera.impl {

	public class CrossHair : MonoBehaviour {

		private static GameObject targetParent;
		private static bool shouldLockCursor;

		public static void AttachToScene() {
			if (targetParent != null) {
				// already attached
				return;
			}

			try {
				targetParent = new GameObject("CrossHairRoot");
				targetParent.AddComponent<CrossHair>();
				DontDestroyOnLoad(targetParent);
				SetVisible(false);
			} catch (Exception e) {
				LoggerProvider.GetLogger().LogError("Attaching CrossHair failed! Error: " + e);
			}
		}

		public static void CheckCursorLock() {
			SetRoamingCursorLocked(!Input.GetKey(KeyCode.LeftAlt));
		}

		public static void SetRoamingCursorLocked(bool lockCursor) {
			lockCursor = lockCursor && FirstPersonState.stateCursorLock;
			if (shouldLockCursor == lockCursor) {
				return;
			}
			shouldLockCursor = lockCursor;
			Screen.lockCursor = lockCursor;
			Screen.showCursor = !lockCursor;
			SetVisible(lockCursor);
		}

		private static void SetVisible(bool visible) {
			targetParent.SetActive(visible);
		}

		private void OnApplicationFocus(bool hasFocus) {
			if (hasFocus && shouldLockCursor) {
				Screen.lockCursor = true;
			}
		}

		private void Start() {
			LoadCrossHair();
		}

		private void LoadCrossHair() {
			GameObject myGo = gameObject;
			Canvas canvas = myGo.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.pixelPerfect = false;

			CanvasScaler scaler = myGo.AddComponent<CanvasScaler>();
			scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			scaler.referenceResolution = new Vector2(1920, 1080);
			scaler.referencePixelsPerUnit = 100;
			scaler.scaleFactor = 0.6f;

			GameObject crossHairObject = new GameObject("CrossHairObj");
			crossHairObject.transform.parent = myGo.transform;

			crossHairObject.AddComponent<CanvasRenderer>();
			Image crossHairImage = crossHairObject.AddComponent<Image>();

			crossHairImage.type = Image.Type.Simple;
			Texture2D texture = LoadTexture("C:\\Users\\mhFre\\Documents\\crossHair.png");
			crossHairImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero, 100f);
			crossHairImage.color = new Color(221f / 255f, 69f / 255f, 230f / 255f);

			RectTransform crossHairTransform = crossHairObject.GetComponent<RectTransform>();
			crossHairTransform.anchorMin = new Vector2(0.5f, 0.5f);
			crossHairTransform.anchorMax = new Vector2(0.5f, 0.5f);
			crossHairTransform.pivot = new Vector2(0.5f, 0.5f);
			crossHairTransform.anchoredPosition = Vector2.zero;
			crossHairTransform.sizeDelta = new Vector2(16, 16);
		}

		private static Texture2D LoadTexture(string path) {
			if (!File.Exists(path)) {
				return null;
			}

			Texture2D texture = new Texture2D(0, 0);
			return texture.LoadImage(File.ReadAllBytes(path)) ? texture : null;
		}

	}

}
