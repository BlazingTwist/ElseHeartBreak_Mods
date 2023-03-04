using System.Diagnostics;
using System.Reflection;
using BepInEx.Logging;
using JetBrains.Annotations;

namespace HeartLibs.heartlibs {

	[PublicAPI]
	public static class LoggerProvider {

		public static ManualLogSource GetLogger() {
			MethodBase callingMethod = new StackFrame(1, false).GetMethod();
			return Logger.CreateLogSource(callingMethod.ReflectedType?.Name + "::" + callingMethod.Name);
		}

	}

}
