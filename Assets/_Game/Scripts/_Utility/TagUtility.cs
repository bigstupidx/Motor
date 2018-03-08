using UnityEngine;

public static class TagUtility {
	public static void SetTag(this GameObject g, string tag) {
		g.tag = tag;
	}

	public static void SetTagRecursion(this GameObject g, string tag) {
		g.tag = tag;
		foreach (Transform child in g.transform)
		{
			SetTagRecursion(child.gameObject, tag);
		}
	}
}
