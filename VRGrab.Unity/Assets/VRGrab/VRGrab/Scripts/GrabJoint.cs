using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Barliesque.VRGrab
{
	/// <summary>
	/// 
	/// </summary>
	public class GrabJoint : MonoBehaviour
	{
		public Rigidbody GrabbedBody;
		public Transform TargetAnchor;
		public float TargetDist = 0f;
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


		void FixedUpdate()
		{
			if (GrabbedBody != null && _hand != null)
			{
				// Move object towards hand
				var delta = (TargetAnchor.position - GrabbedBody.position) * (MoveSpeed / (1f + GrabbedBody.mass));
				GrabbedBody.velocity = delta + _hand.velocity; //TODO  Calculate hand velocity in HandController and use that instead

				// Find the angular delta, and convert from Quaternion to Angle/Axis
				var angDelta = TargetAnchor.rotation * Quaternion.Inverse(GrabbedBody.rotation);
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