using UnityEngine;


namespace Barliesque.VRGrab
{
	/// <summary>
	/// Bring a grabbed object to the hand, by applying force to its Rigidbody
	/// </summary>
	public class GrabJoint : MonoBehaviour
	{
		[SerializeField] Rigidbody _grabbedBody;
		public Rigidbody GrabbedBody { get { return _grabbedBody; } }

		[SerializeField] Transform _grabbedAnchor;
		public Transform GrabbedAnchor { get { return _grabbedAnchor; } }

		[SerializeField] Transform _target;
		public Transform Target { get { return _target; } }

		[SerializeField] Transform _secondAnchor;
		public Transform SecondAnchor { get { return _secondAnchor; } }

		[SerializeField] Transform _secondTarget;
		public Transform SecondTarget { get { return _secondTarget; } }

		[SerializeField] Rigidbody _handBody;
		[SerializeField] float _engageTime = 0.25f;

		float _engaged = 1f;

		const float _moveSpeed = 60f;
		const float _turnSpeed = 0.9f;


		private void Start()
		{
			if (_target == null)
			{
				_target = GetComponent<Transform>();
			}
		}

		public void SetGrab(Rigidbody grabbedBody, Transform anchor)
		{
			_grabbedBody = grabbedBody;
			_grabbedAnchor = anchor;
			_engaged = (_engageTime > 0) ? 0f : 1f;
		}

		public void SetSecondGrab(Transform anchor, Transform target)
		{
			_secondAnchor = anchor;
			_secondTarget = target;
			_engaged = (_engageTime > 0) ? 0f : 1f;
		}



		void FixedUpdate()
		{
			if (_grabbedBody != null && _handBody != null)
			{
				bool isTwoHanded = _secondTarget && _secondAnchor;

				// How far is the grabbed anchor from the hand?
				var mass = _grabbedBody.mass * (isTwoHanded ? 0.5f : 1f);
				var delta = (_target.position - _grabbedAnchor.position) * (_moveSpeed / (1f + mass));

				// Find the angular delta
				Quaternion angDelta;

				//TODO  Calculate hand velocity in HandController and use that instead
				//TODO  Should be a blend of both hands' velocities for two-handed
				var handVelocity = _handBody.velocity;

				if (isTwoHanded)
				{
					// Two hands are manipulating this object...

					// Blend the position deltas of the each hand compared to its anchor
					var delta2 = (_secondTarget.position - _secondAnchor.position) * (_moveSpeed / (1f + _grabbedBody.mass));
					delta = Vector3.LerpUnclamped(delta, delta2, 0.5f * _engaged);

					// TODO  Calculate stretch, twist and bend
					// TODO  Add option to auto-release second hand if stretch goes beyond a tolerance.  Priority could be specified by GrabAnchors

					// Find the angle between the anchors
					var fromDir = (_grabbedAnchor.position - _secondAnchor.position).normalized;
					var fromUp = _grabbedAnchor.up;
					var from = Quaternion.LookRotation(fromDir, fromUp);

					// Find the angle between the hands
					var toDir = (_target.position - _secondTarget.position).normalized;
					var toUp = Vector3.Lerp(_target.up, _secondTarget.up, 0.5f).normalized;
					var to = Quaternion.LookRotation(toDir, toUp);

					// Find the difference between those angles to turn both anchors towards their respective hands
					angDelta = to * Quaternion.Inverse(from);
				}
				else
				{
					angDelta = _target.rotation * Quaternion.Inverse(_grabbedAnchor.rotation);
				}


				// Move the object towards the hand(s)
				if (_engaged < 1f)
				{
					_engaged = Mathf.Clamp01(_engaged + Time.fixedDeltaTime / _engageTime);
				}
				//! Not sure if there's any difference between these two in terms of natural behavior
				//_grabbedBody.velocity = Vector3.Lerp(_grabbedBody.velocity, delta + handVelocity, _engaged);
				_grabbedBody.AddForce(Vector3.LerpUnclamped(_grabbedBody.velocity, delta + handVelocity, _engaged) - _grabbedBody.velocity, ForceMode.VelocityChange);

				// Convert angular delta from Quaternion to Angle/Axis
				float angle;
				Vector3 axis;
				angDelta.ToAngleAxis(out angle, out axis);

				// Check that we're not already aligned
				if (!float.IsInfinity(axis.x))
				{
					// Apply angular velocity to align the object to the hand(s)
					if (angle > 180f) angle -= 360f;
					var magnitude = (_turnSpeed * Mathf.Deg2Rad * angle / Time.fixedUnscaledDeltaTime);
					var newAngVel = magnitude * axis.normalized;
					_grabbedBody.angularVelocity = Vector3.LerpUnclamped(_grabbedBody.angularVelocity, newAngVel, _engaged);
				}
			}
		}

	}
}