using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace HandsOnVR.Editor
{
	[CustomPropertyDrawer(typeof(LinearLimits))]
	public class LinearLimitsDrawer : PropertyDrawer
	{
		override public VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			// Create property container element.
			var container = new VisualElement();

			// Create property fields.
			var lowField = new PropertyField(property.FindPropertyRelative("Low"));
			var highField = new PropertyField(property.FindPropertyRelative("High"));

			// Add fields to the container.
			container.Add(lowField);
			container.Add(highField);

			return container;
		}

		override public void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			// Using BeginProperty / EndProperty on the parent property means that
			// prefab override logic works on the entire property.
			EditorGUI.BeginProperty(position, label, property);

			// Draw label
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			// Don't make child fields be indented
			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			// Calculate rects
			var width = position.width * 0.5f - 26f;
			var lowRect = new Rect(position.x, position.y, width, position.height);
			var toRect = new Rect(position.x + width + 6f, position.y, 14f, position.height);
			var highRect = new Rect(position.x + width + 26, position.y, width, position.height);

			// Draw fields - pass GUIContent.none to each so they are drawn without labels
			EditorGUI.PropertyField(lowRect, property.FindPropertyRelative("Low"), GUIContent.none);
			EditorGUI.LabelField(toRect, "to");
			EditorGUI.PropertyField(highRect, property.FindPropertyRelative("High"), GUIContent.none);

			// Set indent back to what it was
			EditorGUI.indentLevel = indent;

			EditorGUI.EndProperty();
		}
	}
}