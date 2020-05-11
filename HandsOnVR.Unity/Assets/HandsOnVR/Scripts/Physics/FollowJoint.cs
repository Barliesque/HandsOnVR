using UnityEngine;
using UnityEngine.Serialization;


namespace HandsOnVR
{
	/// <summary>
	/// Make this object follow a transform, in a physics-based manner.
	/// </summary>
	[RequireComponent(typeof(Rigidbody))]
	[ExecuteInEditMode]
	public class FollowJoint : MonoBehaviour, IJoint
	{
		[FormerlySerializedAs("Target")] public Transform _connectedTo;
		public Transform ConnectedTo
		{
			get => _connectedTo;
			set => _connectedTo = value;
		}
		
		public float MoveSpeed = 60f;
		[Range(0f, 1f)] public float TurnSpeed = 0.9f;

		[Tooltip("When the target is further than this distance, the Rigidbody's position will be matched immediately.")]
		public float TeleportThreshold = 0.25f;

		private Transform _xform;
		private Rigidbody _body;


		private void Start()
		{
			_body = GetComponent<Rigidbody>();
			_body.isKinematic = false;
			_body.useGravity = false;
		}


#if UNITY_EDITOR
		private void Update()
		{
			// While the application is not playing...
			if (!Application.isPlaying)
			{
				// Make sure we have a Rigidbody
				if (!_body)
				{
					_body = GetComponent<Rigidbody>();
					if (!_body)
					{
						_body = gameObject.AddComponent<Rigidbody>();
					}
				}

				// Make sure it's non-kinematic
				_body.isKinematic = false;
				_body.useGravity = false;

				if (!_connectedTo)
				{
					return;
				}
				// Match the target position/rotation immediately
				_xform.position = _connectedTo.position;
				_xform.rotation = _connectedTo.rotation;
			}
		}
#endif


		private void OnEnable()
		{
			if (_xform == null)
			{
				_xform = GetComponent<Transform>();
			}
			if (_connectedTo != null)
			{
				// Match the target position/rotation immediately
				_xform.position = _connectedTo.position;
				_xform.rotation = _connectedTo.rotation;
				if (_body != null)
				{
					_body.velocity = Vector3.zero;
					_body.angularVelocity = Vector3.zero;
				}
			}
		}


		private void FixedUpdate()
		{
			if (_connectedTo)
			{
				var delta = (_connectedTo.position - _body.position);

				if (delta.sqrMagnitude >= (TeleportThreshold * TeleportThreshold))
				{
					// Teleport Rigidbody to target position
					_body.position = _connectedTo.position;
					_body.velocity = Vector3.zero;
				} else
				{
					// Move Rigidbody towards target position
					_body.velocity = delta * (MoveSpeed / (1f + _body.mass));
				}

				// Find the angular delta, and convert from Quaternion to Angle/Axis
				var angDelta = _connectedTo.rotation * Quaternion.Inverse(_body.rotation);
				float angle;
				Vector3 axis;
				angDelta.ToAngleAxis(out angle, out axis);

				// Check that we're not already aligned
				if (!float.IsInfinity(axis.x))
				{
					if (angle > 180f) angle -= 360f;
					var angVel = (TurnSpeed * Mathf.Deg2Rad * angle / Time.fixedUnscaledDeltaTime) * axis.normalized;
					_body.angularVelocity = float.IsInfinity(angVel.sqrMagnitude) ? Vector3.zero : angVel;
				}
			}
		}

	}
}