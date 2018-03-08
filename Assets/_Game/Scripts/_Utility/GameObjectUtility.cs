//
// GameObjectUtility.cs
//
// Author:
// [LongTianhong]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using UnityEngine;
using XPlugin.Update;
using Object = UnityEngine.Object;

namespace Game {
	public static class GameObjectUtility {

		public static void SetAllChildActive(this Transform transform, bool enable) {
			foreach (Transform child in transform) {
				child.gameObject.SetActive(enable);
			}
		}

		public static GameObject LoadAndIns(string name) {
			return LoadAndIns(name, Vector3.zero, Quaternion.identity);
		}

		public static GameObject LoadAndIns(string name, Vector3 pos, Quaternion rot) {
			GameObject res = UResources.Load<GameObject>(name);
			return (GameObject.Instantiate(res, pos, rot) as GameObject);
		}

		public static void SetGameObjectParent(GameObject g, Transform parent, bool lossyScaleOne = false) {
			g.transform.SetParent(parent);
			g.transform.localPosition = Vector3.zero;
			g.transform.localRotation = Quaternion.identity;
			if (!lossyScaleOne) {
				g.transform.localScale = Vector3.one;
			}
		}


	}
}
