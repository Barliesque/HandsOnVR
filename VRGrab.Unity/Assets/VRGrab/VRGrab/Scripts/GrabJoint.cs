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
		public Transform TargetAnchor;
		[SerializeField] Rigidbody _hand;
		[Range(0f,1f)] public float Influence = 1f;

		const float _moveSpeed = 60f;
		const float _turnSpeed = 0.9f;


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
				var delta = (TargetAnchor.position - GrabbedAnchor.position) * (_moveSpeed / (1f + GrabbedBody.mass));
				//TODO  Calculate hand velocity in HandController and use that instead
				GrabbedBody.velocity = Vector3.Lerp(GrabbedBody.velocity, delta + _hand.velocity, Influence);


				// Find the angular delta, and convert from Quaternion to Angle/Axis
				var angDelta = TargetAnchor.rotation * Quaternion.Inverse(GrabbedAnchor.rotation);
				float angle;
				Vector3 axis;
				angDelta.ToAngleAxis(out angle, out axis);

				// Check that we're not already aligned
				if (!float.IsInfinity(axis.x))
				{
					if (angle > 180f) angle -= 360f;
					var newAngVel = (_turnSpeed * Mathf.Deg2Rad * angle / Time.fixedUnscaledDeltaTime) * axis.normalized;
					GrabbedBody.angularVelocity = Vector3.Lerp(GrabbedBody.angularVelocity, newAngVel, Influence);
				}
			}
		}

	}
}