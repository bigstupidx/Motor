//
// DarkHoleTest.cs
//
// Author:
// [LongTianhong]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using UnityEngine;
using System.Collections;

namespace Game{
public class DarkHoleTest : MonoBehaviour {
	public float radius=50f;
	
	// Update is called once per frame
	void Update () {
		Rect nr = new Rect(Input.mousePosition.x-radius/2f, Input.mousePosition.y-radius/2f, radius,radius );
		DarkHole.ins.Set(nr);
	
	}
}
}
