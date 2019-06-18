using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Barliesque.InspectorTools
{
	public enum HelpBoxType
	{
		None = 0,
		Info = 1,
		Warning = 2,
		Error = 3
	}
	public class HelpBoxAttribute : PropertyAttribute
	{
		static public bool enabled;

		public string text;
		public HelpBoxType messageType;
		public int spaceAbove;
		public int spaceBelow;
		public HelpBoxAttribute(string text, HelpBoxType messageType = HelpBoxType.None, int spaceAbove = 12, int spaceBelow = 4)
		{
			this.text = text;
			this.messageType = messageType;
			this.spaceAbove = spaceAbove;
			this.spaceBelow = spaceBelow;
		}
	}

#if UNITY_EDITOR
	[CustomPropertyDrawer(typeof(HelpBoxAttribute))]
	public class HelpBoxAttributeDrawer : DecoratorDrawer
	{
		public override float GetHeight()
		{
			if (!HelpBoxAttribute.enabled) return 0f;
			var helpBoxAttribute = attribute as HelpBoxAttribute;
			if (helpBoxAttribute == null) return base.GetHeight();
			var helpBoxStyle = (GUI.skin != null) ? GUI.skin.GetStyle("helpbox") : null;
			if (helpBoxStyle == null) return base.GetHeight();
			var height = helpBoxStyle.CalcHeight(new GUIContent(string.Format("{0}----", helpBoxAttribute.text)), EditorGUIUtility.currentViewWidth) + helpBoxAttribute.spaceAbove + helpBoxAttribute.spaceBelow;
			return (helpBoxAttribute.messageType == HelpBoxType.None) ? height : Mathf.Max(height, 55f);
		}
		public override void OnGUI(Rect position)
		{
			if (!HelpBoxAttribute.enabled) return;
			var helpBoxAttribute = attribute as HelpBoxAttribute;
			if (helpBoxAttribute == null) return;
			position.y += helpBoxAttribute.spaceAbove;
			position.height -= (helpBoxAttribute.spaceAbove + helpBoxAttribute.spaceBelow);
			EditorGUI.HelpBox(position, helpBoxAttribute.text, (MessageType)GetHelpBoxType(helpBoxAttribute.messageType));
		}
		private HelpBoxType GetHelpBoxType(HelpBoxType type)
		{
			switch (type) {
				default:
				case HelpBoxType.None: return HelpBoxType.None;
				case HelpBoxType.Info: return HelpBoxType.Info;
				case HelpBoxType.Warning: return HelpBoxType.Warning;
				case HelpBoxType.Error: return HelpBoxType.Error;
			}
		}
	}
#endif
}