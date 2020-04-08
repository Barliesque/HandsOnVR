using System;
using UnityEngine;
using UnityEditor;

namespace Barliesque.InspectorTools.Editor
{
	abstract public class PropertyDrawerHelper : PropertyDrawer
	{
		int _lines;
		int _gaps;
		protected Rect _position;
		SerializedProperty _property;

		public SerializedProperty Property
		{
			get { return _property; }
		}

		protected Rect _rect { get; private set; }

		virtual protected float LineHeight => 18f;
		virtual protected float LineSpacing => 2f;
		virtual protected float HorizSpacing => 8f;

		/// <summary>
		/// Optional left margin override.  If zero, the margin will not be customized.
		/// </summary>
		virtual protected float LeftMargin => 0f;

		float _originalMargin;
		protected float Margin => (LeftMargin > 0f ? LeftMargin : _originalMargin);
		string _propTooltip;

		override public sealed void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var attributes = fieldInfo.GetCustomAttributes(typeof(TooltipAttribute), true);
			if (attributes != null && attributes.Length > 0)
			{
				_propTooltip = ((TooltipAttribute) attributes[0]).tooltip;
			}
			else
			{
				_propTooltip = null;
			}

			label.tooltip = _propTooltip;

			// Using BeginProperty / EndProperty on the parent property means that
			// prefab override logic works on the entire property.
			EditorGUI.BeginProperty(position, label, property);
			_property = property;
			_rect = position;

			// Draw label
			_position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
			_originalMargin = _position.x;
			_position.x = Margin;

			_position.height = 18f;
			_lines = 1;
			_gaps = 0;

			// Don't make child fields be indented
			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			CustomDrawer();
			if (_position.x == Margin) --_lines;

			// Set indent back to what it was
			EditorGUI.indentLevel = indent;

			EditorGUI.EndProperty();
		}

		abstract public void CustomDrawer();

		virtual protected int LinesPerElement => 1;

