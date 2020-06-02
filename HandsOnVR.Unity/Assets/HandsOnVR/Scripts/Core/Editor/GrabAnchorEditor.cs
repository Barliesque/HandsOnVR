using UnityEditor;
using Barliesque.InspectorTools.Editor;
using UnityEngine;


namespace HandsOnVR.Editor
{

	[CustomEditor(typeof(GrabAnchor))]
	public class GrabAnchorEditor : EditorBase<GrabAnchor>
	{

		static public void SetAnchorImmediate() => _setAnchorImmediate = true;
		static private bool _setAnchorImmediate = false;
		

		static private readonly GUIContent _setAnchorButton = new GUIContent()
		{
			text = "Set Anchor Alignment",
			tooltip = "Based on the Grabbable object's current position/rotation relative to the hand, set this anchor's Transform properties."
		};
		static private readonly GUIContent _alignGrabbableButton = new GUIContent()
		{
			text = "Align Grabbable to Anchor",
			tooltip = "Reposition the Grabbable object such that it's aligned to the hand, as specified by this Grab Anchor."
		};
		static private readonly GUIContent _alignHandButton = new GUIContent()
		{
			text = "Align Hand to Anchor",
			tooltip = "Reposition the Hand such that it's aligned to this Grab Anchor.  The Grabbable's world position/rotation will be preserved."
		};
		

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

			if (!grabbable)
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
			} else if (grabbable)
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
					if (GUILayout.Button(_setAnchorButton) || _setAnchorImmediate)
					{
						Undo.RecordObject(anchorXform, "Set Anchor Alignment");
						anchorXform.position = parentXform.position;
						anchorXform.rotation = parentXform.rotation;
						PropertyField("_primaryHand").intValue = (int)grabHand;
						_setAnchorImmediate = false;
					}
					GUI.color = Color.white;

					GUI.enabled = handMatches;

					if (GUILayout.Button(_alignGrabbableButton))
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

					if (GUILayout.Button(_alignHandButton))
					{
						var handXform = inst.GetComponentInParent<SolidHandMover>().transform.parent;
						var attachPoint = inst.GetComponentInParent<GrabAttachPoint>().transform;
						
						// Save the grabbable's world pos/rot
						var worldPos = grabbableXform.position;
						var worldRot = grabbableXform.rotation;
						
						// Move the hand
						Undo.RecordObjects(new Object[] { grabbableXform, handXform }, "Align Hand to Anchor");

						var targetRot = anchorXform.rotation;
						var targetPos = anchorXform.position;
						
						var rotOffset = handXform.InverseTransformRotation(attachPoint.rotation);
						handXform.rotation = targetRot * Quaternion.Inverse(rotOffset);
						
						var posOffset = attachPoint.position - handXform.position;
						handXform.position = targetPos - posOffset;
						
						// Restore the grabbable's world pos/rot
						grabbableXform.position = worldPos;
						grabbableXform.rotation = worldRot;
						
					}
					GUI.enabled = true;

					EditorTools.EndInfoBox();
				}
			}
		}

	}

}
