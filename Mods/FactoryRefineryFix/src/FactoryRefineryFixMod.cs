using System;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using JetBrains.Annotations;

namespace FactoryRefineryFix {

	[PublicAPI]
	[BepInPlugin(pluginGuid, pluginName, pluginVersion)]
	public class FactoryRefineryFixMod : BaseUnityPlugin {

		public const string pluginGuid = "blazingtwist.factoryrefineryfix";
		public const string pluginName = "Factory Refinery Fix";
		public const string pluginVersion = "1.0.0";

		private void Awake() {
			try {
				Logger.LogInfo("Applying patches...");
				Harmony harmony = new Harmony(pluginGuid);
				Assembly assembly = GetType().Assembly;
				harmony.PatchAll(assembly);
				BTHarmonyUtils.PatcherUtils.PatchAll(harmony, assembly);
				Logger.LogInfo("Patches applied.");
			} catch (Exception e) {
				Logger.LogError("Patches failed to exception:\n" + e);
			}
		}


	}

}
