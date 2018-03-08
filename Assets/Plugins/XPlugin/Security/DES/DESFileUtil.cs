using UnityEngine;
using System.Collections;
using System.IO;

namespace XPlugin.Security {
	public static class DESFileUtil {

		public static string ReadAllText(string path, string key, string iv) {
			var text = File.ReadAllText(path);
			return DESUtil.Decrypt(text, key, iv);
		}

		public static void WriteAllText(string path, string text, string key, string iv) {
			var bs = DESUtil.Encrypt(text, key, iv);
			File.WriteAllText(path, bs);
		}

		public static byte[] ReadAllBytes(string path, string key, string iv) {
			var text = File.ReadAllBytes(path);
			return DESUtil.Decrypt(text, key, iv);
		}

		public static void WriteAllBytes(string path, byte[] bytes, string key, string iv) {
			var bs = DESUtil.Encrypt(bytes, key, iv);
			File.WriteAllBytes(path, bs);
		}


	}
}
