﻿using UnityEngine;


namespace Barliesque.VRGrab
{
	/// <summary>
	/// Bring a grabbed object to the hand, in a non-kinematic way
	/// </summary>
	public class GrabJoint : MonoBehaviour
	{
		public Rigidbody GrabbedBody;
		public Transform GrabbedAnchor;
		public Transform TargetAnchor;
		public float MoveSpeed = 60f;
		[Range(0f,1f)] public float TurnSpeed = 0.9f;
		[SerializeField] Rigidbody _hand;


		private void Start()
		{
			if (TargetAnchor == null)
			{
				TargetAnchor = GetComponent<Transform>();
			}
		}

		//TODO  This technique works well for small objects, but larger objects will need a different approach, so that they are pulled from a specified anchor point - Consider option to use ConfigurableJoint for big heavy objects

		void FixedUpdate()
		{
			if (GrabbedBody != null && _hand != null)
			{
				// Move object towards hand
				var delta = (TargetAnchor.position - GrabbedAnchor.position) * (MoveSpeed / (1f + GrabbedBody.mass));
				GrabbedBody.velocity = delta + _hand.velocity; //TODO  Calculate hand velocity in HandController and use that instead

				// Find the angular delta, and convert from Quaternion to Angle/Axis
				var angDelta = TargetAnchor.rotation * Quaternion.Inverse(GrabbedAnchor.rotation);
				float angle;
				Vector3 axis;
				angDelta.ToAngleAxis(out angle, out axis);

				// Check that we're not already aligned
				if (!float.IsInfinity(axis.x))
				{
					if (angle > 180f) angle -= 360f;
					GrabbedBody.angularVelocity = (TurnSpeed * Mathf.Deg2Rad * angle / Time.fixedUnscaledDeltaTime) * axis.normalized;
				}
			}
		}

	}
}