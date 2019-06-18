using UnityEditor;
using Barliesque.InspectorTools.Editor;


namespace Barliesque.VRGrab.Editor
{

	[CustomEditor(typeof(GrabAnchor))]
	public class GrabAnchorEditor : EditorBase<GrabAnchor>
	{

		protected override void CustomInspector(GrabAnchor inst)
		{
			PropertyField("_allowHand");

			if (PropertyField("_overridePose").boolValue)
			{
				PropertyField("_grabPose");
			}

			if (PropertyField("_overrideOrientToHand").boolValue)
			{
				PropertyField("_orientToHand");
			}
		}

	}

}