		override public float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			int lines = Mathf.Max(_lines, LinesPerElement);
			return (LineHeight * lines) + (LineSpacing * (lines - LinesPerElement + 1)) + _gaps;
		}

		//***

		protected void NextLine(int extraLines = 0)
		{
			_position.x = Margin;
			_position.y += (LineHeight + LineSpacing) * (extraLines + 1);
			_lines += (extraLines + 1);
		}

		protected void Divider()
		{
			if (_position.x > Margin) NextLine();

			var rect = new Rect(Margin, _position.y + 3f, _rect.width - Margin, 1f);
			EditorGUI.DrawRect(rect, Color.grey * GUI.color);
			_position.y += 7f;
			_gaps += 7;
		}

		protected void Divider(float width)
		{
			if (_position.x > Margin) NextLine();

			var rect = new Rect(Margin, _position.y + 3f, width, 1f);
			EditorGUI.DrawRect(rect, Color.grey * GUI.color);
			_position.y += 7f;
			_gaps += 7;
		}

		protected void Space(float width)
		{
			_position.x += width;
		}

		//---

		protected SerializedProperty Field(float fieldWidth, string field)
		{
			var prop = _property.FindPropertyRelative(field);
			if (prop == null)
			{
				Debug.LogError($"Unknown property: {field}");
				return _property;
			}

			_position.width = fieldWidth < 0f ? (_rect.width - _position.x) : fieldWidth;
			_position.height = EditorGUI.GetPropertyHeight(prop);
			EditorGUI.PropertyField(_position, prop, GUIContent.none);
			_position.x += fieldWidth + HorizSpacing;
			return prop;
		}

		protected SerializedProperty Field(float labelWidth, float fieldWidth, string field, string tooltip = null)
		{
			var prop = _property.FindPropertyRelative(field);
			if (prop == null)
			{
				Debug.LogError($"Unknown property: {field}");
				return _property;
			}

			if (string.IsNullOrEmpty(tooltip)) tooltip = _propTooltip;

			_position.width = labelWidth;
			_position.height = EditorGUI.GetPropertyHeight(prop);
			EditorGUI.LabelField(_position, new GUIContent(field, tooltip));
			_position.x += labelWidth;
			_position.width = fieldWidth < 0f ? (_rect.width - _position.x) : fieldWidth;
			EditorGUI.PropertyField(_position, prop, GUIContent.none);
			_position.x += fieldWidth + HorizSpacing;
			return prop;
		}

		protected SerializedProperty Field(float labelWidth, string label, float fieldWidth, string field,
			string tooltip = null)
		{
			var prop = _property.FindPropertyRelative(field);
			if (prop == null)
			{
				Debug.LogError($"Unknown property: {field}");
				return _property;
			}

			if (string.IsNullOrEmpty(tooltip)) tooltip = _propTooltip;

			_position.width = labelWidth;
			_position.height = EditorGUI.GetPropertyHeight(prop);
			EditorGUI.LabelField(_position, new GUIContent(label, tooltip));
			_position.x += labelWidth;
			_position.width = fieldWidth < 0f ? (_rect.width - _position.x) : fieldWidth;
			EditorGUI.PropertyField(_position, prop, GUIContent.none);
			_position.x += fieldWidth + HorizSpacing;
			return prop;
		}

		protected SerializedProperty EnumField<T>(float labelWidth, string label, float fieldWidth, string field,
			string tooltip = null) where T : struct, IConvertible
		{
			var prop = _property.FindPropertyRelative(field);
			if (prop == null)
			{
				Debug.LogError($"Unknown property: {field}");
				return _property;
			}

			if (string.IsNullOrEmpty(tooltip)) tooltip = _propTooltip;

			_position.width = labelWidth;
			_position.height = EditorGUI.GetPropertyHeight(prop);
			EditorGUI.LabelField(_position, new GUIContent(label, tooltip));
			_position.x += labelWidth;
			_position.width = fieldWidth < 0f ? (_rect.width - _position.x) : fieldWidth;

			var options = Enum.GetNames(typeof(T));
			for (int i = 0; i < options.Length; i++) options[i] = options[i].SplitCamelCase();
			var values = (int[]) Enum.GetValues(typeof(T));
			var index = Array.IndexOf(values, prop.intValue);

			var selected = EditorGUI.Popup(_position, index, options);
			if (selected != index)
			{
				prop.intValue = values[selected];
			}

			_position.x += fieldWidth + HorizSpacing;
			return prop;
		}

		protected void Label(float labelWidth, string label, string tooltip = null, GUIStyle style = null)
		{
			if (string.IsNullOrEmpty(tooltip)) tooltip = _propTooltip;
			_position.width = labelWidth < 0f ? (_rect.width - _position.x) : labelWidth;
			_position.height = LineHeight;
			if (style == null)
			{
				EditorGUI.LabelField(_position, new GUIContent(label, tooltip));
			}
			else
			{
				EditorGUI.LabelField(_position, new GUIContent(label, tooltip), style);
			}

			_position.x += labelWidth + HorizSpacing;
		}

		protected SerializedProperty FieldAsLabel(float labelWidth, string field, string tooltip = null)
		{
			var prop = _property.FindPropertyRelative(field);
			if (prop == null)
			{
				Debug.LogError($"Unknown property: {field}");
				return _property;
			}

			if (string.IsNullOrEmpty(tooltip)) tooltip = _propTooltip;
			string label = prop.stringValue;
			_position.width = labelWidth < 0f ? (_rect.width - _position.x) : labelWidth;
			_position.height = LineHeight;
			EditorGUI.LabelField(_position, new GUIContent(label, tooltip));
			_position.x += labelWidth + HorizSpacing;
			return prop;
		}

		protected float FloatField(float fieldWidth, float value)
		{
			_position.width = fieldWidth < 0f ? (_rect.width - _position.x) : fieldWidth;
			var result = EditorGUI.FloatField(_position, value);
			_position.x += fieldWidth + HorizSpacing;
			return result;
		}

		//---

		protected bool Button(float width, string label, string tooltip = null)
		{
			_position.width = width < 0f ? (_rect.width - _position.x) : width;
			_position.height = LineHeight;
			GUIContent content = new GUIContent(label, tooltip);
			bool pressed = GUI.Button(_position, content);
			_position.x += width + HorizSpacing;
			return pressed;
		}

		protected SerializedProperty Slider(float width, string field, float min = 0.0f, float max = 1.0f)
		{
			var prop = _property.FindPropertyRelative(field);
			if (prop == null)
			{
				Debug.LogError($"Unknown property: {field}");
				return _property;
			}

			_position.width = width < 0f ? (_rect.width - _position.x) : width;
			_position.height = LineHeight;
			EditorGUI.Slider(_position, prop, min, max, GUIContent.none);
			_position.x += width + HorizSpacing;
			return prop;
		}

		protected void RangeSlider(float width, string minField, string maxField, float min = 0.0f, float max = 1.0f)
		{
			_position.width = width < 0f ? (_rect.width - _position.x) : width;
			_position.height = LineHeight;
			var minProp = _property.FindPropertyRelative(minField);
			if (minProp == null)
			{
				Debug.LogError($"Unknown property: {minField}");
				return;
			}

			var minVal = minProp.floatValue;
			var maxProp = _property.FindPropertyRelative(maxField);
			if (maxProp == null)
			{
				Debug.LogError($"Unknown property: {maxField}");
				return;
			}

			var maxVal = maxProp.floatValue;
			EditorGUI.MinMaxSlider(_position, ref minVal, ref maxVal, min, max);
			minProp.floatValue = minVal;
			maxProp.floatValue = maxVal;
			_position.x += width + HorizSpacing;
		}

		//---

		protected void TextBox(string field, int lines = 2, float fieldWidth = -1f)
		{
			float oldHeight = _position.height;
			_position.height = lines * EditorTools.TextArea.lineHeight + EditorTools.TextArea.padding.top +
			                   EditorTools.TextArea.padding.bottom;
			_position.width = fieldWidth < 0f ? (_rect.width - _position.x) : fieldWidth;
			var prop = _property.FindPropertyRelative(field);
			if (prop == null)
			{
				Debug.LogError($"Unknown property: {field}");
				return;
			}

			prop.stringValue = EditorGUI.TextArea(_position, prop.stringValue, EditorTools.TextArea);

			_position.height = oldHeight;
			if (fieldWidth < 0f)
			{
				NextLine(lines - 1);
			}
			else
			{
				_position.x += fieldWidth + HorizSpacing;
			}
		}

		protected void SpriteBox(string field, int lines = 2, float fieldWidth = -1f)
		{
			float oldHeight = _position.height;
			_position.height = lines * LineHeight;
			_position.width = fieldWidth < 0f ? (_rect.width - _position.x) : fieldWidth;
			var prop = _property.FindPropertyRelative(field);
			if (prop == null)
			{
				Debug.LogError($"Unknown property: {field}");
				return;
			}

			EditorGUI.ObjectField(_position, prop, typeof(Sprite), GUIContent.none);

			_position.height = oldHeight;
			if (fieldWidth < 0f)
			{
				NextLine(lines - 1);
			}
			else
			{
				_position.x += fieldWidth + HorizSpacing;
			}
		}

		protected void ObjectBox<T>(string field, int lines = 2, float fieldWidth = -1f)
		{
			float oldHeight = _position.height;
			_position.height = lines * LineHeight;
			_position.width = fieldWidth < 0f ? (_rect.width - _position.x) : fieldWidth;
			var prop = _property.FindPropertyRelative(field);
			if (prop == null)
			{
				Debug.LogError($"Unknown property: {field}");
				return;
			}

			EditorGUI.ObjectField(_position, prop, typeof(T), GUIContent.none);

			_position.height = oldHeight;
			if (fieldWidth < 0f)
			{
				NextLine(lines - 1);
			}
			else
			{
				_position.x += fieldWidth + HorizSpacing;
			}
		}

		//---

		protected bool GetBool(string field)
		{
			var prop = _property.FindPropertyRelative(field);
			if (prop == null)
			{
				Debug.LogError($"Unknown property: {field}");
				return false;
			}

			return prop.boolValue;
		}

		protected int GetInt(string field)
		{
			var prop = _property.FindPropertyRelative(field);
			if (prop == null)
			{
				Debug.LogError($"Unknown property: {field}");
				return 0;
			}

			return prop.intValue;
		}

		protected float GetFloat(string field)
		{
			var prop = _property.FindPropertyRelative(field);
			if (prop == null)
			{
				Debug.LogError($"Unknown property: {field}");
				return 0f;
			}

			return prop.floatValue;
		}

		protected Half GetHalf(string field)
		{
			var prop = _property.FindPropertyRelative($"{field}._half");
			if (prop == null)
			{
				Debug.LogError($"Unknown property: {field}");
				return 0;
			}

			return Mathf.HalfToFloat((ushort) prop.intValue);
		}

		protected T GetObject<T>(string field) where T : UnityEngine.Object
		{
			var prop = _property.FindPropertyRelative(field);
			if (prop == null)
			{
				Debug.LogError($"Unknown property: {field}");
				return null;
			}

			return prop.objectReferenceValue as T;
		}
	}
}