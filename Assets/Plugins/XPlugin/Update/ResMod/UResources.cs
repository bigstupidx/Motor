using System;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace XPlugin.Update {
	public static class UResources {

		/// <summary>
		/// 请求一个文件
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static FileInfo ReqFile(string path) {
			if (ResManager.Ins.DownloadedFiles.ContainsKey(path)) {
				return ResManager.Ins.DownloadedFiles[path];
			}
			return null;
		}

		/// <summary>
		/// 请求场景
		/// </summary>
		/// <param name="path"></param>
		/// <param name="mode"></param>
		/// <returns></returns>
		public static bool ReqScene(string path) {
			return ResManager.Ins.ReqScene(path);
		}

		/// <summary>
		/// 异步请求场景
		/// </summary>
		/// <param name="path"></param>
		/// <param name="mode"></param>
		/// <param name="onDone"></param>
		public static void ReqSceneAsync(string path, Action<bool> onDone) {
			ResManager.Ins.ReqSceneAsync(path, onDone);
		}

		/// <summary>
		/// 加载物体
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static Object Load(string path) {
			return UResources.Load(path, typeof(Object));
		}
		public static T Load<T>(string path) where T : Object {
			return (T)((object)UResources.Load(path, typeof(T)));
		}
		public static Object Load(string path, Type type) {
			if (!Application.isPlaying) {
				return Resources.Load(path, type);
			}
			return ResManager.Ins.Load(path, type);
		}


		/// <summary>
		/// 异步加载物体
		/// </summary>
		/// <param name="path"></param>
		/// <param name="onDone"></param>
		public static void LoadAsync(string path, Action<Object> onDone) {
			LoadAsync(path, typeof(Object), onDone);
		}
		public static void LoadAsync<T>(string path, Action<Object> onDone) where T : Object {
			LoadAsync(path, typeof(T), onDone);
		}
		public static void LoadAsync(string path, Type type, Action<Object> onDone) {
			ResManager.Ins.LoadAsync(path, type, onDone);
		}


		/// <summary>
		/// 加载StreamingAssets物体（可更新）
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static WWW LoadStreamingAsset(string path) {
			return ResManager.Ins.LoadStreamingAsset(path);
		}

		/// <summary>
		/// 异步加载StreamingAssets物体（可更新）
		/// </summary>
		/// <param name="path"></param>
		/// <param name="onDone"></param>
		public static void LoadStreamingAssetAsync(string path, Action<WWW> onDone) {
			ResManager.Ins.LoadStreamingAssetAsync(path, onDone);
		}

		/*

		internal static T[] ConvertObjects<T>(Object[] rawObjects) where T : Object {
			if (rawObjects == null) {
				return null;
			}
			T[] array = new T[rawObjects.Length];
			for (int i = 0; i < array.Length; i++) {
				array[i] = (T)((object)rawObjects[i]);
			}
			return array;
		}



		public static ResourceRequest LoadAsync(string path) {
			return UResources.LoadAsync(path, typeof(Object));
		}
		public static ResourceRequest LoadAsync<T>(string path) where T : Object {
			return UResources.LoadAsync(path, typeof(T));
		}
		public static ResourceRequest LoadAsync(string path, Type type) {
			return Resources.LoadAsync(path, type);
		}




		public static Object[] FindObjectsOfTypeAll(Type type) {
			return Resources.FindObjectsOfTypeAll(type);
		}
		public static T[] FindObjectsOfTypeAll<T>() where T : Object {
			return UResources.ConvertObjects<T>(UResources.FindObjectsOfTypeAll(typeof(T)));
		}


		public static Object[] LoadAll(string path) {
			return UResources.LoadAll(path, typeof(Object));
		}
		public static T[] LoadAll<T>(string path) where T : Object {
			return UResources.ConvertObjects<T>(UResources.LoadAll(path, typeof(T)));
		}
		public static Object[] LoadAll(string path, Type systemTypeInstance) {
			return Resources.LoadAll(path, systemTypeInstance);
		}


		public static Object GetBuiltinResource(Type type, string path) {
			return Resources.GetBuiltinResource(type, path);
		}
		public static T GetBuiltinResource<T>(string path) where T : Object {
			return (T)((object)UResources.GetBuiltinResource(typeof(T), path));
		}


		public static Object LoadAssetAtPath(string assetPath, Type type) {
			return Resources.LoadAssetAtPath(assetPath, type);
		}
		public static T LoadAssetAtPath<T>(string assetPath) where T : Object {
			return (T)((object)UResources.LoadAssetAtPath(assetPath, typeof(T)));
		}


		public static void UnloadAsset(Object assetToUnload) {
			Resources.UnloadAsset(assetToUnload);
		}

		public static AsyncOperation UnloadUnusedAssets() {
			return Resources.UnloadUnusedAssets();
		}

		*/
	}
}
