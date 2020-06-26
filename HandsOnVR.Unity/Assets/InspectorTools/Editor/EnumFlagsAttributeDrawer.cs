using System;
using UnityEngine;
using UnityEditor;

namespace Barliesque.InspectorTools.Editor
{
	[CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
	public class EnumFlagsAttributeDrawer : PropertyDrawer
	{
		override public void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
		{
			// Make sure tooltip is still shown
			var tooltip = fieldInfo.GetCustomAttributes(typeof(TooltipAttribute), true);
			if (tooltip != null && tooltip.Length > 0) {
				label.tooltip = ((TooltipAttribute)tooltip[0]).tooltip;
			}

			// Change check is needed to prevent values being overwritten during multiple-selection
			EditorGUI.BeginChangeCheck();

			EditorGUI.BeginProperty(new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, rect.height), label, property);
			int newValue = EditorGUI.MaskField(rect, label, property.intValue, property.enumNames);
			EditorGUI.EndProperty();

			if (EditorGUI.EndChangeCheck()) {
				property.intValue = newValue;
			}
		}
	}
}
