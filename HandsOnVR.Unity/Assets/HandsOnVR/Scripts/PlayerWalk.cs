using System;
using UnityEngine;
using Barliesque.InspectorTools;
using UnityEngine.XR;


namespace HandsOnVR
{

	public class PlayerWalk : MonoBehaviour
	{
		[HelpBox("To move forward, point either controller in the direction you'd like to go, and push the thumbstick forward.  Push the thumbstick back to go in the reverse direction.  Use both controllers together to run.")]

		[Tooltip("This is the distance to be travelled per second while walking.")]
		[SerializeField] private float _walkSpeed = 1.5f;
		
		[Tooltip("This is the distance to be travelled per second while running.")] 
		[SerializeField] private float _runSpeed = 4f;

		[Tooltip("If the controller is within this radius from the head position (on the X/Z plane) then gaze will determine the direction of travel instead of the controller orientation.  A value less than zero disables this feature.")]
		[SerializeField] private float _gazeRadius = 0.4f;

		[Header("Component Links")]
		[SerializeField] private GameObject _rightHandController;
		[SerializeField] private IHandController _rightHand;
		[SerializeField] private GameObject _leftHandController;
		[SerializeField] private IHandController _leftHand;
		[SerializeField] private Transform _head;

		private Transform _xform;
		private Transform _right;
		private Transform _left;

		private Vector3 _direction;
		private float _velocity;
		private bool _usingGaze;

		private float _smoothT;
		
		private void Start()
		{
			_leftHand = _leftHandController.GetComponent<IHandController>();
			_rightHand = _rightHandController.GetComponent<IHandController>();
			if (_leftHand == null || _rightHand == null)
			{
				throw new Exception("PlayerWalk is missing component links!");
			}

			_xform = GetComponent<Transform>();
			_right = _rightHand.GetComponent<Transform>();
			_left = _leftHand.GetComponent<Transform>();
			
			
			_smoothT = Mathf.LerpUnclamped(0.125f, 0.0625f, Mathf.InverseLerp(0.2f, 0.1f, Time.fixedDeltaTime));
			Debug.Log(_smoothT);
		}


		private void FixedUpdate()
		{
			var leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
			var rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

			leftHand.TryGetFeatureValue(CommonUsages.primary2DAxis, out var leftStick);
			rightHand.TryGetFeatureValue(CommonUsages.primary2DAxis, out var rightStick);
			
			var absL = Mathf.Abs(leftStick.y);
			var absR = Mathf.Abs(rightStick.y);
			var sign = Mathf.Sign(leftStick.y + rightStick.y);
			var speed = Mathf.Abs(leftStick.y + rightStick.y);
			speed *= speed * speed;
			speed = speed > 1f ? Mathf.Lerp(_walkSpeed, _runSpeed, speed - 1f) : Mathf.Lerp(0f, _walkSpeed, speed);
			_velocity = Mathf.Lerp(_velocity, speed * sign,  _smoothT);

			// If either hand is beyond a specified radius from the head (on the XZ-plane) that hand's direction is used, otherwise the head's direction is used.
			var leftDeltaXZ = (_leftHand.Position - _head.position);
			leftDeltaXZ.y = 0f;
			var rightDeltaXZ = (_rightHand.Position - _head.position);
			rightDeltaXZ.y = 0f;
			var leftPointing = leftDeltaXZ.sqrMagnitude > (_gazeRadius * _gazeRadius) && absL > 0.25f;
			var rightPointing = rightDeltaXZ.sqrMagnitude > (_gazeRadius * _gazeRadius) && absR > 0.25f;

			// Change between gaze and controller direction? 
			//if (absL < 0.1f && absR < 0.1f)
			{
				_usingGaze = !leftPointing && !rightPointing;
			}
			
			if (_usingGaze)
			{
				// Gaze controls direction
				var headAngle = _head.eulerAngles.y * Mathf.Deg2Rad;
				_direction = new Vector3(Mathf.Sin(headAngle), 0f, Mathf.Cos(headAngle));
			}
			else
			{
				// Blend the directions of the two hands, balanced by strength of input
				_direction = (_left.forward * absL + _right.forward * absR);
				_direction.y = 0f;
				_direction.Normalize();
			}

			_xform.position += _direction * (_velocity * Time.fixedDeltaTime);

			

			/*
			bool head = false;
			if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
			{
				walk = 1f; 
				head = true;
			}
			if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
			{
				walk = 1f;
				head = true;
			}

			if (Mathf.Abs(walk) < 0.01f)
			{
				_velocity = Mathf.Lerp(_velocity, 0f, 0.125f);
			} 
			else 
			{
				if (head)
				{
					_direction = _head.forward * (forwardR ? 1f : -1f);
				}
				else if (right)
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
			
			*/
		}


	}

}