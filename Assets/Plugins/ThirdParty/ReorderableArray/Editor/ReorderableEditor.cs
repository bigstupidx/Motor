using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditorInternal;
using UnityEngine;

using Object = UnityEngine.Object;

namespace UnityEditor {
	[CanEditMultipleObjects, CustomEditor(typeof(Object), true, isFallback = true)]
	public class ReorderableArrayEditor : Editor {
		private class ReorderableListWrapper {
			private const float border = 6f;
			public ReorderableList list {
				get;
				private set;
			}
			public SerializedProperty property {
				get {
					return this.list.serializedProperty;
				}
				set {
					this.list.serializedProperty = value;
				}
			}
			public bool isExpanded {
				get {
					return this.property.isExpanded;
				}
				set {
					this.property.isExpanded = value;
				}
			}
			public ReorderableListWrapper(SerializedProperty serializedProperty) {
				this.list = new ReorderableList(serializedProperty.serializedObject, serializedProperty, true, true, true, true);
				this.list.showDefaultBackground = true;

				this.list.drawHeaderCallback += DrawHeader;
				this.list.onCanRemoveCallback += reorderableList => reorderableList.count>0;
				this.list.drawElementCallback += DrawElement;
				this.list.elementHeightCallback+=ElementHeight;
			}
			private float ElementHeight(int index) {
				SerializedProperty arrayElementAtIndex = this.property.GetArrayElementAtIndex(index);
				float propertyHeight = EditorGUI.GetPropertyHeight(arrayElementAtIndex, GUIContent.none, true);
				return Mathf.Max(EditorGUIUtility.singleLineHeight, propertyHeight) + border;
			}
			private void DrawHeader(Rect rect) {
				rect.x = (rect.x - border);
				this.isExpanded = EditorGUI.Foldout(rect, this.isExpanded, string.Format("{0} [{1}]", this.property.displayName, this.property.arraySize));
				Rect rect2 = rect;
				if (this.list.count == 0) {
					rect2.height = (rect2.height * 3f);
				}
				Event current = Event.current;
				EventType type = current.type;
				if (type != EventType.DragExited) {
					if (type == EventType.DragUpdated || type == EventType.DragPerform) {
						if (rect2.Contains(current.mousePosition)) {
							DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
							if (current.type == EventType.DragPerform) {
								DragAndDrop.AcceptDrag();
								Object[] objectReferences = DragAndDrop.objectReferences;
								for (int i = 0; i < objectReferences.Length; i++) {
									Object obj = objectReferences[i];
									this.AddElement(obj);
								}
							}
						}
					}
				} else {
					DragAndDrop.PrepareStartDrag();
				}
			}
			private void DrawElement(Rect rect, int index, bool active, bool focused) {
				SerializedProperty arrayElementAtIndex = this.property.GetArrayElementAtIndex(index);
				rect.height = (EditorGUI.GetPropertyHeight(arrayElementAtIndex, GUIContent.none, true));
				rect.y = (rect.y + 3f);
				if (arrayElementAtIndex.hasVisibleChildren && !this.IsVectorElement(arrayElementAtIndex)) {
					this.DrawChildren(rect, index, arrayElementAtIndex, active, focused);
				} else {
					EditorGUI.PropertyField(rect, this.property.GetArrayElementAtIndex(index), GUIContent.none, true);
				}
				this.list.elementHeight = rect.height + 6f;
			}
			private void DrawChildren(Rect rect, int index, SerializedProperty parent, bool active, bool focused) {
				rect.x = (rect.x + 15f);
				rect.width = (rect.width - 15f);
				if (parent.isExpanded) {
					EditorGUI.PropertyField(rect, parent, this.ParentValueStringAndTooltip(parent, index), true);
				} else {
					parent.isExpanded = (EditorGUI.Foldout(rect, parent.isExpanded, this.ParentValueStringAndTooltip(parent, index)));
				}
			}
			private bool IsVectorElement(SerializedProperty element) {
				return element.propertyType == SerializedPropertyType.Quaternion || element.propertyType == SerializedPropertyType.Vector2 || element.propertyType == SerializedPropertyType.Vector3 || element.propertyType == SerializedPropertyType.Vector4;
			}
			private GUIContent ParentValueStringAndTooltip(SerializedProperty parent, int index) {
				SerializedProperty serializedProperty = parent.Copy();
				if (serializedProperty.NextVisible(true)) {
					if (serializedProperty.propertyType == SerializedPropertyType.String && serializedProperty.stringValue != null) {
						return new GUIContent(string.Empty + serializedProperty.stringValue, serializedProperty.tooltip);
					}
					if (serializedProperty.propertyType == SerializedPropertyType.ObjectReference && serializedProperty.objectReferenceValue != null) {
						return new GUIContent(string.Empty + serializedProperty.objectReferenceValue, serializedProperty.tooltip);
					}
				}
				return new GUIContent("Element " + index);
			}
			private void AddElement(Object obj) {
				this.property.InsertArrayElementAtIndex(this.property.arraySize);
				SerializedProperty arrayElementAtIndex = this.property.GetArrayElementAtIndex(this.property.arraySize - 1);
				arrayElementAtIndex.objectReferenceValue = obj;
			}
		}

