using Barliesque.InspectorTools.Editor;
using UnityEditor;
using UnityEngine;

namespace HandsOnVR.Editor
{
	[CustomEditor(typeof(SliderJoint))]
	public class SliderJointEditor : EditorBase<SliderJoint>
	{
		protected override void CustomInspector(SliderJoint inst)
		{
			EditorGUI.BeginChangeCheck();

			var body = PropertyField("Body", "This Body");
			if (!body.objectReferenceValue)
			{
				body.objectReferenceValue = inst.GetComponentInParent<Rigidbody>();
			}

			PropertyField("ConnectedBody");
			PropertyField("AxisOfMovement");
			PropertyField("AnchorPosition");
			PropertyField("Limit");

			var rot = PropertyField("Rotation");
			if ((ConfigurableJointMotion)rot.intValue == ConfigurableJointMotion.Limited)
			{
				PropertyField("RotationLimitLow");
				PropertyField("RotationLimitHigh");
			}

			if (EditorGUI.EndChangeCheck() && Application.isPlaying)
			{
				serializedObject.ApplyModifiedProperties();
				inst.UpdateJoint();
			}
		}

		/*
		private void OnSceneGUI()
		{
			var inst = target as SliderJoint;
			var sign = Mathf.Sign(inst.Limit);
			var axis = (HasAxis(Axis.X) ? Vector3.right : (HasAxis(Axis.Y) ? Vector3.up : Vector3.forward)) * sign;
			var transform = inst.transform;
			var direction = transform.TransformDirection(axis);
			var from = transform.TransformPoint(inst.AnchorPosition);
			var to = transform.TransformPoint(inst.AnchorPosition + (inst.Limit * axis));

			Handles.color = Color.yellow;
			Handles.SphereHandleCap(0, from, Quaternion.identity, 0.02f, EventType.Repaint);
			Handles.DrawLine(from, to);
			Handles.ArrowHandleCap(0, to - (direction * 0.125f), Quaternion.LookRotation(direction, Vector3.up), 0.125f, EventType.Repaint);
		}

		bool HasAxis(Axis axis)
		{
			var inst = target as SliderJoint;
			return (inst.AxisOfMovement & axis) == axis;
		}
		*/

	}
}