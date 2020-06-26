using Barliesque.InspectorTools.Editor;
using UnityEditor;
using UnityEngine;

namespace HandsOnVR.Editor
{

	[CustomEditor(typeof(Grabbable))]
	public class GrabbableEditor : EditorBase<Grabbable>
	{
		override protected void CustomInspector(Grabbable inst)
		{
			DrawDefaultInspector();

			if (Application.isPlaying)
			{
				EditorTools.Header("Runtime Info");
				EditorTools.BeginInfoBox();
				GUI.enabled = false;
				EditorGUILayout.ObjectField("Grabbed By", inst.GrabbedBy, typeof(Grabber), true);
				if (inst.GrabbedBy)
				{
					EditorGUILayout.ObjectField("...at anchor", inst.GrabbedBy.GrabbedAnchor as MonoBehaviour, typeof(IGrabAnchor), true);
				}
				EditorGUILayout.ObjectField("Grabbed By Second", inst.GrabbedBySecond, typeof(Grabber), true);
				if (inst.GrabbedBySecond)
				{
					EditorGUILayout.ObjectField("...at anchor", inst.GrabbedBySecond.GrabbedAnchor as MonoBehaviour, typeof(IGrabAnchor), true);
				}
				GUI.enabled = true;
				EditorTools.EndInfoBox();
			}
			else
			{
				var attachPt = inst.GetComponentInParent<GrabAttachPoint>(true);
				if (attachPt)
				{
					EditorGUILayout.Space();
					if (GUILayout.Button("Create a new GrabAnchor"))
					{
						var anyAnchor = inst.GetComponentInChildren<GrabAnchor>(true);
						var container = anyAnchor ? anyAnchor.transform.parent : null;
						if (container == null)
						{
							var go = new GameObject("Grab Anchors");
							container = go.transform;
							container.SetParent(inst.transform, false);
							Undo.RegisterCreatedObjectUndo(go, "Created Grab Anchors container");
						}
						var hand = attachPt.Grabber.Controller.Hand;
						var anchorGo = new GameObject($"Anchor ({hand.ToString().Substring(0, 1).ToUpper()})", typeof(GrabAnchor));
						anchorGo.transform.SetParent(container, false);
						var anchor = anchorGo.GetComponent<GrabAnchor>();
						var serial = new SerializedObject(anchor);
						var handProp = serial.FindProperty("_primaryHand");
						handProp.intValue = (int)hand;
						serial.ApplyModifiedProperties();
						Selection.objects = new Object[] { anchorGo };
						Undo.RegisterCreatedObjectUndo(anchorGo, "Created new Grab Anchor");
						GrabAnchorEditor.SetAnchorImmediate();
					}
				}
				else
				{
					EditorTools.HelpBox("To create/edit GrabAnchors, temporarily place this Grabbable object under the AttachPoint of either hand of the Player Rig.", MessageType.Info);
				}
			}

		}
	}

}