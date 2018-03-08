using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SplineBend))]
public class SplineBendEditor : Editor {
	public int maxTiles;
	public bool blockingMouseInput;
	public SerializedProperty markers_SP;
	public SerializedProperty markerSize_SP;
	public SerializedProperty equalize_SP;
	public SerializedProperty closed_SP;
	public SerializedProperty color_SP;
	public SerializedProperty initialRenderMesh_SP;
	public SerializedProperty initialCollisionMesh_SP;
	public SerializedProperty renderMesh_SP;
	public SerializedProperty collisionMesh_SP;
	public SerializedProperty tiles_SP;
	public SerializedProperty tileOffSP;
	public SerializedProperty dropToTerrain_SP;
	public SerializedProperty terrainSeekDist_SP;
	public SerializedProperty terrainLayer_SP;
	public SerializedProperty terrainOffSP;
	public SplineBendEditor() {
		this.maxTiles = -1;
	}
	public void CalculateMaxTiles() {
		int maxCol = 0; int maxRen = 0;
		var target = (SplineBend)this.target;
		if (!!target.initialCollisionMesh) maxCol = Mathf.FloorToInt(65000 / target.initialCollisionMesh.vertexCount);
		if (!!target.initialRenderMesh) maxRen = Mathf.FloorToInt(65000 / target.initialRenderMesh.vertexCount);
		maxTiles = Mathf.Min(maxCol, maxRen);
	}
	public void OnEnable() {
		this.markers_SP = this.serializedObject.FindProperty("markers");
		this.markerSize_SP = this.serializedObject.FindProperty("markerSize");
		this.closed_SP = this.serializedObject.FindProperty("closed");
		this.color_SP = this.serializedObject.FindProperty("VertexColor");
		this.equalize_SP = this.serializedObject.FindProperty("equalize");
		this.initialRenderMesh_SP = this.serializedObject.FindProperty("initialRenderMesh");
		this.initialCollisionMesh_SP = this.serializedObject.FindProperty("initialCollisionMesh");
		this.renderMesh_SP = this.serializedObject.FindProperty("renderMesh");
		this.collisionMesh_SP = this.serializedObject.FindProperty("collisionMesh");
		this.tiles_SP = this.serializedObject.FindProperty("tiles");
		this.tileOffSP = this.serializedObject.FindProperty("tileOffset");
		this.dropToTerrain_SP = this.serializedObject.FindProperty("dropToTerrain");
		this.terrainSeekDist_SP = this.serializedObject.FindProperty("terrainSeekDist");
		this.terrainLayer_SP = this.serializedObject.FindProperty("terrainLayer");
		this.terrainOffSP = this.serializedObject.FindProperty("terrainOffset");
	}


	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		serializedObject.Update();
		var target = (SplineBend)this.target;
		//remaking markers and updating if any marker is missing
		for (int m = 0; m < target.markers.Length; m++) {
			if (!target.markers[m]) {
				target.UpdateNow(); break;
			}
		}

		EditorGUILayout.PropertyField(markerSize_SP, new GUIContent("Marker Size", "Size of markers mesh cones"));
		EditorGUILayout.PropertyField(closed_SP, new GUIContent("Closed"));
		EditorGUILayout.PropertyField(this.color_SP, new GUIContent("Color"));
		EditorGUILayout.PropertyField(equalize_SP, new GUIContent("Equalize"));

		EditorGUILayout.LabelField("Meshes");
		EditorGUILayout.PropertyField(initialRenderMesh_SP, new GUIContent("\t Initial Render Mesh"));
		EditorGUILayout.PropertyField(initialCollisionMesh_SP, new GUIContent("\t Initial Collision Mesh"));
		EditorGUILayout.PropertyField(renderMesh_SP, new GUIContent("\t Render Mesh"));
		EditorGUILayout.PropertyField(collisionMesh_SP, new GUIContent("\t Collision Mesh"));

		EditorGUILayout.LabelField("Tiles");
		EditorGUILayout.PropertyField(tiles_SP, new GUIContent("\t Tile Count", "Mesh geometry repeats itself N times"));
		EditorGUILayout.PropertyField(this.tileOffSP, new GUIContent("\t Tile Offset", "Distance between tiles in Z axis"));

