using UnityEngine;
using System.Collections;
using GameUI;
using UnityEditor;
using UnityEngine.UI;

public class AutoAddSfx : Editor {
	[MenuItem("Tools/自动添加按钮音效")]
	public static void AutoAddBtnSfx() {

		foreach (var gameObject in Selection.gameObjects) {
			_addBtnSfx(gameObject.transform);
		}
	}

	private static void _addBtnSfx(Transform transform) {
		if (transform.GetComponent<Button>() != null && transform.GetComponent<BtnAudio>() == null) {
			transform.gameObject.AddComponent<BtnAudio>();
			Debug.Log("add btn audio on " + transform.name);
		}

		foreach (Transform child in transform) {
			_addBtnSfx(child);
		}
	}

}
