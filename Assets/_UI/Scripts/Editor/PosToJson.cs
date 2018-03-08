using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XPlugin.Data.Json;

public class PosToJson: MonoBehaviour {

	[MenuItem("Assets/生成路线点位置Json")]
	static void PosToJsonFunc() 
	{
		GameObject parent = Selection.activeObject as GameObject;
		var list = parent.GetComponentsInChildren<RectTransform>();

		List<RectTransform> rtList = new List<RectTransform>();
		foreach (var rt in list)
		{
			rtList.Add(rt);
		}
		rtList.Sort((p1,p2) =>int.Parse(p1.gameObject.name) - int.Parse(p2.gameObject.name));

		List<float[]> posList = new List<float[]>();
		for (int i = 0; i < rtList.Count; i++)
		{
			if (rtList[i].gameObject == parent)
			{
				continue;
			}
			posList.Add(new [] { rtList[i].position.x, rtList[i].position.y } );
		}
		string s = JsonMapper.ToJson(posList);
		Debug.Log("PointList json:" + s);
	}
}
