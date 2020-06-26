using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Barliesque.InspectorTools.Editor
{
	abstract public class EditorBase : UnityEditor.Editor
	{

		Dictionary<string, SerializedProperty> _properties = new Dictionary<string, SerializedProperty>();

		virtual protected bool ShowScriptField => true;

		override public sealed void OnInspectorGUI()
		{
			serializedObject.Update();

			if (ShowScriptField)
			{
				EditorTools.ScriptField(serializedObject);
			}

			EditorGUILayout.Space();
			CustomInspector();

			serializedObject.ApplyModifiedProperties();
			EditorTools.HelpBoxesEnabled = true;
		}

		abstract protected void CustomInspector();

		static protected SerializedProperty PropertyField(EditorBase editor, string propName) {
			SerializedProperty prop = editor.GetProperty(propName);
			if (prop != null) {
				EditorGUILayout.PropertyField(prop);
			}
			return prop;
		}

		protected SerializedProperty PropertyField(string propName)
		{
			SerializedProperty prop = GetProperty(propName);
			if (prop != null) {
				EditorGUILayout.PropertyField(prop);
			}
			return prop;
		}

		static protected SerializedProperty PropertyField(EditorBase editor, string propName, string label)
		{
			SerializedProperty prop = editor.GetProperty(propName);
			if (prop != null) {
				EditorGUILayout.PropertyField(prop, new GUIContent(label, prop.tooltip));
			}
			return prop;
		}

		protected SerializedProperty PropertyField(string propName, string label)
		{
			SerializedProperty prop = GetProperty(propName);
			if (prop != null) {
				EditorGUILayout.PropertyField(prop, new GUIContent(label, prop.tooltip));
			}
			return prop;
		}

		static protected SerializedProperty PropertyField(EditorBase editor, string propName, string label, bool includeChildren)
		{
			SerializedProperty prop = editor.GetProperty(propName);
			if (prop != null) {
				EditorGUILayout.PropertyField(prop, new GUIContent(label, prop.tooltip), includeChildren);
			}
			return prop;
		}

		protected SerializedProperty PropertyField(string propName, string label, bool includeChildren)
		{
			SerializedProperty prop = GetProperty(propName);
			if (prop != null) {
				EditorGUILayout.PropertyField(prop, new GUIContent(label, prop.tooltip), includeChildren);
			}
			return prop;
		}

		protected SerializedProperty GetProperty(string propName)
		{
			SerializedProperty prop;
			if (!_properties.ContainsKey(propName)) {
				prop = serializedObject.FindProperty(propName);
				if (prop == null) {
					Debug.LogErrorFormat("{0} editor could not find property: \"{1}\"", GetType().ToString(), propName);
				} else {
					_properties.Add(propName, prop);
				}
			}
			return _properties[propName];
		}


	}
}