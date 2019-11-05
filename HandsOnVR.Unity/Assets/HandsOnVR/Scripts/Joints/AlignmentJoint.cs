using UnityEngine;


namespace HandsOnVR
{
	/// <summary>
	/// Draws a connected object towards the joint and brings it into alignment.  May be paired with the PropAttacher component.
	/// </summary>
	public class AlignmentJoint : MonoBehaviour, IJoint
	{

		[SerializeField] Transform _connectedTo;
		public Transform ConnectedTo
		{
			get => _connectedTo;
			set 
			{
				_connectedTo = value;
				_connectedBody = value.GetComponent<Rigidbody>();
				_engaged = (_engageTime > 0) ? 0f : 1f;
			}
		}


		[SerializeField] Rigidbody _connectedBody;
		public Rigidbody ConnectedBody => _connectedBody;

		Transform _xform;
		Rigidbody _body;

		[SerializeField] float _engageTime = 0.15f;

		float _engaged = 1f;

		const float _moveSpeed = 60f;
		const float _turnSpeed = 0.9f;


		private void Start()
		{
			_xform = GetComponent<Transform>();
			_body = GetComponentInParent<Rigidbody>();
		}


		void FixedUpdate()
		{
			if (_connectedBody)
			{
				// How far away is the connected object?
				var mass = _connectedBody.mass;
				var delta = (_xform.position - _connectedTo.position) * (_moveSpeed / (1f + mass));

				// Find the angular delta
				Quaternion angDelta;

				var ourVelocity = _body?.velocity ?? Vector3.zero;

				angDelta = _xform.rotation * Quaternion.Inverse(_connectedTo.rotation);


				// Move the object towards the joint
				if (_engaged < 1f)
				{
					_engaged = Mathf.Clamp01(_engaged + Time.fixedDeltaTime / _engageTime);
				}
				//! Not sure if there's any difference between these two in terms of natural behavior
				//_grabbedBody.velocity = Vector3.Lerp(_grabbedBody.velocity, delta + handVelocity, _engaged);
				_connectedBody.AddForce(Vector3.LerpUnclamped(_connectedBody.velocity, delta + ourVelocity, _engaged) - _connectedBody.velocity, ForceMode.VelocityChange);

				// Convert angular delta from Quaternion to Angle/Axis
				angDelta.ToAngleAxis(out float angle, out Vector3 axis);

				// Check that we're not already aligned
				if (float.IsInfinity(axis.x)) return;
				
				// Apply angular velocity to align the object to the hand(s)
				if (angle > 180f) angle -= 360f;
				var magnitude = (_turnSpeed * Mathf.Deg2Rad * angle / Time.fixedUnscaledDeltaTime);
				var newAngVel = magnitude * axis.normalized;
				_connectedBody.angularVelocity = Vector3.LerpUnclamped(_connectedBody.angularVelocity, newAngVel, _engaged);
			}
		}

	}
}