//
// LTHShowOverdraw.cs
//
// Author:
// [LongTianhong]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using UnityEngine;
using System.Collections;

namespace Game {
	[ExecuteInEditMode]
	public class LTHShowOverdraw : MonoBehaviour {


		public bool fullOverdraw = false;
		public Shader shaderWithZ;
		public Shader shaderWithoutZ;

		private Color oldColor;
		private CameraClearFlags oldClear;

		void Start() {

		}

		void OnPreCull() {
			if (!enabled) {
				return;
			}
			oldColor = GetComponent<Camera>().backgroundColor;
			oldClear = GetComponent<Camera>().clearFlags;
			GetComponent<Camera>().backgroundColor = new Color(0f, 0f, 0f, 0f);
			GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
			GetComponent<Camera>().SetReplacementShader(fullOverdraw ? shaderWithoutZ : shaderWithZ, "RenderType");
		}

		void OnPostRender() {
			if (!enabled) {
				return;
			}
			GetComponent<Camera>().ResetReplacementShader();
			GetComponent<Camera>().backgroundColor = oldColor;
			GetComponent<Camera>().clearFlags = oldClear;
		}

	}
}
