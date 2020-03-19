using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace HandsOnVR.Editor
{
	[CustomPropertyDrawer(typeof(AngularLimits))]
	public class AngularLimitsDrawer : PropertyDrawer
	{
		override public VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			// Create property container element.
			var container = new VisualElement();

			// Create property fields.
			var startField = new PropertyField(property.FindPropertyRelative("_start"));
			var endField = new PropertyField(property.FindPropertyRelative("_end"));
			var ccwField = new PropertyField(property.FindPropertyRelative("CCW"));

			// Add fields to the container.
			container.Add(startField);
			container.Add(endField);
			container.Add(ccwField);

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
			var width = position.width * 0.5f - 52f;
			var lowRect = new Rect(position.x, position.y, width, position.height);
			var toRect = new Rect(position.x + width + 6f, position.y, 14f, position.height);
			var highRect = new Rect(position.x + width + 26, position.y, width, position.height);
			var ccwLabelRect = new Rect(highRect.xMax + 4f, position.y, 32f, position.height);
			var ccwRect = new Rect(ccwLabelRect.xMax, position.y, 20f, position.height);

			var startProp = property.FindPropertyRelative("_start");
			var endProp = property.FindPropertyRelative("_end");
			var ccwProp = property.FindPropertyRelative("CCW");
			
			// Draw fields - pass GUIContent.none to each so they are drawn without labels
			EditorGUI.PropertyField(lowRect, startProp, GUIContent.none);
			EditorGUI.LabelField(toRect, "to");
			EditorGUI.PropertyField(highRect, endProp, GUIContent.none);
			
			var ccwLabel = new GUIContent("CCW", "Is the angular range specified in counter-clockwise order?  Angular values increase travelling counter-clockwise.  This option can be used to invert the angular range.");
			EditorGUI.LabelField(ccwLabelRect, ccwLabel);
			EditorGUI.PropertyField(ccwRect, ccwProp, GUIContent.none);

			// Set indent back to what it was
			EditorGUI.indentLevel = indent;

			EditorGUI.EndProperty();
		}
	}
}