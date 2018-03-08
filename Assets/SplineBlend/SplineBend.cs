using System.Linq;
using UnityEngine;

public enum SplineBendAxis {
	x,
	y,
	z
}

[ExecuteInEditMode]
public class SplineBend : MonoBehaviour {
	public SplineBendMarker[] markers;
	[HideInInspector]
	public Mesh initialRenderMesh;

	[HideInInspector]
	public Mesh renderMesh;

	[HideInInspector]
	public Mesh initialCollisionMesh;

	[HideInInspector]
	public Mesh collisionMesh;

	[HideInInspector]
	public int tiles;

	[HideInInspector]
	public float tileOffset;

	[HideInInspector]
	public bool dropToTerrain;

	[HideInInspector]
	public float terrainSeekDist;

	[HideInInspector]
	public LayerMask terrainLayer;

	[HideInInspector]
	public float terrainOffset;

	[HideInInspector]
	public bool equalize;

	[HideInInspector]
	public bool closed;

	[HideInInspector]
	private bool wasClosed;
	[HideInInspector]
	public Color VertexColor=Color.green;

	[HideInInspector]
	public float markerSize = 100;

	[HideInInspector]
	public bool displayRolloutOpen;

	[HideInInspector]
	public bool settingsRolloutOpen;

	[HideInInspector]
	public bool terrainRolloutOpen;

	[HideInInspector]
	public bool showExport;

	public SplineBendAxis axis;
	private Vector3 axisVector;

	public SplineBend() {
		this.tiles = 1;
		this.tileOffset = (float)-1;
		this.terrainSeekDist = (float)1000;
		this.equalize = true;
		this.markerSize = (float)1;
		this.axis = SplineBendAxis.z;
	}

	[ContextMenu("Reverset")]
	void Reverse() {
		this.markers= this.markers.Reverse().ToArray();
		this.UpdateNow();
	}

