using UnityEngine;
using System.Collections;
using UnityEditor;

public class AutoPack : AssetPostprocessor
{
	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
		string[] movedFromAssetPaths)
	{
		//改变和导入
		foreach (string s in importedAssets)
		{
			
			MonitorAtlasChange(s);
		}
		//删除
		foreach (string s in deletedAssets)
		{
			
			MonitorAtlasChange(s);
		}
		//移入目录
		foreach (string s in movedAssets)
		{
			
			MonitorAtlasChange(s);
		}
		//移出目录
		foreach (string s in movedFromAssetPaths)
		{
			
			MonitorAtlasChange(s);
		}
	}

	static void MonitorAtlasChange(string s)
	{
		if (s.Contains("_UI/Textures/Sprites"))
		{
			Debug.Log("发现sprite改变，自动打包图集"+ s);
			var atlas= Resources.FindObjectsOfTypeAll<Atlas>();
			foreach (var atlase in atlas)
			{
				Debug.Log("打包图集 " + atlase.name);
				atlase.Pack();
			}
		}

		//if (s.Contains ("_UI/Textures/Texture")) {
		//	Debug.Log ("发现Texture改变，自动打包图集" + s);
		//	var atlas = Resources.FindObjectsOfTypeAll<Atlas> ();
		//	foreach (var atlase in atlas) {
		//		Debug.Log ("打包图集 " + atlase.name);
		//		atlase.Pack ();
		//	}
		//}
	}
}