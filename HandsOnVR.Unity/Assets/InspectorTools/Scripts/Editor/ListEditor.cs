using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

// More info:
// https://va.lent.in/unity-make-your-lists-functional-with-reorderablelist/

namespace Barliesque.InspectorTools.Editor
{
	public class ListEditor
	{
		public ReorderableList List { get; }

		public string Header
		{
			get => _showHeader ? _header : null;
			set => _header = value;
		}

		private string _header;
		private readonly bool _showHeader;
		private int _linesPerElement;

		/// <summary>
		/// Instantiate this from the OnEnable part of your custom Editor.  Then call DoLayoutList() from the OnInspectorGUI method to draw the inspector.
		/// </summary>
		/// <param name="serializedObject">The serialized object containing the property.</param>
		/// <param name="field">The path to the property.</param>
		/// <param name="header">By default, the property name is used as the header.  Assign a custom header here, or an empty string for no header.</param>
		/// <param name="linesPerElement">How many lines per element?</param>
		public ListEditor(SerializedObject serializedObject, string field, string header = null, int linesPerElement = 1) : this(
			serializedObject.FindProperty(field), header, linesPerElement)
		{ }

		/// <summary>
		/// Instantiate this from the OnEnable part of your custom Editor.  Then call DoLayoutList() from the OnInspectorGUI method to draw the inspector.
		/// </summary>
		/// <param name="property">The serialized property containing the List.</param>
		/// <param name="header">By default, the property name is used as the header.  Assign a custom header here, or an empty string for no header.</param>
		/// <param name="linesPerElement">How many lines per element?</param>
		public ListEditor(SerializedProperty property, string header = null, int linesPerElement = 1)
		{
			List = new ReorderableList(property.serializedObject, property, true, !string.IsNullOrEmpty(header), true, true)
			{
				elementHeight = (EditorGUIUtility.singleLineHeight + 4f) * linesPerElement + 5f,
				drawElementCallback = DrawElement,
				drawHeaderCallback = DrawHeader,
				onChangedCallback = ApplyChanges,
				onReorderCallback = ApplyChanges
			};
			_showHeader = !string.IsNullOrEmpty(header);
			_header = header ?? List.serializedProperty.displayName;
			_linesPerElement = linesPerElement;
		}

		private void DrawHeader(Rect rect)
		{
			EditorGUI.LabelField(rect, _header);
			List.displayAdd = List.displayRemove = GUI.enabled;
		}

		private void DrawElement(Rect rect, int i, bool isActive, bool isFocused)
		{
			var element = List.serializedProperty.GetArrayElementAtIndex(i);
			
			if (_linesPerElement > 1)
			{
				// Put a box around each element
				EditorGUI.HelpBox(rect, "", MessageType.None);
			}

			rect.y += 5;
			EditorGUI.PropertyField(rect, element, GUIContent.none, true);
		}

		private void ApplyChanges(ReorderableList list)
		{
			List.serializedProperty.serializedObject.ApplyModifiedProperties();
		}

		public void DoLayoutList() => List.DoLayoutList();
	}
}