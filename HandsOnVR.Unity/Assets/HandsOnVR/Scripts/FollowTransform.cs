using UnityEngine;


namespace HandsOnVR
{
	/// <summary>
	/// Make a Rigidbody follow a transform, in a non-kinematic manner.
	/// </summary>
	[RequireComponent(typeof(Rigidbody))]
	[ExecuteInEditMode]
	public class FollowTransform : MonoBehaviour
	{
		public Transform Target;
		public float MoveSpeed = 60f;
		[Range(0f, 1f)] public float TurnSpeed = 0.9f;

		[Tooltip("When the target is further than this distance, the Rigidbody's position will be matched immediately.")]
		public float TeleportThreshold = 0.25f;

		Rigidbody _body;


		void Start()
		{
			_body = GetComponent<Rigidbody>();
			_body.isKinematic = false;
			_body.useGravity = false;
		}


#if UNITY_EDITOR
		void Update()
		{
			// While the application is not playing...
			if (!Application.isPlaying)
			{
				// Make sure we have a Rigidbody
				if (_body == null)
				{
					_body = GetComponent<Rigidbody>();
					if (_body == null)
					{
						_body = gameObject.AddComponent<Rigidbody>();
					}
				}

				// Make sure it's non-kinematic
				_body.isKinematic = false;
				_body.useGravity = false;

				if (Target == null)
				{
					return;
				}
				// Match the target position/rotation immediately
				transform.position = Target.position;
				transform.rotation = Target.rotation;
			}
		}
#endif


		private void OnEnable()
		{
			if (Target != null)
			{
				// Match the target position/rotation immediately
				transform.position = Target.position;
				transform.rotation = Target.rotation;
				if (_body != null)
				{
					_body.velocity = Vector3.zero;
					_body.angularVelocity = Vector3.zero;
				}
			}
		}


		void FixedUpdate()
		{
			if (Target != null)
			{
				var delta = (Target.position - _body.position);

				if (delta.sqrMagnitude >= (TeleportThreshold * TeleportThreshold))
				{
					// Teleport Rigidbody to target position
					_body.position = Target.position;
					_body.velocity = Vector3.zero;
				} else
				{
					// Move Rigidbody towards target position
					_body.velocity = delta * (MoveSpeed / (1f + _body.mass));
				}

				// Find the angular delta, and convert from Quaternion to Angle/Axis
				var angDelta = Target.rotation * Quaternion.Inverse(_body.rotation);
				float angle;
				Vector3 axis;
				angDelta.ToAngleAxis(out angle, out axis);

				// Check that we're not already aligned
				if (!float.IsInfinity(axis.x))
				{
					if (angle > 180f) angle -= 360f;
					_body.angularVelocity = (TurnSpeed * Mathf.Deg2Rad * angle / Time.fixedUnscaledDeltaTime) * axis.normalized;
				}
			}
		}

	}
}