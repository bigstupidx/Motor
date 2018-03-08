using System;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshEditor))]
public class MeshEditorEditor : Editor {

	public int Choosed = 5;

	private Mesh m;

	void OnEnable() {
		MeshEditor editor = (MeshEditor)target;
		Mesh mesh = editor.GetComponent<MeshFilter>().sharedMesh;
		this.m = new Mesh();
		this.m.name = "editable mesh";
		this.m.vertices = mesh.vertices;
		this.m.bounds = mesh.bounds;
		this.m.uv = mesh.uv;
		this.m.triangles = mesh.triangles;
		this.m.RecalculateNormals();
		this.m.tangents = mesh.tangents;
		editor.GetComponent<MeshFilter>().sharedMesh = mesh;
	}

	void OnSceneGUI() {
		MeshEditor editor = (MeshEditor)target;
		if (editor.enabled) {
			Event currentEvent = Event.current;
			var verts = m.vertices;
			if (currentEvent.type == EventType.MouseMove && currentEvent.alt) {
				var ray = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit)) {
					float[] diss = new float[verts.Length];
					for (int i = 0; i < diss.Length; i++) {
						diss[i] = Vector3.Distance(hit.point, editor.transform.TransformPoint(verts[i]));
					}
					float min = Mathf.Infinity;
					for (int i = 0; i < diss.Length; i++) {
						if (diss[i] < min) {
							this.Choosed = i;
							min = diss[i];
						}
					}
				}
			}
			var pos = editor.transform.InverseTransformPoint(Handles.PositionHandle(editor.transform.TransformPoint(verts[this.Choosed]), Quaternion.identity));

			pos.x = (float)Math.Round(pos.x, 2);
			pos.y = (float)Math.Round(pos.y, 2);
			pos.z = (float)Math.Round(pos.z, 2);
			verts[this.Choosed] = pos;
			Handles.BeginGUI();
			GUILayout.Window(0, new Rect(10, 30, 130, 30), id => {
				verts[this.Choosed] = EditorGUILayout.Vector3Field("Pos", verts[this.Choosed]);
				                                                     if (GUILayout.Button("Collider"))
				                                                     {
								if (editor.GetComponent<MeshCollider>() != null) {
									editor.GetComponent<MeshCollider>().sharedMesh = m;
								}
				}
			}, "");
			Handles.EndGUI();
			this.m.vertices = verts;
			editor.GetComponent<MeshFilter>().sharedMesh = m;
//			if (editor.GetComponent<MeshCollider>() != null) {
//				editor.GetComponent<MeshCollider>().sharedMesh = m;
//			}



		}





	}

}
