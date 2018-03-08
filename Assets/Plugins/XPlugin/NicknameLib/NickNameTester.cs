// Author:
// [LongTianhong]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace XPlugin {

	public class NickNameTester : MonoBehaviour {

		public string Name;

		[ContextMenu("Test")]
		void Start() {
			Name = new NicknameLibrary().Random;
		}
	}
}