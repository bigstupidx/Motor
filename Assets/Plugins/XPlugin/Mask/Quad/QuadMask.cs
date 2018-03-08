// Author:
// [LongTianhong]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace XPlugin {
	public class QuadMask : MonoBehaviour {

		public Camera Camera;
		public Material Material;

		private Rect rect;

		private Mesh mesh;
		private Vector3[] verts;
		private Vector2 cornerScreen;
		private float screenRate;

		void Start() {
			if (this.Camera == null) {
				this.Camera = GetComponent<Camera>();
				if (this.Camera == null) {
					Debug.LogError("camera not found");
				}
			}

			transform.localPosition = new Vector3(0f, 0f, this.Camera.nearClipPlane);

			Vector3[] corners = GetWorldCorners(this.Camera, this.Camera.nearClipPlane);
			for (int i = 0; i < corners.Length; i++) {
				corners[i] -= transform.position;
				corners[i].z = 0f;
			}
			float w = Vector3.Distance(corners[1], corners[2]);
			float h = Vector3.Distance(corners[0], corners[1]);
			cornerScreen = new Vector2(w, h);
			screenRate = (float)this.Camera.pixelRect.width / w;
			//apply corners
			verts = new Vector3[8];
			verts[0] = verts[4] = corners[0];
			verts[1] = verts[5] = corners[1];
			verts[2] = verts[6] = corners[2];
			verts[3] = verts[7] = corners[3];

			mesh = new Mesh();
			mesh.vertices = verts;
			mesh.triangles = new int[]{
				5,4,0,
				5,0,1,

				6,5,1,
				6,1,2,

				7,6,2,
				7,2,3,

				4,7,3,
				4,3,0,

			};

			MeshFilter meshFilter = GetComponent<MeshFilter>();
			if (meshFilter == null) {
				meshFilter = gameObject.AddComponent<MeshFilter>();
			}
			MeshRenderer renderer = GetComponent<MeshRenderer>();
			if (renderer == null) {
				renderer = gameObject.AddComponent<MeshRenderer>();
			}
			meshFilter.mesh = mesh;
			renderer.material = Material;
		}

		void UpdateMesh() {
			verts[4] = new Vector3(rect.x, rect.y, 0f);//bottom left
			verts[5] = new Vector3(rect.x, rect.y + rect.height, 0f);//top left
			verts[6] = new Vector3(rect.x + rect.width, rect.y + rect.height, 0f);//top right
			verts[7] = new Vector3(rect.x + rect.width, rect.y, 0f);//bottom right
			mesh.vertices = verts;
		}

		public void Set(Rect rect) {
			Vector3 viewPortPoint = this.Camera.ScreenToViewportPoint(new Vector3(rect.x, rect.y, this.Camera.nearClipPlane));
			viewPortPoint.x -= 0.5f;
			viewPortPoint.y -= 0.5f;
			this.rect.x = cornerScreen.x * viewPortPoint.x;
			this.rect.y = cornerScreen.y * viewPortPoint.y;

			this.rect.width = rect.width / screenRate;
			this.rect.height = rect.height / screenRate;

			UpdateMesh();
		}

		public void SetByWorld(Vector3 worldPos, Vector2 wh) {
			Vector3 screenPos = this.Camera.WorldToScreenPoint(worldPos);
			Rect rect = new Rect(screenPos.x - wh.x / 2f, screenPos.y - wh.y / 2f, wh.x, wh.y);
			Set(rect);

		}


#if UNITY_EDITOR
		static int mSizeFrame = -1;
		static System.Reflection.MethodInfo s_GetSizeOfMainGameView;
		static Vector2 mGameSize = Vector2.one;

		/// <summary>
		/// Size of the game view cannot be retrieved from Screen.width and Screen.height when the game view is hidden.
		/// </summary>
		static public Vector2 screenSize {
			get {
				int frame = Time.frameCount;

				if (mSizeFrame != frame || !Application.isPlaying) {
					mSizeFrame = frame;

					if (s_GetSizeOfMainGameView == null) {
						System.Type type = System.Type.GetType("UnityEditor.GameView,UnityEditor");
						s_GetSizeOfMainGameView = type.GetMethod("GetSizeOfMainGameView",
							System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
					}
					mGameSize = (Vector2)s_GetSizeOfMainGameView.Invoke(null, null);
				}
				return mGameSize;
			}
		}
#else
	/// <summary>
	/// Size of the game view cannot be retrieved from Screen.width and Screen.height when the game view is hidden.
	/// </summary>

	static public Vector2 screenSize { get { return new Vector2(Screen.width, Screen.height); } }
#endif

		static Vector3[] mSides = new Vector3[4];
		/// <summary>
		/// Get the camera's world-space corners. The order is bottom-left, top-left, top-right, bottom-right.
		/// </summary>
		static public Vector3[] GetWorldCorners(Camera cam, float depth, Transform relativeTo = null) {
			if (cam.orthographic) {
				float os = cam.orthographicSize;
				float x0 = -os;
				float x1 = os;
				float y0 = -os;
				float y1 = os;

				Rect rect = cam.rect;
				Vector2 size = screenSize;
				float aspect = size.x / size.y;
				aspect *= rect.width / rect.height;
				x0 *= aspect;
				x1 *= aspect;

				// We want to ignore the scale, as scale doesn't affect the camera's view region in Unity
				Transform t = cam.transform;
				Quaternion rot = t.rotation;
				Vector3 pos = t.position;

				mSides[0] = rot * (new Vector3(x0, y0, depth)) + pos;
				mSides[1] = rot * (new Vector3(x0, y1, depth)) + pos;
				mSides[2] = rot * (new Vector3(x1, y1, depth)) + pos;
				mSides[3] = rot * (new Vector3(x1, y0, depth)) + pos;
			} else {
				mSides[0] = cam.ViewportToWorldPoint(new Vector3(0f, 0f, depth));
				mSides[1] = cam.ViewportToWorldPoint(new Vector3(0f, 1f, depth));
				mSides[2] = cam.ViewportToWorldPoint(new Vector3(1f, 1f, depth));
				mSides[3] = cam.ViewportToWorldPoint(new Vector3(1f, 0f, depth));
			}

			if (relativeTo != null) {
				for (int i = 0; i < 4; ++i)
					mSides[i] = relativeTo.InverseTransformPoint(mSides[i]);
			}
			return mSides;
		}


	}




}