		Rect rect = GUILayoutUtility.GetRect(10, 18, "TextField");
		rect.x += 30; rect.width -= 60;

		if (GUI.Button(rect, "Reset Tile Offset")) {
			SplineBendAxis targetAxis = target.axis;
			if (!!target.initialRenderMesh) {
				switch (targetAxis) {
					case SplineBendAxis.z: target.tileOffset = target.initialRenderMesh.bounds.size.z; break;
					case SplineBendAxis.y: target.tileOffset = target.initialRenderMesh.bounds.size.y; break;
					case SplineBendAxis.x: target.tileOffset = target.initialRenderMesh.bounds.size.x; break;
				}
			} else if (!!target.initialCollisionMesh) {
				switch (targetAxis) {
					case SplineBendAxis.z: target.tileOffset = target.initialCollisionMesh.bounds.size.z; break;
					case SplineBendAxis.y: target.tileOffset = target.initialCollisionMesh.bounds.size.y; break;
					case SplineBendAxis.x: target.tileOffset = target.initialCollisionMesh.bounds.size.x; break;
				}
			}
		}

		EditorGUILayout.LabelField("Drop to Terrain");
		EditorGUILayout.PropertyField(dropToTerrain_SP, new GUIContent("\t Drop", "Places mesh at the surface of terrain or other collision mesh"));
		EditorGUILayout.PropertyField(terrainSeekDist_SP, new GUIContent("\t Seek Distance", "Seeks for terrain within this Y distance"));
		EditorGUILayout.PropertyField(terrainLayer_SP, new GUIContent("\t Terrain Layer", "Layer of the terrain object"));
		EditorGUILayout.PropertyField(this.terrainOffSP, new GUIContent("\t Height Offset", "Raises (or lowers) mesh above terrain"));

		if (GUILayout.Button("Update Now")) {
			target.UpdateNow();
		}

		target.showExport = EditorGUILayout.Foldout(target.showExport, "Export");
		if (target.showExport)
		{
			rect = GUILayoutUtility.GetRect (10, 18, "TextField");
			rect.x += 30; rect.width -= 30;
			if (GUI.Button(rect, "Export To Obj")) { target.ForceUpdate(); ExportToObj(target); }

			rect = GUILayoutUtility.GetRect (10, 18, "TextField");
			rect.x += 30; rect.width -= 30;
			if (GUI.Button(rect, "Export And Assign")) 
			{
				target.ForceUpdate();
				var localPath  = ExportToObj(target);
				Transform asset  = AssetDatabase.LoadAssetAtPath(localPath, typeof(Transform)) as Transform;
				if (!asset) { Debug.Log("Could not load exported asset. Please make sure it was exported inside 'Assets' folder."); return; }

				target.enabled = false;

				Transform renderTfm  = asset.Find(target.transform.name + "_render");
				if (!!renderTfm)
				{ 
					MeshFilter targetFilter = target.GetComponent<MeshFilter>();
					if (!!targetFilter) targetFilter.mesh = renderTfm.GetComponent<MeshFilter>().sharedMesh;
				}

				Transform collisionTfm = asset.Find(target.transform.name + "_collision");
				if (!!collisionTfm)
				{ 
					MeshCollider targetCollider = target.GetComponent<MeshCollider>();
					if (!!targetCollider) targetCollider.sharedMesh = collisionTfm.GetComponent<MeshFilter>().sharedMesh;
				}
			}

			//EditorGUILayout.PropertyField (objFile_SP, new GUIContent("\t Object file (.obj):"));
		}

		serializedObject.ApplyModifiedProperties();

