using System;
using System.IO;
using System.Reflection;
using BepInEx;
using FirstPersonCamera.impl;
using HarmonyLib;
using HeartLibs;
using JetBrains.Annotations;

namespace FirstPersonCamera {

	[PublicAPI]
	[BepInPlugin(pluginGuid, pluginName, pluginVersion)]
	[BepInDependency(HeartLibsMod.pluginGuid, "1.0.0")]
	public class FirstPersonCameraMod : BaseUnityPlugin {

		public const string pluginGuid = "blazingtwist.firstpersoncamera";
		public const string pluginName = "BT FPC";
		public const string pluginVersion = "1.0.0";

		public static string SpritePath => Path.Combine(Paths.ConfigPath, "fpc/sprites/");

		private void Awake() {
			try {
				Logger.LogInfo("Applying patches...");
				Harmony harmony = new Harmony(pluginGuid);
				Assembly fpcAssembly = GetType().Assembly;
				harmony.PatchAll(fpcAssembly);
				BTHarmonyUtils.PatcherUtils.PatchAll(harmony, fpcAssembly);
				Logger.LogInfo("Patches applied.");
			} catch (Exception e) {
				Logger.LogError("Patches failed to exception:\n" + e);
			}

			CrossHair.AttachToScene();
		}

	}

}
