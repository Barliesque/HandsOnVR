using UnityEngine;
using UnityEditor;
using Barliesque.InspectorTools.Editor;

[CustomPropertyDrawer(typeof(Half))]
public class HalfDrawer : PropertyDrawerHelper
{
	override public void CustomDrawer()
	{
		var prop = Property.FindPropertyRelative("_half");
		var value = Mathf.HalfToFloat((ushort)prop.intValue);
		var result = FloatField(72f, value);
		if (result != value) {
			prop.intValue = Mathf.FloatToHalf(result);
		}
	}
}
