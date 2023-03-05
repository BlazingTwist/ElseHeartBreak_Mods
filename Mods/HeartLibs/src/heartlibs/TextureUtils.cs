using System.IO;
using UnityEngine;

namespace HeartLibs.heartlibs {

	public static class TextureUtils {

		public static Texture2D LoadTexture(string path) {
			if (!File.Exists(path)) {
				return null;
			}

			Texture2D texture = new Texture2D(0, 0);
			return texture.LoadImage(File.ReadAllBytes(path)) ? texture : null;
		}

	}

}
