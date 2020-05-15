using Barliesque.InspectorTools;
using System;
using UnityEngine;


namespace HandsOnVR
{

	[Flags]
	public enum Hand { Left = 1, Right = 2 }


	/// <summary>
	/// Match this transform to an Oculus Touch controller and monitor the states of its buttons.
	/// </summary>
	public class HandController : HandControllerBase
	{
		[SerializeField, SingleSelection] private Hand _hand;
		override public Hand Hand { get { return _hand; } }

		// Note: Open an Inspector panel in debug mode to monitor these ButtonState values at runtime

		override public ButtonState Grip { get; protected set; } = new ButtonState();
		override public ButtonState Trigger { get; protected set; } = new ButtonState();
		override public ButtonState AorX { get; protected set; } = new ButtonState();
		override public ButtonState BorY { get; protected set; } = new ButtonState();
		override public ButtonState ThumbRest { get; protected set; } = new ButtonState();

		override public bool IsConnected
		{
			get
			{
				var controller = _hand == Hand.Left ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch;
				return (OVRInput.IsControllerConnected(controller));
			}
		}

		private Transform _xform;

		private Transform Xform
		{
			get
			{
				if (_xform == null) _xform = GetComponent<Transform>();
				return _xform;
			}
		}

		override public Vector3 Position => Xform.position;
		override public Quaternion Rotation => Xform.rotation;


		private void Update()
		{
			var controller = _hand == Hand.Left ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch;
			if (OVRInput.IsControllerConnected(controller))
			{
				// Match this transform to the controller's position & rotation
				Xform.localPosition = OVRInput.GetLocalControllerPosition(controller);
				_xform.localRotation = OVRInput.GetLocalControllerRotation(controller);

				// Update the states of the Grip and Trigger analog buttons
				Grip.Update(OVRInput.Get(_hand == Hand.Left ? OVRInput.RawAxis1D.LHandTrigger : OVRInput.RawAxis1D.RHandTrigger, controller));
				Trigger.Update(OVRInput.Get(_hand == Hand.Left ? OVRInput.RawAxis1D.LIndexTrigger : OVRInput.RawAxis1D.RIndexTrigger, controller));

				// Update the A/X button, interpreting a touch with a value that can be seen in the ButtonState's analog value
				if (OVRInput.Get(_hand == Hand.Left ? OVRInput.RawButton.X : OVRInput.RawButton.A, controller))
				{
					AorX.Update(1f);
				}
				else if (OVRInput.Get(_hand == Hand.Left ? OVRInput.RawTouch.X : OVRInput.RawTouch.A, controller))
				{
					AorX.Update(0.25f);
				}
				else
				{
					AorX.Update(0f);
				}

				// Update the B/Y button, interpreting a touch with a value that can be seen in the ButtonState's analog value
				if (OVRInput.Get(_hand == Hand.Left ? OVRInput.RawButton.Y : OVRInput.RawButton.B, controller))
				{
					BorY.Update(1f);
				}
				else if (OVRInput.Get(_hand == Hand.Left ? OVRInput.RawTouch.Y : OVRInput.RawTouch.B, controller))
				{
					BorY.Update(0.25f);
				}
				else
				{
					BorY.Update(0f);
				}

				// Update the state of the ThumbRest
				if (OVRInput.Get(_hand == Hand.Left ? OVRInput.RawTouch.LThumbRest : OVRInput.RawTouch.RThumbRest, controller))
				{
					ThumbRest.Update(1f);
				}
				else
				{
					ThumbRest.Update(0f);
				}

			}
		}

	}
}