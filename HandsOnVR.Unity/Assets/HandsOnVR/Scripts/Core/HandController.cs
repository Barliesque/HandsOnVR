using System.Collections.Generic;
using Barliesque.InspectorTools;
using UnityEngine;
using UnityEngine.XR;


namespace HandsOnVR
{
	/// <summary>
	/// Match this transform to an Oculus Touch controller and monitor the states of its buttons.
	/// </summary>
	public class HandController : MonoBehaviour, IHandController
	{
		[SerializeField, SingleSelection] private Hand _hand;
		public Hand Hand => _hand;

		// Note: Open an Inspector panel in debug mode to monitor these ButtonState values at runtime

		public ButtonState Grip { get; protected set; } = new ButtonState();
		public ButtonState Trigger { get; protected set; } = new ButtonState();
		public ButtonState AorX { get; protected set; } = new ButtonState();
		public ButtonState BorY { get; protected set; } = new ButtonState();

		public bool IsConnected { get; private set; }

		private Transform _xform;

		private Transform Xform
		{
			get
			{
				if (_xform == null) _xform = GetComponent<Transform>();
				return _xform;
			}
		}

		public Vector3 Position => Xform.position;
		public Quaternion Rotation => Xform.rotation;

		private List<XRNodeState> _states = new List<XRNodeState>();

		private void Awake()
		{
			if (_xform == null) _xform = GetComponent<Transform>();
		}

		private void Update()
		{
			var node = _hand == Hand.Left ? XRNode.LeftHand : XRNode.RightHand;
			var device = InputDevices.GetDeviceAtXRNode(node);

			// Match this transform to the controller's position & rotation
			InputTracking.GetNodeStates(_states);
			IsConnected = false;
			for (var i = 0; i < _states.Count; i++)
			{
				var state = _states[i];
				if (state.nodeType == node)
				{
					if (state.TryGetPosition(out var pos))
					{
						_xform.localPosition = pos;
						if (state.TryGetRotation(out var rot))
						{
							_xform.localRotation = rot;
							IsConnected = true;
						}
					}
					break;
				}
			}

			// Update the states of the Grip and Trigger analog buttons
			if (device.TryGetFeatureValue(CommonUsages.grip, out var value))
			{
				Grip.Update(value);
			}

			if (device.TryGetFeatureValue(CommonUsages.trigger, out value))
			{
				Trigger.Update(value);
			}


			// Update the A/X button, interpreting a touch with a value that can be seen in the ButtonState's analog value
			if (device.TryGetFeatureValue(CommonUsages.primaryButton, out var buttonValue))
			{
				if (buttonValue)
				{
					AorX.Update(1f);
				}
				else if (device.TryGetFeatureValue(CommonUsages.primaryTouch, out buttonValue))
				{
					AorX.Update(buttonValue ? 0.25f : 0f);
				}
				else
				{
					AorX.Update(0f);
				}
			}

			// Update the B/Y button, interpreting a touch with a value that can be seen in the ButtonState's analog value
			if (device.TryGetFeatureValue(CommonUsages.secondaryButton, out buttonValue))
			{
				if (buttonValue)
				{
					BorY.Update(1f);
				}
				else if (device.TryGetFeatureValue(CommonUsages.secondaryTouch, out buttonValue))
				{
					BorY.Update(buttonValue ? 0.25f : 0f);
				}
				else
				{
					BorY.Update(0f);
				}
			}
			
		}
	}
}