		//updating
		if (GUI.changed) {
			if (target.dropToTerrain && Tools.pivotMode == PivotMode.Center) {
				Tools.pivotMode = PivotMode.Pivot;
			}
			target.ForceUpdate();
		}
	}



	public void OnSceneGUI() {
		DrawMarkers((SplineBend)this.target);
	}

	public static void DrawMarkers(SplineBend splineBend) {
		//Debug.LogError ("type:" + (int)Event.current.type + "   button:" + Event.current.button);
		if (Event.current.type == 0 && Event.current.button == 1 && Event.current.control) {
			Ray camRay = Camera.current.ScreenPointToRay(new Vector2(Event.current.mousePosition.x, Camera.current.pixelHeight - Event.current.mousePosition.y));
			splineBend.AddMarker(camRay);
		}
		Handles.matrix = (splineBend.transform.localToWorldMatrix);
		for (int i = 0; i < (splineBend.markers.Length); i++) {
			Handles.color = Color.green;
			Handles.DrawCapFunction func = Handles.SphereCap;
			SplineBendMarker.MarkerType type = splineBend.markers[i].type;
			if (type == SplineBendMarker.MarkerType.Smooth) {
				func = Handles.SphereCap;
			} else if (type == SplineBendMarker.MarkerType.Transform) {
				func = Handles.ConeCap;
			}
			if (Handles.Button(splineBend.markers[i].position, Quaternion.LookRotation(splineBend.markers[i].nextHandle - splineBend.markers[i].prewHandle, Vector3.up), splineBend.markerSize, splineBend.markerSize * 0.8f, func)) {
				Selection.activeTransform = (splineBend.markers[i].transform);
			}
			if (i != (splineBend.markers.Length) - 1) {
				Vector3[] subPoints = splineBend.markers[i].subPoints;
				Handles.DrawPolyLine(subPoints);
			}
		}
	}

	public static string ExportToObj(SplineBend splineBend){
		string path  = EditorUtility.SaveFilePanel("Save To Obj","Assets", splineBend.transform.name + ".obj", "obj");	
		if (path.Length == 0) return string.Empty;
		ExportToObj(path,splineBend);

		var localPath = path.Replace(Application.dataPath, "Assets");	
		AssetDatabase.ImportAsset(localPath, ImportAssetOptions.Default);		
		return localPath;
	}

	public static void ExportToObj(string path,SplineBend splineBend){
		var text = new System.Text.StringBuilder();
		int currentVertCount = 1;

		//exporting render mesh
		Mesh renderMesh = null;
		MeshFilter filter = splineBend.GetComponent<MeshFilter>();
		if (filter != null && filter.sharedMesh != null) 
			renderMesh = filter.sharedMesh;

		if (renderMesh != null) 
		{
			text.Append( ExportToString(renderMesh, currentVertCount, "_render",splineBend ));
			text.AppendLine();
			currentVertCount += renderMesh.vertices.Length;
		}

		//exporting collision mesh
		Mesh collisionMesh = null;
		MeshCollider collider   = splineBend.GetComponent<MeshCollider>();
		if (collider != null && collider.sharedMesh != null) collisionMesh = collider.sharedMesh;

		if (collisionMesh != null) 
		{
			text.Append( ExportToString(collisionMesh, currentVertCount, "_collision" ,splineBend));
			text.AppendLine();
			currentVertCount += collisionMesh.vertices.Length;
		}

		//writing exported data
		//var sw = new System.IO.StreamWriter(path);
		//sw.WriteLine(text);
		//sw.Close();
		System.IO.File.WriteAllText(path, text.ToString());
	}

	public static string ExportToString (Mesh mesh, int vCount,string name,SplineBend splineBend)
	{
		StringBuilder text = new System.Text.StringBuilder();
		foreach(Vector3 v in mesh.vertices) { 
			text.Append("v " + (-v.x) + " " + v.y + " " + v.z); 
			text.AppendLine(); 
		}
		foreach(Vector3 v in mesh.normals) { 
			text.Append("vn " + v.x + " " + v.y + " " + v.z); 
			text.AppendLine(); 
		}
		foreach(Vector3 v  in mesh.uv) { 
			text.Append("vt " + v.x + " " + v.y + " " + v.z); 
			text.AppendLine(); 
		}

		text.AppendLine();

		text.Append("g ").Append(splineBend.transform.name + name); text.AppendLine();

		text.Append("usemtl unnamed"); text.AppendLine();
		text.Append("usemap unnamed"); text.AppendLine();
		for (var i=0;i<mesh.triangles.Length;i+=3) 
		{
			text.Append(string.Format("f {2}/{2}/{2} {1}/{1}/{1} {0}/{0}/{0}\n", mesh.triangles[i]+vCount, mesh.triangles[i+1]+vCount, mesh.triangles[i+2]+vCount));
			text.AppendLine();
		}

		return text.ToString();
	}
}
