using System;
using System.Diagnostics;
using BepInEx.Logging;

namespace FirstPersonCamera {

	public static class FPCLogger {

		private static string GetLoggerName(Type containingClass) {
			return $"FPC_{containingClass.Name}";
		}

		public static ManualLogSource GetLogger() {
			Type containingClass = new StackFrame(1, false).GetMethod().ReflectedType;
			return Logger.CreateLogSource(GetLoggerName(containingClass));
		}

	}

}
