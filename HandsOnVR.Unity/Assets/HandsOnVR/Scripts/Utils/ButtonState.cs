using System;
using UnityEngine;


namespace HandsOnVR
{
	
	[Serializable]  // Open an Inspector panel in debug mode to monitor the state values in HandController
	public class ButtonState
	{
		private float _analog;
		private bool _wasActive;
		private float _threshold;
		private float _heldTime;


		/// <summary>
		/// Create a ButtonState to track the current state of a VR controller button
		/// </summary>
		/// <param name="threshold">The minimum analog value required for the button to be interpreted as active.</param>
		public ButtonState(float threshold = 0.5f)
		{
			_threshold = threshold;
		}


		/// <summary>
		/// To be called once every frame to update the current state of the button.
		/// </summary>
		/// <param name="analog">The analog value of the button, from 0.0 to 1.0</param>
		public void Update(float analog)
		{
			_wasActive = IsActive;
			_analog = analog;

			if (_wasActive && IsActive)
			{
				_heldTime += Time.unscaledDeltaTime;
			}
			else
			{
				_heldTime = 0f;
			}
		}


		/// <summary>
		/// Is the button currently pressed sufficiently to activate?
		/// </summary>
		public bool IsActive => _analog >= _threshold;

		/// <summary>
		/// Did the button become active this frame?
		/// </summary>
		public bool Began => IsActive && !_wasActive;

		/// <summary>
		/// Did the button become inactive this frame?
		/// </summary>
		public bool Ended => _wasActive && !IsActive;

		/// <summary>
		/// How long has the button been active?  Zero is returned if the button is not currently active.
		/// </summary>
		public float HeldTime => _heldTime;
	}

}