	void Reset() {
		UpdateNow();
	}
	public static Vector3 GetBeizerPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
		float num = (float)1 - t;
		return num * num * num * p0 + (float)3 * t * num * num * p1 + (float)3 * t * t * num * p2 + t * t * t * p3;
	}
	public static float GetBeizerLength(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
		float num = (float)0;
		Vector3 vector = p0;
		Vector3 vector2 = default(Vector3);
		for (float num2 = (float)0; num2 < 1.01f; num2 += 0.1f) {
			vector2 = SplineBend.GetBeizerPoint(p0, p1, p2, p3, num2);
			num += (vector - vector2).magnitude;
			vector = vector2;
		}
		return num;
	}
	public static float GetBeizerLength(SplineBendMarker marker1, SplineBendMarker marker2) {
//		float num = (marker2.position - marker1.position).magnitude * 0.5f;
		return SplineBend.GetBeizerLength(marker1.position, marker1.nextHandle + marker1.position, marker2.prewHandle + marker2.position, marker2.position);
	}
	public static Vector3 AlignPoint(SplineBendMarker marker1, SplineBendMarker marker2, float percent, Vector3 coords) {
//		float num = (marker2.position - marker1.position).magnitude * 0.5f;
		Vector3 beizerPoint = SplineBend.GetBeizerPoint(marker1.position, marker1.nextHandle + marker1.position, marker2.prewHandle + marker2.position, marker2.position, Mathf.Max((float)0, percent - 0.01f));
		Vector3 beizerPoint2 = SplineBend.GetBeizerPoint(marker1.position, marker1.nextHandle + marker1.position, marker2.prewHandle + marker2.position, marker2.position, Mathf.Min((float)1, percent + 0.01f));
		Vector3 beizerPoint3 = SplineBend.GetBeizerPoint(marker1.position, marker1.nextHandle + marker1.position, marker2.prewHandle + marker2.position, marker2.position, percent);
		Vector3 vector = beizerPoint - beizerPoint2;
		Vector3 vector2 = Vector3.Slerp(marker1.up, marker2.up, percent);
		Vector3 normalized = Vector3.Cross(vector, vector2).normalized;
		Vector3 normalized2 = Vector3.Cross(normalized, vector).normalized;
		Vector3 vector3 = new Vector3((float)1, (float)1, (float)1);
		if (marker1.expandWithScale || marker2.expandWithScale) {
			float num2 = percent * percent;
			float num3 = (float)1 - ((float)1 - percent) * ((float)1 - percent);
			float num4 = num3 * percent + num2 * ((float)1 - percent);
			vector3.x = marker1.transform.localScale.x * ((float)1 - num4) + marker2.transform.localScale.x * num4;
			vector3.y = marker1.transform.localScale.y * ((float)1 - num4) + marker2.transform.localScale.y * num4;
		}
		return beizerPoint3 + normalized * coords.x * vector3.x + normalized2 * coords.y * vector3.y;
	}
	public void BuildMesh(Mesh mesh, Mesh initialMesh, int num, float offset) {
		Vector3[] vertices = initialMesh.vertices;
		Vector2[] uv = initialMesh.uv;
//		Vector2[] uv2 = initialMesh.uv2;
		int[] triangles = initialMesh.triangles;
		Vector4[] tangents = initialMesh.tangents;
		Vector3[] array = new Vector3[vertices.Length * num];
		Vector2[] array2 = new Vector2[vertices.Length * num];
		Vector2[] uv3 = new Vector2[vertices.Length * num];
		Vector4[] array3 = new Vector4[vertices.Length * num];
		Color[] colors = new Color[vertices.Length * num];
//		bool flag = uv2.Length > 0;
		for (int i = 0; i < num; i++) {
			for (int j = 0; j < vertices.Length; j++) {
				array[i * vertices.Length + j] = vertices[j];
				array2[i * vertices.Length + j] = uv[j];
				array3[i * vertices.Length + j] = tangents[j];
				if (i == 0) {
					if (j == 1 || j == 2) {//为指示器添加透明顶点
						colors[i * vertices.Length + j] = new Color(this.VertexColor.r, this.VertexColor.g, this.VertexColor.b, 0);
					} else {
						colors[i * vertices.Length + j] = this.VertexColor;
					}
				} else if (i == num - 1) {
					if (j == 0 || j == 3) {//为指示器添加透明顶点
						colors[i * vertices.Length + j] = new Color(this.VertexColor.r, this.VertexColor.g, this.VertexColor.b, 0);
					} else {
						colors[i * vertices.Length + j] = this.VertexColor;
					}
				} else {
					colors[i * vertices.Length + j] = this.VertexColor;
				}
			}
		}
		int[] array4 = new int[triangles.Length * num];
		for (int i = 0; i < num; i++) {
			for (int j = 0; j < triangles.Length; j++) {
				array4[i * triangles.Length + j] = triangles[j] + vertices.Length * i;
			}
		}
		mesh.Clear();
		mesh.vertices = array;
		mesh.uv = array2;
		mesh.uv2 = uv3;
		mesh.triangles = array4;
		mesh.tangents = array3;
		mesh.colors = colors;
		mesh.RecalculateNormals();
	}
	public void RebuildMeshes() {
		if (this.renderMesh) {
			MeshFilter meshFilter = (MeshFilter)this.GetComponent(typeof(MeshFilter));
			if (!meshFilter) {
				return;
			}
			this.renderMesh.Clear();
			this.BuildMesh(this.renderMesh, this.initialRenderMesh, this.tiles, this.tileOffset);
			meshFilter.sharedMesh = this.renderMesh;
			this.renderMesh.RecalculateBounds();
			this.renderMesh.RecalculateNormals();
		}
		if (this.collisionMesh) {
			MeshCollider meshCollider = (MeshCollider)this.GetComponent(typeof(MeshCollider));
			if (meshCollider) {
				this.collisionMesh.Clear();
				this.BuildMesh(this.collisionMesh, this.initialCollisionMesh, this.tiles, this.tileOffset);
				meshCollider.sharedMesh = null;
				meshCollider.sharedMesh = this.collisionMesh;
				this.collisionMesh.RecalculateBounds();
				this.collisionMesh.RecalculateNormals();
			}
		}
	}
	public void Align(Mesh mesh, Mesh initialMesh) {
		Vector3[] array = new Vector3[mesh.vertexCount];
		Vector3[] vertices = initialMesh.vertices;
		for (int i = 0; i < this.tiles; i++) {
			for (int j = 0; j < vertices.Length; j++) {
				int num = i * vertices.Length + j;
				array[num] = vertices[j] + this.axisVector * this.tileOffset * (float)i;
				if (this.axis == SplineBendAxis.x) {
					array[num] = new Vector3(-array[num].z, array[num].y, array[num].x);
				} else if (this.axis == SplineBendAxis.y) {
					array[num] = new Vector3(-array[num].x, array[num].z, array[num].y);
				}
			}
		}
		float num2 = default(float);
		float num3 = float.PositiveInfinity;
		float num4 = float.NegativeInfinity;
		for (int j = 0; j < array.Length; j++) {
			num3 = Mathf.Min(num3, array[j].z);
			num4 = Mathf.Max(num4, array[j].z);
		}
		num2 = num4 - num3;
		for (int j = 0; j < array.Length; j++) {
			double num5 = (double)((array[j].z - num3) / num2);
			num5 = (double)Mathf.Clamp01((float)num5);
			if (Mathf.Approximately(num2, (float)0)) {
				num5 = (double)0;
			}
			int num6 = 0;
			for (int k = 1; k < markers.Length; k++) {
				if ((double)this.markers[k].percent >= num5) {
					num6 = k - 1;
					break;
				}
			}
			if (this.closed && num5 < (double)this.markers[1].percent) {
				num6 = 0;
			}
			float num7 = (float)((num5 - (double)this.markers[num6].percent) / (double)(this.markers[num6 + 1].percent - this.markers[num6].percent));
			if (this.closed && num5 < (double)this.markers[1].percent) {
				num7 = (float)(num5 / (double)this.markers[1].percent);
			}
			if (this.equalize) {
				int num8 = default(int);
				float num9 = default(float);
				for (int l = 1; l < this.markers[num6].subPoints.Length; l++) {
					if (this.markers[num6].subPointPercents[l] >= num7) {
						num8 = l - 1;
						break;
					}
				}
				num9 = (num7 - this.markers[num6].subPointPercents[num8]) * this.markers[num6].subPointFactors[num8];
				num7 = this.markers[num6].subPointMustPercents[num8] + num9;
			}
			array[j] = SplineBend.AlignPoint(this.markers[num6], this.markers[num6 + 1], num7, array[j]);
		}
		mesh.vertices = array;
	}
	public void FallToTerrain(Mesh mesh, Mesh initialMesh, float seekDist, LayerMask layer, float offset) {
		Vector3[] vertices = mesh.vertices;
		float[] array = new float[mesh.vertexCount];
		Vector3[] vertices2 = initialMesh.vertices;
		SplineBendAxis splineBendAxis = this.axis;
		if (splineBendAxis == SplineBendAxis.z || splineBendAxis == SplineBendAxis.x) {
			for (int i = 0; i < this.tiles; i++) {
				for (int j = 0; j < vertices2.Length; j++) {
					array[i * vertices2.Length + j] = vertices2[j].y;
				}
			}
		} else if (splineBendAxis == SplineBendAxis.y) {
			for (int i = 0; i < this.tiles; i++) {
				for (int j = 0; j < vertices2.Length; j++) {
					array[i * vertices2.Length + j] = vertices2[j].z;
				}
			}
		}
		int layer2 = this.gameObject.layer;
		this.gameObject.layer = 4;
		RaycastHit raycastHit;
		for (int j = 0; j < vertices.Length; j++) {
			Vector3 vector = this.transform.TransformPoint(vertices[j]);
			vector.y = this.transform.position.y;
			if (Physics.Raycast(vector + new Vector3((float)0, seekDist * 0.5f, (float)0), Vector3.down, out raycastHit, seekDist, layer)) {
				vertices[j].y = array[j] + this.transform.InverseTransformPoint(raycastHit.point).y + offset;
			}
		}
		this.gameObject.layer = layer2;
		mesh.vertices = vertices;
	}
	public void ResetMarkers() {
		this.ResetMarkers(markers.Length);
	}
	public void ResetMarkers(int count) {
		this.markers = new SplineBendMarker[count];
//		if (this.initialRenderMesh) {
//			Mesh mesh = this.initialRenderMesh;
//		} else if (this.initialCollisionMesh) {
//			Mesh mesh = this.initialCollisionMesh;
//		}
		Bounds bounds = default(Bounds);
		bool flag = default(bool);
		if (this.initialRenderMesh) {
			bounds = this.initialRenderMesh.bounds;
			flag = true;
		} else if (this.initialCollisionMesh) {
			bounds = this.initialCollisionMesh.bounds;
			flag = true;
		}
		if (!flag && (MeshFilter)this.GetComponent(typeof(MeshFilter))) {
			bounds = ((MeshFilter)this.GetComponent(typeof(MeshFilter))).sharedMesh.bounds;
			flag = true;
		}
		if (!flag && (MeshCollider)this.GetComponent(typeof(MeshCollider))) {
			bounds = ((MeshCollider)this.GetComponent(typeof(MeshCollider))).sharedMesh.bounds;
			flag = true;
		}
		if (!flag) {
			bounds = new Bounds(Vector3.zero, new Vector3((float)1, (float)1, (float)1));
		}
		float z = bounds.min.z;
		float num = bounds.size.z / (float)(count - 1);
		for (int i = 0; i < count; i++) {
			Transform transform = new GameObject("Marker" + i).transform;
			transform.parent = this.transform;
			transform.localPosition = (new Vector3((float)0, (float)0, z + num * (float)i));
			this.markers[i] = (SplineBendMarker)transform.gameObject.AddComponent(typeof(SplineBendMarker));
		}
	}
	public void AddMarker(Vector3 coords) {
		int prewMarkerNum = default(int);
		float num = float.PositiveInfinity;
		float num2 = default(float);
		for (int i = 0; i < markers.Length; i++) {
			num2 = (this.markers[i].position - coords).sqrMagnitude;
			if (num2 < num) {
				prewMarkerNum = i;
				num = num2;
			}
		}
		this.AddMarker(prewMarkerNum, coords);
	}
	public void AddMarker(Ray camRay) {
		float num = float.PositiveInfinity;
		int num2 = default(int);
		int num3 = default(int);
		for (int i = 0; i < markers.Length; i++) {
			SplineBendMarker splineBendMarker = this.markers[i];
			for (int j = 0; j < splineBendMarker.subPoints.Length; j++) {
				Vector3 vector = this.transform.TransformPoint(splineBendMarker.subPoints[j]);
				float num4 = Vector3.Dot(camRay.direction, (vector - camRay.origin).normalized) * (camRay.origin - vector).magnitude;
				float magnitude = (camRay.origin + camRay.direction * num4 - vector).magnitude;
				if (magnitude < num) {
					num2 = i;
					num3 = j;
					num = magnitude;
				}
			}
		}
		Vector3 vector2 = this.transform.TransformPoint(this.markers[num2].subPoints[num3]);
		float magnitude2 = (camRay.origin - vector2).magnitude;
		this.AddMarker(num2, camRay.origin + camRay.direction * magnitude2);
		this.UpdateNow();
		this.UpdateNow();
	}
	public void AddMarker(int prewMarkerNum, Vector3 coords) {
		SplineBendMarker[] array = new SplineBendMarker[markers.Length + 1];
		for (int i = 0; i < markers.Length; i++) {
			if (i <= prewMarkerNum) {
				array[i] = this.markers[i];
			} else {
				array[i + 1] = this.markers[i];
			}
		}
		Transform transform = new GameObject("Marker" + (prewMarkerNum + 1)).transform;
		transform.parent = this.transform;
		transform.position = coords;
		array[prewMarkerNum + 1] = (SplineBendMarker)transform.gameObject.AddComponent(typeof(SplineBendMarker));
		this.markers = array;
	}
	public void RefreshMarkers() {
		int num = 0;
		for (int i = 0; i < markers.Length; i++) {
			if (this.markers[i]) {
				num++;
			}
		}
		SplineBendMarker[] array = new SplineBendMarker[num];
		int num2 = 0;
		for (int i = 0; i < markers.Length; i++) {
			if (this.markers[i]) {
				array[num2] = this.markers[i];
				num2++;
			}
		}
		this.markers = array;
	}
	public void RemoveMarker(int num) {
		Object.DestroyImmediate(this.markers[num].gameObject);
		SplineBendMarker[] array = new SplineBendMarker[markers.Length - 1];
		for (int i = 0; i < markers.Length - 1; i++) {
			if (i < num) {
				array[i] = this.markers[i];
			} else {
				array[i] = this.markers[i + 1];
			}
		}
		this.markers = array;
	}
	public void CloseMarkers() {
		if (!this.closed && !(this.markers[0] == this.markers[markers.Length - 1])) {
			SplineBendMarker[] array = new SplineBendMarker[markers.Length + 1];
			for (int i = 0; i < markers.Length; i++) {
				array[i] = this.markers[i];
			}
			this.markers = array;
			this.markers[markers.Length - 1] = this.markers[0];
			this.UpdateNow();
			this.closed = true;
		}
	}
	public void UnCloseMarkers() {
		if (this.closed && !(this.markers[0] != this.markers[markers.Length - 1])) {
			SplineBendMarker[] array = new SplineBendMarker[markers.Length - 1];
			for (int i = 0; i < markers.Length - 1; i++) {
				array[i] = this.markers[i];
			}
			this.markers = array;
			this.UpdateNow();
			this.closed = false;
		}
	}

	//	public void OnEnable() {
	//		this.renderMesh = null;
	//		this.collisionMesh = null;
	//		this.ForceUpdate();
	//		MeshFilter meshFilter = (MeshFilter)this.GetComponent(typeof(MeshFilter));
	//		MeshCollider meshCollider = (MeshCollider)this.GetComponent(typeof(MeshCollider));
	//		if (this.renderMesh && meshFilter) {
	//			meshFilter.sharedMesh = this.renderMesh;
	//		}
	//		if (this.collisionMesh && meshCollider) {
	//			meshCollider.sharedMesh = null;
	//			meshCollider.sharedMesh = this.collisionMesh;
	//		}
	//	}
	//	public void OnDisable() {
	//		MeshFilter meshFilter = (MeshFilter)this.GetComponent(typeof(MeshFilter));
	//		MeshCollider meshCollider = (MeshCollider)this.GetComponent(typeof(MeshCollider));
	//		if (this.initialRenderMesh && meshFilter) {
	//			meshFilter.sharedMesh = this.initialRenderMesh;
	//		}
	//		if (this.initialCollisionMesh && meshCollider) {
	//			meshCollider.sharedMesh = null;
	//			meshCollider.sharedMesh = this.initialCollisionMesh;
	//		}
	//	}
	public void UpdateNow() {
		this.ForceUpdate(true);
	}
	public void ForceUpdate() {
		this.ForceUpdate(true);
	}
	public void ForceUpdate(bool refreshCollisionMesh) {
		MeshCollider meshCollider = (MeshCollider)this.GetComponent(typeof(MeshCollider));
		MeshFilter meshFilter = (MeshFilter)this.GetComponent(typeof(MeshFilter));
		SplineBendAxis splineBendAxis = this.axis;
		if (splineBendAxis == SplineBendAxis.x) {
			this.axisVector = new Vector3((float)1, (float)0, (float)0);
		} else if (splineBendAxis == SplineBendAxis.y) {
			this.axisVector = new Vector3((float)0, (float)1, (float)0);
		} else if (splineBendAxis == SplineBendAxis.z) {
			this.axisVector = new Vector3((float)0, (float)0, (float)1);
		}
		if (this.initialRenderMesh) {
			this.tiles = Mathf.Min(this.tiles, Mathf.FloorToInt(65000f / (float)this.initialRenderMesh.vertices.Length));
		} else if (this.initialCollisionMesh) {
			this.tiles = Mathf.Min(this.tiles, Mathf.FloorToInt(65000f / (float)this.initialCollisionMesh.vertices.Length));
		}
		this.tiles = Mathf.Max(this.tiles, 1);
		if (this.markers == null) {
			this.ResetMarkers(2);
		}
		for (int i = 0; i < markers.Length; i++) {
			if (!this.markers[i]) {
				this.RefreshMarkers();
			}
		}
		if (markers.Length < 2) {
			this.ResetMarkers(2);
		}
		for (int i = 0; i < markers.Length; i++) {
			this.markers[i].Init(this, i);
		}
		if (this.closed) {
			this.markers[0].dist = this.markers[markers.Length - 2].dist + SplineBend.GetBeizerLength(this.markers[markers.Length - 2], this.markers[0]);
		}
		float dist = this.markers[markers.Length - 1].dist;
		if (this.closed) {
			dist = this.markers[0].dist;
		}
		for (int i = 0; i < markers.Length; i++) {
			this.markers[i].percent = this.markers[i].dist / dist;
		}
		if (this.closed && !this.wasClosed) {
			this.CloseMarkers();
		}
		if (!this.closed && this.wasClosed) {
			this.UnCloseMarkers();
		}
		this.wasClosed = this.closed;
		if (meshFilter && !this.renderMesh) {
			if (!this.initialRenderMesh) {
				this.initialRenderMesh = meshFilter.sharedMesh;
			}
			if (this.initialRenderMesh) {
				if (this.tileOffset < (float)0) {
					this.tileOffset = this.initialRenderMesh.bounds.size.z;
				}
				this.renderMesh = (Mesh)Object.Instantiate(this.initialRenderMesh);
				meshFilter.sharedMesh = this.renderMesh;
			}
		}
		if (meshCollider && !this.collisionMesh) {
			if (!this.initialCollisionMesh) {
				this.initialCollisionMesh = meshCollider.sharedMesh;
			}
			if (this.initialCollisionMesh) {
				if (this.tileOffset < (float)0) {
					this.tileOffset = this.initialCollisionMesh.bounds.size.z;
				}
				this.collisionMesh = (Mesh)Object.Instantiate(this.initialCollisionMesh);
				meshCollider.sharedMesh = this.collisionMesh;
			}
		}
		if (this.renderMesh && this.initialRenderMesh && meshFilter) {
//			if (this.renderMesh.vertexCount != this.initialRenderMesh.vertexCount * this.tiles) {
				this.BuildMesh(this.renderMesh, this.initialRenderMesh, this.tiles, (float)0);
//			}
			this.Align(this.renderMesh, this.initialRenderMesh);
			if (this.dropToTerrain) {
				this.FallToTerrain(this.renderMesh, this.initialRenderMesh, this.terrainSeekDist, this.terrainLayer, this.terrainOffset);
			}
			this.renderMesh.RecalculateBounds();
			this.renderMesh.RecalculateNormals();
		}
		if (this.collisionMesh && this.initialCollisionMesh && meshCollider) {
//			if (this.collisionMesh.vertexCount != this.initialCollisionMesh.vertexCount * this.tiles) {
				this.BuildMesh(this.collisionMesh, this.initialCollisionMesh, this.tiles, (float)0);
//			}
			this.Align(this.collisionMesh, this.initialCollisionMesh);
			if (this.dropToTerrain) {
				this.FallToTerrain(this.collisionMesh, this.initialCollisionMesh, this.terrainSeekDist, this.terrainLayer, this.terrainOffset);
			}
			if (refreshCollisionMesh && meshCollider.sharedMesh == this.collisionMesh) {
				this.collisionMesh.RecalculateBounds();
				this.collisionMesh.RecalculateNormals();
				meshCollider.sharedMesh = null;
				meshCollider.sharedMesh = this.collisionMesh;
			}
		}

		if (this.renderMesh && meshFilter) {
			meshFilter.sharedMesh = this.renderMesh;
		}
		if (this.collisionMesh && meshCollider) {
			meshCollider.sharedMesh = null;
			meshCollider.sharedMesh = this.collisionMesh;
		}


	}
}