		private Dictionary<string, ReorderableArrayEditor.ReorderableListWrapper> reorderableLists;
		protected virtual void OnEnable() {
			this.reorderableLists = new Dictionary<string, ReorderableArrayEditor.ReorderableListWrapper>();
		}

		public override void OnInspectorGUI() {
			this.DrawDefaultInspector();
		}

		public new bool DrawDefaultInspector() {
			Color color = GUI.color;
			base.serializedObject.Update();
			SerializedProperty iterator = base.serializedObject.GetIterator();
			if (iterator.NextVisible(true)) {
				do {
					GUI.color = color;
					this.DrawProperty(iterator);
				}
				while (iterator.NextVisible(false));
			}
			return base.serializedObject.ApplyModifiedProperties();
		}

		public void DrawProperty(SerializedProperty property) {
			if (property.isArray && property.propertyType != SerializedPropertyType.String && ReorderableArrayEditor.HasReorderableAttribute(property)) {
				this.DrawReorderableArray(property);
			} else {
				EditorGUILayout.PropertyField(property, property.isExpanded, new GUILayoutOption[0]);
			}
		}

		public void DrawReorderableArray(SerializedProperty property) {
			HeaderAttribute propertyAttribute = ReorderableArrayEditor.GetPropertyAttribute<HeaderAttribute>(property);
			if (propertyAttribute != null) {
				EditorGUILayout.Space();
				EditorGUILayout.LabelField(propertyAttribute.header, EditorStyles.boldLabel, new GUILayoutOption[0]);
			}
			ReorderableArrayEditor.ReorderableListWrapper reorderableListWrapper = this.GetReorderableListWrapper(property);
			if (reorderableListWrapper.isExpanded) {
				reorderableListWrapper.list.DoLayoutList();
			} else {
				property.isExpanded = (EditorGUILayout.Foldout(property.isExpanded, new GUIContent(string.Format("{0} [{1}]", property.displayName, property.arraySize), property.tooltip)));
			}
		}
		public static bool HasReorderableAttribute(SerializedProperty property) {
			PropertyAttribute[] propertyAttributes = ReorderableArrayEditor.GetPropertyAttributes<PropertyAttribute>(property);
			if (propertyAttributes != null) {
				PropertyAttribute[] array = propertyAttributes;
				for (int i = 0; i < array.Length; i++) {
					PropertyAttribute propertyAttribute = array[i];
					if (propertyAttribute.GetType().Name == "ReorderableAttribute") {
						return true;
					}
				}
			}
			return false;
		}
		private static T GetPropertyAttribute<T>(SerializedProperty property) where T : Attribute {
			T[] propertyAttributes = ReorderableArrayEditor.GetPropertyAttributes<T>(property);
			if (propertyAttributes != null && propertyAttributes.Length > 0) {
				return propertyAttributes[0];
			}
			return (T)((object)null);
		}
		private static T[] GetPropertyAttributes<T>(SerializedProperty property) where T : Attribute {
			BindingFlags bindingAttr = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.GetProperty;
			if (property.serializedObject.targetObject == null) {
				return null;
			}
			Type type = property.serializedObject.targetObject.GetType();
			FieldInfo field = type.GetField(property.name, bindingAttr);
			if (field != null) {
				return (T[])field.GetCustomAttributes(typeof(T), true);
			}
			return null;
		}
		private ReorderableArrayEditor.ReorderableListWrapper GetReorderableListWrapper(SerializedProperty property) {
			ReorderableArrayEditor.ReorderableListWrapper reorderableListWrapper;
			if (this.reorderableLists.TryGetValue(property.name, out reorderableListWrapper)) {
				reorderableListWrapper.property = property;
				return reorderableListWrapper;
			}
			reorderableListWrapper = new ReorderableArrayEditor.ReorderableListWrapper(property);
			this.reorderableLists.Add(property.name, reorderableListWrapper);
			return reorderableListWrapper;
		}
	}
}
