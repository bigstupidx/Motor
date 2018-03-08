// Author:
// [LongTianhong]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XPlugin;

namespace Game {

	public class QuadMaskTest : MonoBehaviour {
		public float radius = 50f;

		// Update is called once per frame
		void Update() {
			Rect nr = new Rect(Input.mousePosition.x - radius / 2f, Input.mousePosition.y - radius / 2f, radius, radius);
			GetComponent<QuadMask>().Set(nr);
		}
	}
}