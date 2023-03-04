using BepInEx;
using JetBrains.Annotations;

namespace HeartLibs {

	[PublicAPI]
	[BepInPlugin(pluginGuid, pluginName, pluginVersion)]
	public class HeartLibsMod : BaseUnityPlugin {

		public const string pluginGuid = "blazingtwist.heartlibs";
		public const string pluginName = "HeartLibs";
		public const string pluginVersion = "1.0.0";

	}

}
