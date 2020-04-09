using UnityEngine;
using UnityEditor;

namespace Barliesque.InspectorTools.Editor
{
	[CustomPropertyDrawer(typeof(HelpBoxAttribute))]
	public class HelpBoxAttributeDrawer : DecoratorDrawer
	{
		static private GUIStyle _style;

		static GUIStyle HelpBoxStyle
		{
			get
			{
				if (_style != null) return _style;
				_style = EditorStyles.helpBox;
				_style.richText = true;
				return _style;
			}
		}

		override public float GetHeight()
		{
			if (!HelpBoxAttribute.enabled) return 0f;
			if (GUI.skin == null) return base.GetHeight();

			var helpBoxAttribute = attribute as HelpBoxAttribute;
			if (helpBoxAttribute == null) return base.GetHeight();

			var height = HelpBoxStyle.CalcHeight(new GUIContent(string.Format("{0}----", helpBoxAttribute.text)), EditorGUIUtility.currentViewWidth) +
			             helpBoxAttribute.spaceAbove + helpBoxAttribute.spaceBelow;
			return (helpBoxAttribute.messageType == HelpBoxType.None) ? height : Mathf.Max(height, 55f);
		}

		override public void OnGUI(Rect position)
		{
			if (!HelpBoxAttribute.enabled) return;
			var helpBoxAttribute = attribute as HelpBoxAttribute;
			if (helpBoxAttribute == null) return;

			position.y += helpBoxAttribute.spaceAbove;
			position.height -= (helpBoxAttribute.spaceAbove + helpBoxAttribute.spaceBelow);

			EditorGUI.HelpBox(position, helpBoxAttribute.text, GetHelpBoxType(helpBoxAttribute.messageType));
		}

		private MessageType GetHelpBoxType(HelpBoxType type)
		{
			switch (type)
			{
				case HelpBoxType.Info: return MessageType.Info;
				case HelpBoxType.Warning: return MessageType.Warning;
				case HelpBoxType.Error: return MessageType.Error;
				default: return MessageType.None;
			}
		}
	}
}