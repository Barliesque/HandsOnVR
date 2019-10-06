using System;
using UnityEngine;


namespace HandsOnVR
{


	public class PlayerWalk : MonoBehaviour
	{
		[HelpBox("To move forward, point either controller in the direction you'd like to go, and push the thumbstick forward.  Push the thumbstick back to go in the reverse direction.  Use both controllers together to run.")]

		[SerializeField, Tooltip("This is the distance to be travelled persecond while walking.")]
		float _walkSpeed = 1.5f;
		[SerializeField, Tooltip("This is the distance to be travelled persecond while running.")]
		float _runSpeed = 4f;

		[Header("Component Links")]
		[SerializeField] HandController _rightHand;
		[SerializeField] HandController _leftHand;

		Transform _xform;
		Transform _right;
		Transform _left;

		Vector3 _direction;
		float _velocity;

		void Start()
		{
			if (_leftHand == null || _rightHand == null)
			{
				throw new Exception("PlayerWalk is missing component links!");
			}

			_xform = GetComponent<Transform>();
			_right = _rightHand.GetComponent<Transform>();
			_left = _leftHand.GetComponent<Transform>();
		}


		void FixedUpdate()
		{
			var forwardR = OVRInput.Get(OVRInput.RawButton.RThumbstickUp);
			var forwardL = OVRInput.Get(OVRInput.RawButton.LThumbstickUp);
			var backwardR = OVRInput.Get(OVRInput.RawButton.RThumbstickDown);
			var backwardL = OVRInput.Get(OVRInput.RawButton.LThumbstickDown);
			var right = (forwardR || backwardR);
			var left = (forwardL || backwardL);
			var walk = right ^ left;
			var run = right && left && (forwardR == forwardL);

			if (!walk && !run)
			{
				_velocity = Mathf.Lerp(_velocity, 0f, 0.125f);
			}

			if (walk)
			{
				if (right)
				{
					_direction = _right.forward * (forwardR ? 1f : -1f);
				}
				else
				{
					_direction = _left.forward * (forwardL ? 1f : -1f);
				}
				_direction.y = 0f;
				_direction.Normalize();

				var speed = 1f - Mathf.Abs(Vector3.Dot(forwardR ? _right.forward : _left.forward, Vector3.down));
				_velocity = Mathf.Lerp(_velocity, speed * _walkSpeed, 0.125f);
			}

			if (run)
			{
				var speedR = 1f - Mathf.Abs(Vector3.Dot(_right.forward, Vector3.down));
				var speedL = 1f - Mathf.Abs(Vector3.Dot(_left.forward, Vector3.down));
				var dirR = _right.forward * (forwardR ? 1f : -1f);
				var dirL = _left.forward * (forwardL ? 1f : -1f);
				_direction = (dirR * speedR) + (dirL * speedL);
				_direction.y = 0f;
				_direction.Normalize();
				_velocity = Mathf.Lerp(_velocity, ((speedR + speedL) * 0.5f) * _runSpeed, 0.125f);
			}

			_xform.position += _direction * (_velocity * Time.fixedDeltaTime);
		}


	}


}