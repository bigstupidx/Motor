using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Game {

//	[CustomPropertyDrawer(typeof(PrefabWithSpawnPos))]
	public class PrefabWithSpwanPosEditor : PropertyDrawer {

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			return base.GetPropertyHeight(property, label) + EditorGUIUtility.singleLineHeight * 4;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

			//			base .OnGUI(position, property, label);
//			EditorGUI.BeginProperty(position, label, property);
			position.height -= EditorGUIUtility.singleLineHeight * 2;
//			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
			EditorGUI.PropertyField(position, property, GUIContent.none, true);

			var buttonPos = new Rect(position);
			buttonPos.y += buttonPos.height+ EditorGUIUtility.singleLineHeight * 1f;
			buttonPos.height = EditorGUIUtility.singleLineHeight * 1;
			GUI.Button(buttonPos, "123");
//			EditorGUI.EndProperty();


		}

	}
}
