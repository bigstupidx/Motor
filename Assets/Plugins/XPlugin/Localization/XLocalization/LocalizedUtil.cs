using System;
using System.IO;
using System.Text.RegularExpressions;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

public static class LocalizedUtil {

	// 换行占位符
	public const string NEWLINE_PLACEHOLDER = "$$$";

	public static string ReplaceNewLine(string str) {
		return str.Replace("\r\n", NEWLINE_PLACEHOLDER)
			.Replace("\r", NEWLINE_PLACEHOLDER)
			.Replace("\n", NEWLINE_PLACEHOLDER);
	}
	public static string RecoverNewLine(string str) {
		return str.Replace(NEWLINE_PLACEHOLDER, "\r\n");
	}



	public static Regex RegexChineseStr = new Regex(@"[\u4e00-\u9fa5]");


	public static bool ContainChinese(this string str) {
		return RegexChineseStr.IsMatch(str);
	}

#if UNITY_EDITOR
	/// <summary>
	/// 获取物体相对于Resource的路径
	/// </summary>
	/// <param name="obj"></param>
	/// <returns></returns>
	public static string GetResourcePath(Object obj, bool withExt = false) {
		var path = AssetDatabase.GetAssetPath(obj);
		string dir = path;
		for (int i = 0; i < 100; i++) {//max search depth is 100
			dir = Path.GetDirectoryName(dir);
			if (dir == null) {
				break;
			}
			if (dir.EndsWith("Resources")) {
				break;
			} else if (dir == Path.GetPathRoot(path)) {
				Debug.LogError("这个物体不在resource下面，请检查============" + path);
				return null;
			}
		}
		var ret = path.Replace(dir + "/", "");
		if (!withExt) {
			ret = ret.Remove(ret.LastIndexOf("."));
		}
		return ret;
	}
#endif

	/// <summary>
	/// 将字符串本地化(扩展方法)
	/// </summary>
	/// <param name="str"></param>
	/// <returns></returns>
	public static string ToLocalized(this string str) {
		return LocalizedDict.Localized(str);
	}
}
