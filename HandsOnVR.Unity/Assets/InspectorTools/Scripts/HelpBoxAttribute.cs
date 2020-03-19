using UnityEngine;

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
		static public bool enabled = true;

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
}