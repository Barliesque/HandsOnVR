using UnityEngine;
using UnityEngine.XR;

namespace HandsOnVR
{

	public class PlayerReorient : MonoBehaviour
	{

		[SerializeField] private Increment _increment = Increment._45Degrees;
		public enum Increment
		{
			_15Degrees = 15,
			_30Degrees = 30,
			_45Degrees = 45,
			_90Degrees = 90
		}

		[Header("Component Links")]
		[SerializeField] private Transform _head;

		private Transform _xform;

		private readonly ButtonState _turnLeft = new ButtonState();
		private readonly ButtonState _turnRight = new ButtonState();


		private void Start()
		{
			_xform = GetComponent<Transform>();
		}


		private void Update()
		{
			var leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
			var rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

			leftHand.TryGetFeatureValue(CommonUsages.primary2DAxis, out var leftStick);
			rightHand.TryGetFeatureValue(CommonUsages.primary2DAxis, out var rightStick);

			_turnLeft.Update(Mathf.Clamp01(-leftStick.x - rightStick.x));
			_turnRight.Update(Mathf.Clamp01(leftStick.x + rightStick.x));
			
			if (_turnRight.Began)
			{
				_xform.RotateAround(_head.position, Vector3.up, (float)_increment);
			}
			if (_turnLeft.Began)
			{
				_xform.RotateAround(_head.position, Vector3.up, -(float)_increment);
			}

		}

	}
}