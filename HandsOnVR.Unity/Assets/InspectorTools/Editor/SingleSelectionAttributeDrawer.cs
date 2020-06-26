using System;
using UnityEngine;
using UnityEditor;

namespace Barliesque.InspectorTools.Editor
{
	[CustomPropertyDrawer(typeof(SingleSelectionAttribute))]
	public class SingleSelectionAttributeDrawer : PropertyDrawer
	{
		override public void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
		{
			// Make sure tooltip is still shown
			var tooltip = fieldInfo.GetCustomAttributes(typeof(TooltipAttribute), true);
			if (tooltip.Length > 0)
			{
				label.tooltip = ((TooltipAttribute) tooltip[0]).tooltip;
			}

			var singleSelection = attribute as SingleSelectionAttribute;
			if (singleSelection == null) return;
			var allowNone = singleSelection.AllowNone;
			var addNone = (allowNone ? 1 : 0);

			EditorGUI.BeginChangeCheck();
			EditorGUI.BeginProperty(new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, rect.height), label, property);

			var options = new GUIContent[property.enumNames.Length + addNone];
			for (int i = 0; i < options.Length; i++)
			{
				options[i] = new GUIContent((i == 0 && allowNone) ? "None" : property.enumNames[i - addNone]);
			}

			var newIndex = EditorGUI.Popup(rect, label, property.enumValueIndex + addNone, options);
			EditorGUI.EndProperty();

			if (EditorGUI.EndChangeCheck())
			{
				if (allowNone)
				{
					if (newIndex == 0)
					{
						property.intValue = 0;
					}
					else
					{
						property.enumValueIndex = newIndex - 1;
					}
				}
				else
				{
					property.enumValueIndex = newIndex;
				}
			}
		}

		
	}
}