using UnityEngine;


namespace HandsOnVR
{
	/// <summary>
	/// Bring a grabbed object to the hand, by applying force to its Rigidbody
	/// </summary>
	public class GrabAttacher : MonoBehaviour
	{
		[SerializeField] private Rigidbody _grabbedBody;
		public Rigidbody GrabbedBody => _grabbedBody;

		[SerializeField] private IGrabAnchor _grabbedAnchor;
		public IGrabAnchor GrabbedAnchor => _grabbedAnchor;

		[SerializeField] private Transform _target;
		public Transform Target => _target;

		[SerializeField] private IGrabAnchor _secondAnchor;
		public IGrabAnchor SecondAnchor => _secondAnchor;

		[SerializeField] private Transform _secondTarget;
		public Transform SecondTarget => _secondTarget;

		[SerializeField] private Rigidbody _handBody;

		public bool DisableForce { get; set; }

		private float _engaged = 1f;
		private Hand _firstGrabbedBy;
		private const float _moveSpeed = 60f;
		private const float _turnSpeed = 0.9f;


		private void Start()
		{
			if (_target == null)
			{
				_target = GetComponent<Transform>();
			}
		}

		public void SetGrab(Rigidbody grabbedBody, IGrabAnchor anchor, Hand grabbedBy)
		{
			_grabbedBody = grabbedBody;
			_grabbedAnchor = anchor;
			_firstGrabbedBy = grabbedBy;
		}

		public void SetSecondGrab(IGrabAnchor anchor, Transform target)
		{
			_secondAnchor = anchor;
			_secondTarget = target;
		}


		private void FixedUpdate()
		{
			if (DisableForce) return;
			if (_grabbedBody && _handBody)
			{
				bool isTwoHanded = _secondTarget && (_secondAnchor != null);

				// How far is the grabbed anchor from the hand?
				var mass = _grabbedBody.mass * (isTwoHanded ? 0.5f : 1f);
				var delta = (_target.position - _grabbedAnchor.GetPosition(_firstGrabbedBy)) * (_moveSpeed / (1f + mass));

				// Find the angular delta
				Quaternion angDelta;

				//TODO  Calculate hand velocity in HandController and use that instead
				//TODO  Should be a blend of both hands' velocities for two-handed
				var handVelocity = _handBody.velocity;
				var handAngVelocity = _handBody.angularVelocity;

				// Move the object towards the hand(s)
				if (_engaged < 1f)
				{
					_engaged = Mathf.Lerp(_engaged, 1f, 0.125f);
				}

				if (isTwoHanded)
				{
					// Two hands are manipulating this object...
					var secondHand = _firstGrabbedBy == Hand.Left ? Hand.Right : Hand.Left;
					var secondAnchorPos = _secondAnchor.GetPosition(secondHand);

					// Blend the position deltas of the each hand compared to its anchor
					var delta2 = (_secondTarget.position - secondAnchorPos) * (_moveSpeed / (1f + _grabbedBody.mass));
					delta = Vector3.LerpUnclamped(delta, delta2, 0.5f * _engaged);

					// TODO  Calculate stretch, twist and bend
					// TODO  Add option to auto-release second hand if stretch goes beyond a tolerance.  Priority could be specified by GrabAnchors

					// Find the angle between the anchors
					var fromDir = (_grabbedAnchor.GetPosition(_firstGrabbedBy) - secondAnchorPos).normalized;
					var fromUp = _secondAnchor.GetUp(secondHand);
					var from = Quaternion.LookRotation(fromDir, fromUp);

					// Find the angle between the hands
					var toDir = (_target.position - secondAnchorPos).normalized;
					var toUp = Vector3.Lerp(_target.up, _secondTarget.up, 0.5f).normalized;
					var to = Quaternion.LookRotation(toDir, toUp);

					// Find the difference between those angles to turn both anchors towards their respective hands
					angDelta = to * Quaternion.Inverse(from);
				}
				else
				{
					angDelta = _target.rotation * Quaternion.Inverse(_grabbedAnchor.GetRotation(_firstGrabbedBy));
				}


				//! Not sure if there's any difference between these two in terms of natural behavior
				//_grabbedBody.velocity = Vector3.Lerp(_grabbedBody.velocity, delta + handVelocity, _engaged);
				_grabbedBody.AddForce(Vector3.Lerp(_grabbedBody.velocity, delta + handVelocity, _engaged) - _grabbedBody.velocity, ForceMode.VelocityChange);

				// Convert angular delta from Quaternion to Angle/Axis
				angDelta.ToAngleAxis(out float angle, out Vector3 axis);

				// Check that we're not already aligned
				if (float.IsInfinity(axis.x)) return;

				// Apply angular velocity to align the object to the hand controller(s)
				if (angle > 180f) angle -= 360f;
				var magnitude = (_turnSpeed * Mathf.Deg2Rad * angle / Time.fixedUnscaledDeltaTime);
				var newAngVel = magnitude * axis.normalized;
				_grabbedBody.angularVelocity = Vector3.Lerp(_grabbedBody.angularVelocity, newAngVel, _engaged);
				_grabbedBody.AddTorque(handAngVelocity, ForceMode.VelocityChange);
			} else
			{
				// Nothing is grabbed, so disengage
				_engaged = 0f;
			}
		}

		public void SnapOrient(Axis twistAxis = Axis.X | Axis.Y | Axis.Z)
		{
			if (!_target || !_grabbedBody)
			{
				Debug.LogError("Cannot SnapOrient because the body has not been attached yet.");
				return;
			}
			var target = _target.rotation.eulerAngles;
			var rot = _grabbedBody.rotation.eulerAngles;
			if ((twistAxis & Axis.X) != 0) rot.x = target.x;
			if ((twistAxis & Axis.Y) != 0) rot.y = target.y;
			if ((twistAxis & Axis.Z) != 0) rot.z = target.z;
			_grabbedBody.MoveRotation(Quaternion.Euler(rot));
			_grabbedBody.transform.eulerAngles = rot;
		}
		
	}
}