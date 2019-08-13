using UnityEngine;

namespace Barliesque.InspectorTools
{

	public class EnumButtonsAttribute : PropertyAttribute
	{
		public bool AllowMultiple;
		public int MaxPerLine = 3;

		public EnumButtonsAttribute(bool allowMultiple = false, int maxPerLine = 3) {
			AllowMultiple = allowMultiple;
			MaxPerLine = maxPerLine;
		}
	}

}