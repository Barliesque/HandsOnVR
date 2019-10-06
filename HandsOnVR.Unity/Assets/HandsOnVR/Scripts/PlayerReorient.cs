using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
		var turnRight = OVRInput.GetDown(OVRInput.RawButton.RThumbstickRight | OVRInput.RawButton.LThumbstickRight);
		var turnLeft = OVRInput.GetDown(OVRInput.RawButton.RThumbstickLeft | OVRInput.RawButton.LThumbstickLeft);

		if (turnRight) {
			_xform.RotateAround(_head.position, Vector3.up, (float)_increment);
		}
		if (turnLeft) {
			_xform.RotateAround(_head.position, Vector3.up, -(float)_increment);
		}

	}

}
