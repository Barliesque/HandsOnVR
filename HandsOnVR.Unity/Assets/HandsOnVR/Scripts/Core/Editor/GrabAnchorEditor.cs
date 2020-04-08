using UnityEditor;
using Barliesque.InspectorTools.Editor;
using UnityEngine;

namespace HandsOnVR.Editor
{

	[CustomEditor(typeof(GrabAnchor))]
	public class GrabAnchorEditor : EditorBase<GrabAnchor>
	{

		override protected void CustomInspector(GrabAnchor inst)
		{
			Grabbable grabbable = null;
			GrabAttachPoint attachPt = null;
			Hand grabHand = (Hand)0;
			bool handMatches = true;

				grabbable = inst.GetComponentInParent<Grabbable>(true);
				attachPt = inst.GetComponentInParent<GrabAttachPoint>(true);
				if (attachPt)
				{
					grabHand = attachPt.Grabber.Controller.Hand;
					handMatches = inst.SupportsHand(grabHand);
				}

			if (grabbable == null)
			{
				EditorGUILayout.HelpBox("GrabAnchors are intended to be placed on GameObject children of a GameObject with a Grabbable component.", MessageType.Error);
				EditorGUILayout.Space();
			}


			GUI.color = handMatches ? Color.white : new Color(1f, 0.8f, 0.8f, 1f);
			PropertyField("_primaryHand");
			GUI.color = Color.white;

			if (PropertyField("_mirrorForOtherHand").boolValue)
			{
				PropertyField("_mirrorActions");
			}
			PropertyField("_grabOrder");

			EditorTools.Header("Grabbable Overrides");
			EditorTools.BeginInfoBox();

			if (PropertyField("_overrideGrabPose").boolValue)
			{
				PropertyField("_grabPose");
			} else
			{
				GUI.enabled = false;
				EditorGUILayout.TextField("Grab Pose", grabbable.GrabPose);
				GUI.enabled = true;
			}

			//TODO  Support proximity pose overrides in GrabAnchors
			/*
			if (PropertyField("_overrideProximityPose").boolValue)
			{
				PropertyField("_proximityPose");
			}
			else
			{
				GUI.enabled = false;
				EditorGUILayout.TextField("Proximity Pose", grabbable.ProximityPose);
				GUI.enabled = true;
			}
			*/

			EditorTools.Separator();

			if (PropertyField("_overrideOrientToHand").boolValue)
			{
				PropertyField("_orientToHand");
			} else
			{
				GUI.enabled = false;
				EditorGUILayout.Toggle("Orient to Hand", grabbable.OrientToHand);
				GUI.enabled = true;
			}
			EditorTools.EndInfoBox();

			if (Application.isPlaying)
			{
				EditorTools.Header("Runtime Info");
				EditorTools.BeginInfoBox();
				GUI.enabled = false;

				var grabbedBy = (inst.Grabbable) ? (inst.Grabbable.GrabbedBy ?? inst.Grabbable.GrabbedBySecond) : null;
				if (grabbedBy && grabbedBy.GrabbedAnchor != (IGrabAnchor)inst) grabbedBy = null;

				EditorGUILayout.ObjectField("Grabbed By", grabbedBy, typeof(Grabber), true);
				GUI.enabled = true;
				EditorTools.EndInfoBox();
			}
			else if (grabbable)
			{
				if (attachPt)
				{
					EditorGUILayout.Space();
					EditorTools.BeginInfoBox();

					var anchorXform = inst.transform;
					var grabbableXform = grabbable.transform;
					var parentXform = grabbableXform.parent;

					GUI.color = handMatches ? Color.white : new Color(1f, 0.8f, 0.8f, 1f);
					if (GUILayout.Button("Set Anchor Alignment"))
					{
						Undo.RecordObject(anchorXform, "Set Anchor Alignment");
						anchorXform.position = parentXform.position;
						anchorXform.rotation = parentXform.rotation;
						PropertyField("_primaryHand").intValue = (int)grabHand;
					}
					GUI.color = Color.white;

					GUI.enabled = handMatches;
					if (GUILayout.Button("Align Grabbable to Anchor"))
					{
						Undo.RecordObject(grabbableXform, "Align Grabbable to Anchor");
						// Align rotation
						grabbableXform.localRotation = Quaternion.identity;
						grabbableXform.rotation = parentXform.rotation * Quaternion.Inverse(Quaternion.Euler(anchorXform.eulerAngles));
						//grabbableXform.rotation = parentXform.rotation * Quaternion.Euler(-anchorXform.eulerAngles);

						// Align position
						grabbableXform.localPosition = Vector3.zero;
						grabbableXform.position -= anchorXform.position - grabbableXform.position;
					}
					GUI.enabled = true;

					EditorTools.EndInfoBox();
				}
			}
		}

	}

}
