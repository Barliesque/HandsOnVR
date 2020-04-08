using UnityEditor;
using Barliesque.InspectorTools.Editor;

namespace HandsOnVR.Editor
{
	[CustomEditor(typeof(Grabber))]
	public class GrabberEditor : EditorBase<Grabber>
	{
		protected override void CustomInspector(Grabber inst)
		{
			PropertyField("_controller");
			PropertyField("_handSolid");
			PropertyField("_handGhost");
			PropertyField("_focusPoint");
			PropertyField("_solidHandMover");
			PropertyField("_handColliders");
			PropertyField("_otherHand");
			PropertyField("_grabbableLayers");


			if (EditorApplication.isPlaying)
			{
				EditorTools.BeginInfoBox();
				EditorGUILayout.LabelField($"Grabbed Anchor: {inst.GrabbedAnchor}");
				EditorTools.EndInfoBox();
			}
		}
	}

}