//
// DarkHole.cs
//
// Author:
// [LongTianhong]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using System.Collections;
using UnityEngine;

namespace Game {
	public class DarkHole : MonoBehaviour {
		public static DarkHole ins;

		private Rect rect;

		private Mesh mesh;
		private Vector3[] verts;
		private Vector2 cornerScreen;
		private float screenRate;

		[Range(0,1)]
		public float Alpha=0.5f;

		public Camera cam;

		void Awake() {
			ins = this;
		}

		// Use this for initialization
		void Start() {
			if (cam == null) {
				Debug.LogError("camera is null ,darkhole will be disable!");

				return;
			}
			mesh = GetComponent<MeshFilter>().mesh;
			transform.localPosition = new Vector3(0f, 0f, cam.nearClipPlane+1);
			transform.CounteractLocalScale();

			Vector3[] corners = new Vector3[4];
			float depth = cam.nearClipPlane+1;
			corners[0] = cam.ViewportToWorldPoint(new Vector3(0f, 0f, depth));
			corners[1] = cam.ViewportToWorldPoint(new Vector3(0f, 1f, depth));
			corners[2] = cam.ViewportToWorldPoint(new Vector3(1f, 1f, depth));
			corners[3] = cam.ViewportToWorldPoint(new Vector3(1f, 0f, depth));
			for (int i = 0; i < corners.Length; i++) {
				corners[i] -= transform.position;
				corners[i].z = 0f;
			}
			float w = Vector3.Distance(corners[1], corners[2]);
			float h = Vector3.Distance(corners[0], corners[1]);
			cornerScreen = new Vector2(w, h);

			screenRate =(float)cam.pixelRect.width /w;

			//apply corners
			verts = mesh.vertices;
			verts[0] = corners[2];
			verts[1] = corners[1];
			verts[2] = corners[3];
			verts[3] = corners[0];

			mesh.vertices = verts;

			Color[] colors=new Color[12];
			for (int i = 0; i < colors.Length; i++) {
				colors[i]=new Color(1,1,1,this.Alpha);
			}
			this.mesh.colors = colors;
		}

		// Update is called once per frame
		void UpdateMesh() {
			verts[6] = verts[10] = new Vector3(rect.x, rect.y, 0f);//bottom left
			verts[9] = verts[5] = new Vector3(rect.x, rect.y + rect.height, 0f);//top left
			verts[4] = verts[8] = new Vector3(rect.x + rect.width, rect.y + rect.height, 0f);//top right
			verts[7] = verts[11] = new Vector3(rect.x + rect.width, rect.y, 0f);//bottom right
			mesh.vertices = verts;


			Color[] colors = this.mesh.colors;
			for (int i = 0; i < colors.Length; i++) {
				colors[i]=new Color(1,1,1,this.Alpha);
			}
			this.mesh.colors = colors;
		}

		public void Set(Rect rect) {
			this.Start();
			Vector3 viewPortPoint = cam.ScreenToViewportPoint(new Vector3(rect.x, rect.y, cam.nearClipPlane+1));
			viewPortPoint.x -= 0.5f;
			viewPortPoint.y -= 0.5f;
			this.rect.x = cornerScreen.x * viewPortPoint.x;
			this.rect.y = cornerScreen.y * viewPortPoint.y;

			this.rect.width = rect.width / this.screenRate;
			this.rect.height = rect.height / this.screenRate;
			UpdateMesh();
		}

		public void SetByWorld(Vector3 worldPos, Vector2 wh) {
			Vector3 screenPos = cam.WorldToScreenPoint(worldPos);
			Rect rect = new Rect(screenPos.x - wh.x / 2f, screenPos.y - wh.y / 2f, wh.x, wh.y);
			Set(rect);

		}



	}
}
