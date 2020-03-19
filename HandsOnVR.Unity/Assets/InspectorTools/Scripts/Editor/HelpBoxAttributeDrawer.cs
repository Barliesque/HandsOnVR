using UnityEngine;
using UnityEditor;

namespace Barliesque.InspectorTools.Editor
{
	[CustomPropertyDrawer(typeof(HelpBoxAttribute))]
	public class HelpBoxAttributeDrawer : DecoratorDrawer
	{
		override public float GetHeight()
		{
			if (!HelpBoxAttribute.enabled) return 0f;
			var helpBoxAttribute = attribute as HelpBoxAttribute;
			if (helpBoxAttribute == null) return base.GetHeight();
			var helpBoxStyle = (GUI.skin != null) ? GUI.skin.GetStyle("helpbox") : null;
			if (helpBoxStyle == null) return base.GetHeight();
			helpBoxStyle.richText = true;
			var height = helpBoxStyle.CalcHeight(new GUIContent(string.Format("{0}----", helpBoxAttribute.text)), EditorGUIUtility.currentViewWidth) +
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