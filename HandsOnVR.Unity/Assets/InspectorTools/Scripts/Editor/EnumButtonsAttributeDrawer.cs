using System;
using UnityEngine;
using UnityEditor;

namespace Barliesque.InspectorTools.Editor
{
	[CustomPropertyDrawer(typeof(EnumButtonsAttribute))]
	public class EnumButtonsAttributeDrawer : PropertyDrawer
	{
		const float ButtonHeight = 19f;
		const float VerticalSpace = 1f;

		override public void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
		{
			// Make sure tooltip is still shown
			var tooltip = fieldInfo.GetCustomAttributes(typeof(TooltipAttribute), true);
			if (tooltip != null && tooltip.Length > 0) {
				label.tooltip = ((TooltipAttribute)tooltip[0]).tooltip;
			}

			// Check for buttons type of rendering
			var attribute = ((EnumButtonsAttribute)fieldInfo.GetCustomAttributes(typeof(EnumButtonsAttribute), true)[0]);
			var allowMultiple = attribute.AllowMultiple;
			int maxPerLine = attribute.MaxPerLine;

			// Change check is needed to prevent values being overwritten during multiple-selection
			EditorGUI.BeginChangeCheck();

			int newValue = 0;

			//
			// Make a button for each enum entry
			//
			string[] buttonNames = property.enumDisplayNames;
			string[] enumNames = property.enumNames;
			int count = enumNames.Length;
			int lines = Mathf.CeilToInt((float)count / maxPerLine);
			EditorGUI.BeginProperty(new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, (ButtonHeight + VerticalSpace) * lines), label, property);
			EditorGUI.LabelField(new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, ButtonHeight), label);
			float buttonWidth = (rect.width - EditorGUIUtility.labelWidth) / Mathf.Min(count, maxPerLine);
			int oldValue = property.intValue;

			for (int i = 0; i < count; i++) {
				// Get the value of each enum option
				int enumValue = (int)Enum.Parse(fieldInfo.FieldType, enumNames[i]);
				bool pressed = (oldValue & enumValue) == enumValue;

				Rect buttonPos = new Rect(rect.x + EditorGUIUtility.labelWidth + buttonWidth * (i % maxPerLine), rect.y + ((ButtonHeight + VerticalSpace) * (i / maxPerLine)), buttonWidth, ButtonHeight);

				if (GUI.Toggle(buttonPos, pressed, buttonNames[i], "Button")) {
					if (allowMultiple) {
						newValue |= enumValue;
					} else {
						oldValue = newValue = enumValue;
					}
				}
			}
			EditorGUI.EndProperty();

			if (EditorGUI.EndChangeCheck()) {
				property.intValue = newValue;
			}
		}


		override public float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			// Check for buttons type of rendering
			var attribute = ((EnumButtonsAttribute)fieldInfo.GetCustomAttributes(typeof(EnumButtonsAttribute), true)[0]);
			int maxPerLine = attribute.MaxPerLine;
			int count = property.enumNames.Length;
			int lines = Mathf.CeilToInt((float)count / maxPerLine);
			return (ButtonHeight + VerticalSpace) * lines;
		}
	}
}
