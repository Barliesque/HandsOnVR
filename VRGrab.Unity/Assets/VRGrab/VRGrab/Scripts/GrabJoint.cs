using UnityEngine;


namespace Barliesque.VRGrab
{
	/// <summary>
	/// Bring a grabbed object to the hand, in a non-kinematic way
	/// </summary>
	public class GrabJoint : MonoBehaviour
	{
		public Rigidbody GrabbedBody;
		public Transform GrabbedAnchor;
		public Transform Target;

		public Transform SecondAnchor;
		public Transform SecondTarget;

		[SerializeField] Rigidbody _handBody;
		[Range(0f, 1f)] public float Influence = 1f;

		const float _moveSpeed = 60f;
		const float _turnSpeed = 0.9f;


		private void Start()
		{
			if (Target == null)
			{
				Target = GetComponent<Transform>();
			}
		}


		void FixedUpdate()
		{
			if (GrabbedBody != null && _handBody != null)
			{
				bool isTwoHanded = SecondTarget && SecondAnchor;

				// How far is the grabbed anchor from the hand?
				var mass = GrabbedBody.mass * (isTwoHanded ? 0.5f : 1f);
				var delta = (Target.position - GrabbedAnchor.position) * (_moveSpeed / (1f + mass));

				// Find the angular delta
				Quaternion angDelta;

				//TODO  Calculate hand velocity in HandController and use that instead
				//TODO  Should be a blend of both hands' velocities for two-handed
				var handVelocity = _handBody.velocity;

				if (isTwoHanded)
				{
					// Two hands are manipulating this object...

					// Blend the position deltas of the each hand compared to its anchor
					var delta2 = (SecondTarget.position - SecondAnchor.position) * (_moveSpeed / (1f + GrabbedBody.mass));
					delta = Vector3.Lerp(delta, delta2, 0.5f);

					// TODO  Calculate stretch, twist and bend
					// TODO  Add option to auto-release second hand if stretch goes beyond a tolerance.  Priority could be specified by GrabAnchors

					// Find the angle between the anchors
					var fromDir = (GrabbedAnchor.position - SecondAnchor.position).normalized;
					var fromUp = GrabbedBody.transform.up;
					var from = Quaternion.LookRotation(fromDir, fromUp);

					// Find the angle between the hands
					var toDir = (Target.position - SecondTarget.position).normalized;
					var toUp = Vector3.Lerp(Target.up, SecondTarget.up, 0.5f).normalized;
					var to = Quaternion.LookRotation(toDir, toUp);

					// Find the difference between those angles to turn both anchors towards their respective hands
					angDelta = to * Quaternion.Inverse(from);
				}
				else
				{
					angDelta = Target.rotation * Quaternion.Inverse(GrabbedAnchor.rotation);
				}

				// Move the object towards the hand(s)
				GrabbedBody.velocity = Vector3.Lerp(GrabbedBody.velocity, delta + handVelocity, Influence);

				// Convert angular delta from Quaternion to Angle/Axis
				float angle;
				Vector3 axis;
				angDelta.ToAngleAxis(out angle, out axis);

				// Check that we're not already aligned
				if (!float.IsInfinity(axis.x))
				{
					// Apply angular velocity to align the object to the hand(s)
					if (angle > 180f) angle -= 360f;
					var newAngVel = (_turnSpeed * Mathf.Deg2Rad * angle / Time.fixedUnscaledDeltaTime) * axis.normalized;
					GrabbedBody.angularVelocity = Vector3.Lerp(GrabbedBody.angularVelocity, newAngVel, Influence);
				}
			}
		}

	}
}