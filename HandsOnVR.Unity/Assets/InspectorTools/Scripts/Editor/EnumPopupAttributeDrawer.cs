using System;
using UnityEngine;
using UnityEditor;

namespace Barliesque.InspectorTools.Editor
{
	[CustomPropertyDrawer(typeof(EnumPopupAttribute))]
	public class EnumPopupAttributeDrawer : PropertyDrawer
	{
		override public void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
		{
			// Make sure tooltip is still shown
			var tooltip = fieldInfo.GetCustomAttributes(typeof(TooltipAttribute), true);
			if (tooltip != null && tooltip.Length > 0)
			{
				label.tooltip = ((TooltipAttribute)tooltip[0]).tooltip;
			}

			// Change check is needed to prevent values being overwritten during multiple-selection
			EditorGUI.BeginChangeCheck();

			EditorGUI.BeginProperty(new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, rect.height), label, property);
			var options = new GUIContent[property.enumNames.Length];
			for (int i = 0; i < options.Length; i++) options[i] = new GUIContent(property.enumNames[
				
				
			i]);
			int newIndex = EditorGUI.Popup(rect, label, property.enumValueIndex, options);
			EditorGUI.EndProperty();

			if (EditorGUI.EndChangeCheck())
			{
				property.enumValueIndex = newIndex;
			}
		}
	}
}
