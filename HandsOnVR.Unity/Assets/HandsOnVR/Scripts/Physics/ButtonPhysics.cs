using Barliesque.InspectorTools;
using UnityEngine;
using UnityEngine.Events;

namespace HandsOnVR
{

	[RequireComponent(typeof(Rigidbody))]
	public class ButtonPhysics : MonoBehaviour
	{
		[SerializeField, SingleSelection] private Axis _axisOfMovement = Axis.Y;
		[SerializeField] private float _upPosition;
		[SerializeField] private float _downPosition;
		[SerializeField, Range(0f, 1f)] private float _springiness = 0.25f;
		public UnityEvent OnButtonPressed;
		public UnityEvent OnButtonReleased;

		public bool IsDown { get; private set; }
		public bool IsUp { get; private set; }


		private Transform _xform;
		private Rigidbody _body;
		private Vector3 _motorDirection;
		private bool _wasPressed;

		private void Awake()
		{
			_xform = GetComponent<Transform>();
			AxisSetup();
		}


		private void AxisSetup()
		{
			_body = GetComponent<Rigidbody>();
			_body.constraints = RigidbodyConstraints.FreezeRotation |
				(_axisOfMovement == Axis.X ? (RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ) : 0) |
				(_axisOfMovement == Axis.Y ? (RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ) : 0) |
				(_axisOfMovement == Axis.Z ? (RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY) : 0);
			_body.useGravity = false;
			_body.isKinematic = false;

			_motorDirection = SetAxis(Vector3.zero, _axisOfMovement, 1f);
		}


		private void FixedUpdate()
		{
			var pos = (_axisOfMovement == Axis.X ? _xform.localPosition.x : (_axisOfMovement == Axis.Y ? _xform.localPosition.y : _xform.localPosition.z));
			var movement = Mathf.Sign(_upPosition - _downPosition);
			var range = Mathf.Abs(_upPosition - _downPosition);
			var inside = ((movement > 0 && pos > _downPosition && pos < _upPosition) || (movement < 0 && pos > _upPosition && pos < _downPosition));
			if (inside)
			{
				// Spring towards the up position
				_body.AddForce(_motorDirection * (range * _springiness / Time.fixedUnscaledDeltaTime), ForceMode.Force);
				IsDown = IsUp = false;
			} else
			{
				// Enforce limits
				var position = _xform.localPosition;
				bool wasDown = IsDown;
				bool wasUp = IsUp;
				if (movement > 0)
				{
					IsUp = (pos >= _upPosition);
					IsDown = (pos <= _downPosition);
					if (!wasDown && IsDown) ButtonDown();
					if (!wasUp && IsUp) ButtonUp();
					position = SetAxis(position, _axisOfMovement, IsUp ? _upPosition : _downPosition);
				} else
				{
					IsUp = (pos <= _upPosition);
					IsDown = (pos >= _downPosition);
					if (!wasDown && IsDown) ButtonDown();
					if (!wasUp && IsUp) ButtonUp();
					position = SetAxis(position, _axisOfMovement, IsUp ? _upPosition : _downPosition);
				}
				_xform.localPosition = position;
				_body.MovePosition(_xform.position);
				_body.velocity = Vector3.zero;
			}
		}

		private void ButtonDown()
		{
			IsDown = true;
			_wasPressed = true;
			OnButtonPressed?.Invoke();
		}
		private void ButtonUp()
		{
			_wasPressed = false;
			OnButtonReleased?.Invoke();
		}

		private Vector3 SetAxis(Vector3 position, Axis axis, float value)
		{
			if (axis == Axis.X) position.x = value;
			else if (axis == Axis.Y) position.y = value;
			else if (axis == Axis.Z) position.z = value;
			return position;
		}

		private float GetAxis(Vector3 position, Axis axis)
		{
			if (axis == Axis.X) return position.x;
			else if (axis == Axis.Y) return position.y;
			return position.z;
		}

		/// <summary>
		/// Returns the current position of the button as a value from 0 to 1, where 1 is all the way down and 0 is all the way up.
		/// </summary>
		public float NormalizedPosition
		{
			get {
				var pos = GetAxis(_xform.localPosition, _axisOfMovement);
				return Mathf.InverseLerp(_upPosition, _downPosition, pos);
			}
		}

	}
}