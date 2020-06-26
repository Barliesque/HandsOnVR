using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Barliesque.InspectorTools.Editor
{

	abstract public class EditorBase<T> : UnityEditor.Editor where T : UnityEngine.Object
	{

		Dictionary<string, SerializedProperty> _properties = new Dictionary<string, SerializedProperty>();

		override public sealed void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorTools.ScriptField(serializedObject);

			EditorGUILayout.Space();

			var inst = target as T;
			CustomInspector(inst);

			serializedObject.ApplyModifiedProperties();
			EditorTools.HelpBoxesEnabled = true;
		}

		private void OnEnable()
		{
			var inst = target as T;
			if (inst != null) OnEnabled(inst);
		}

		virtual protected void OnEnabled(T inst)
		{ }

		private void OnDisable()
		{
			OnDisabled(target as T);
		}

		virtual protected void OnDisabled(T inst)
		{ }

		abstract protected void CustomInspector(T inst);

		//TODO  Make IncludeChildren an optional parameter, so there aren't so many permutations
		//TODO  Duplicate all remaining PropertyField() methods as RequiredProperty()
		//TODO  Add optional method ShowWarning()
		//TODO  If warning's been set ShowWarning hasn't been called directly, then show it after calling CustomInspector()

		static protected SerializedProperty PropertyField(EditorBase<T> editor, string propName)
		{
			SerializedProperty prop = editor.GetProperty(propName);
			if (prop != null)
			{
				EditorGUILayout.PropertyField(prop);
			}
			return prop;
		}

		protected SerializedProperty PropertyField(string propName)
		{
			SerializedProperty prop = GetProperty(propName);
			if (prop != null)
			{
				EditorGUILayout.PropertyField(prop);
			}
			return prop;
		}

		static protected SerializedProperty PropertyField(EditorBase<T> editor, string propName, string label)
		{
			SerializedProperty prop = editor.GetProperty(propName);
			if (prop != null)
			{
				EditorGUILayout.PropertyField(prop, new GUIContent(label, prop.tooltip));
			}
			return prop;
		}

		protected SerializedProperty PropertyField(string propName, string label)
		{
			SerializedProperty prop = GetProperty(propName);
			if (prop != null)
			{
				EditorGUILayout.PropertyField(prop, new GUIContent(label, prop.tooltip));
			}
			return prop;
		}

		static protected SerializedProperty PropertyField(EditorBase<T> editor, string propName, string label, bool includeChildren)
		{
			SerializedProperty prop = editor.GetProperty(propName);
			if (prop != null)
			{
				EditorGUILayout.PropertyField(prop, new GUIContent(label, prop.tooltip), includeChildren);
			}
			return prop;
		}

		protected SerializedProperty PropertyField(string propName, string label, bool includeChildren)
		{
			SerializedProperty prop = GetProperty(propName);
			if (prop != null)
			{
				EditorGUILayout.PropertyField(prop, new GUIContent(label, prop.tooltip), includeChildren);
			}
			return prop;
		}

		protected SerializedProperty GetProperty(string propName)
		{
			SerializedProperty prop;
			if (!_properties.ContainsKey(propName))
			{
				prop = serializedObject.FindProperty(propName);
				if (prop == null)
				{
					Debug.LogErrorFormat("{0} editor could not find property: \"{1}\"", GetType().ToString(), propName);
				}
				else
				{
					_properties.Add(propName, prop);
				}
			}
			return _properties[propName];
		}


	}

}