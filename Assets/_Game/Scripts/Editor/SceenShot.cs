using UnityEngine;
using System.Collections;
using UnityEditor;

public class SceenShot : MonoBehaviour {
	[MenuItem("Tools/拍摄小地图照片")]
	static void Menu() {
		SaveScreenshotToFile("Assets/a.png");
	}

	public static Texture2D TakeScreenShot() {
		return Screenshot();
	}

	static Texture2D Screenshot() {
		Camera camera = Selection.activeGameObject.GetComponent<Camera>();
		if (camera == null) {
			Debug.Log("需要选中一个相机");
			return null;
		}
		int resWidth = 1024 * 1;
		int resHeight = 1024 * 1;
		RenderTexture rt = new RenderTexture(resWidth, resHeight, 32);
		camera.targetTexture = rt;
		Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.ARGB32, false);
		camera.Render();
		RenderTexture.active = rt;
		screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
		screenShot.Apply();
		camera.targetTexture = null;
		RenderTexture.active = null; // JC: added to avoid errors
		DestroyImmediate(rt);

		var pixels = screenShot.GetPixels();
		for (int i = 0; i < pixels.Length; i++) {
			var pixel = pixels[i];
			var a = new Vector3(pixel.r, pixel.g, pixel.b).magnitude;
			pixel = new Color(150f,150f,150f);
			pixel.a = a;
			pixels[i] = pixel;
		}
		screenShot.SetPixels(pixels);
		return screenShot;
	}

	public static Texture2D SaveScreenshotToFile(string fileName) {
		Texture2D screenShot = Screenshot();
		byte[] bytes = screenShot.EncodeToPNG();
		System.IO.File.WriteAllBytes(fileName, bytes);
		Application.OpenURL(fileName);
		return screenShot;
	}
}
