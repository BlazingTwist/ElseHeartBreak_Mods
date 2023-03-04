using GameTypes;
using JetBrains.Annotations;
using UnityEngine;

namespace HeartLibs.heartlibs {

	[PublicAPI]
	public static class MathHelper {

		public static Vector3 DirectionToVector(Direction direction) {
			IntPoint intPointDir = IntPoint.DirectionToIntPoint(direction);
			return new Vector3(intPointDir.x, 0, intPointDir.y).normalized;
		}

	}

}
