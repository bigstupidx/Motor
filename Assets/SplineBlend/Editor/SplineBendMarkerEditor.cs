using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SplineBendMarker))]
public class SplineBendMarkerEditor : Editor {
	[NonSerialized]
	public static Vector3[] triVerts = new Vector3[] {
		new Vector3((float) 1, (float) 0, (float) 0),
		new Vector3((float) 0, (float) 1, (float) 0),
		new Vector3((float) 0, (float) 0, (float) 1)
	};

	[NonSerialized]
	public static int[] triFaces = new int[] {
		0,
		1,
		2
	};

	public bool changed;
	public bool wasChange;
	public bool collisionUpdated;
	public bool mousePressed;
	public bool mouseWasPressed;


	public override void OnInspectorGUI() {
		//EditorGUIUtility.LookLikeInspector ();
		DrawDefaultInspector();

		var target = (SplineBendMarker)this.target;
		//remaking markers and updating if any marker is missing
		if (!target.splineScript) return;
		for (int m = 0; m < target.splineScript.markers.Length; m++) if (!target.splineScript.markers[m]) { changed = true; break; }

		if (GUI.changed) target.splineScript.ForceUpdate();
	}

	void OnSceneGUI() {
		var target = (SplineBendMarker)this.target;
		Undo.RecordObject(target, "SplineBend");
		SplineBendEditor.DrawMarkers(target.splineScript);
		//display marker itself
		Handles.color = new Color(1, 0, 0, 1);
		Quaternion diaplayRotation = Quaternion.LookRotation(target.nextHandle - target.prewHandle, Vector3.up);
		switch (target.type) {
			case SplineBendMarker.MarkerType.Smooth:
				Handles.SphereCap(0, target.position, diaplayRotation, target.splineScript.markerSize * 1.2f);
				break;
			case SplineBendMarker.MarkerType.Transform:
				Handles.ConeCap(0, target.position, diaplayRotation, target.splineScript.markerSize * 1.2f);
				break;
			default:
				Handles.CubeCap(0, target.position, diaplayRotation, target.splineScript.markerSize * 1.2f);
				break;
		}

		//drawing tangents
		//Handles.color = Color(0.25f,0,0,1);
		Handles.DrawLine(target.position, target.position + target.prewHandle);
		Handles.DrawLine(target.position, target.position + target.nextHandle);


		//displaing handles
		if (target.type == SplineBendMarker.MarkerType.Beizer || target.type == SplineBendMarker.MarkerType.BeizerCorner) {
			Vector3 prewHandle = Handles.PositionHandle(target.prewHandle + target.position, Quaternion.identity) -
								 target.position;
			Vector3 nextHandle = Handles.PositionHandle(target.nextHandle + target.position, Quaternion.identity) -
								 target.position;

			//finding which of handles changed more
			float prewHandleDelta = (prewHandle - target.prewHandle).sqrMagnitude;
			float nextHandleDelta = (nextHandle - target.nextHandle).sqrMagnitude;

			if (prewHandleDelta > 0.000001f || nextHandleDelta > 0.000001f) {
				if (prewHandleDelta > nextHandleDelta) {
					target.prewHandle = prewHandle;
					if (target.type == SplineBendMarker.MarkerType.Beizer)
						target.nextHandle = -prewHandle.normalized * target.nextHandle.magnitude;
				} else //(prewHandleDelta < nextHandleDelta)
				  {
					target.nextHandle = nextHandle;
					if (target.type == SplineBendMarker.MarkerType.Beizer)
						target.prewHandle = -nextHandle.normalized * target.prewHandle.magnitude;
				}

				changed = true;
			}
		}

		if (Event.current.type == EventType.MouseDown) mousePressed = true;
		if (Event.current.type == EventType.MouseUp) mousePressed = false;


		if (changed
			|| target.transform.position != target.oldPos
			|| target.transform.localScale != target.oldScale
			|| target.transform.rotation != target.oldRot) {
			//updating without collision
			if (Event.current.type == EventType.Repaint) {
				target.splineScript.ForceUpdate(false);

				target.oldPos = target.transform.position;
				target.oldScale = target.transform.localScale;
				target.oldRot = target.transform.rotation;

				changed = false;
				wasChange = true;
			}
		} else if (wasChange && Event.current.type == EventType.MouseUp) {
			target.splineScript.ForceUpdate(true);
			wasChange = false;
		}



	}

}
