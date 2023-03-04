using System.Collections.Generic;
using JetBrains.Annotations;

namespace HeartLibs.heartlibs {

	[PublicAPI]
	public static class DictionaryExtension {

		public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue fallback) {
			return dict.ContainsKey(key) ? dict[key] : fallback;
		}

	}

}
