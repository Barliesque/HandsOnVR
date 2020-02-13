using UnityEngine;

namespace HandsOnVR
{

	public class PlayerReorient : MonoBehaviour
	{

		[SerializeField] Increment _increment = Increment._45Degrees;
		public enum Increment
		{
			_15Degrees = 15,
			_30Degrees = 30,
			_45Degrees = 45,
			_90Degrees = 90
		}

		[Header("Component Links")]
		[SerializeField] Transform _head;

		Transform _xform;

		void Start()
		{
			_xform = GetComponent<Transform>();
		}


		void Update()
		{
			var turnRight = OVRInput.GetDown(OVRInput.RawButton.RThumbstickRight | OVRInput.RawButton.LThumbstickRight) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D);
			var turnLeft = OVRInput.GetDown(OVRInput.RawButton.RThumbstickLeft | OVRInput.RawButton.LThumbstickLeft) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A);

			if (turnRight)
			{
				_xform.RotateAround(_head.position, Vector3.up, (float)_increment);
			}
			if (turnLeft)
			{
				_xform.RotateAround(_head.position, Vector3.up, -(float)_increment);
			}

		}

	}
}