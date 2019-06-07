
using System;
using UnityEngine;

namespace Barliesque.VRGrab
{
	[Serializable]  // Open an Inspector panel in debug mode to monitor the state values
	public class ButtonState
	{
		float _analog;
		bool _wasActive;
		float _threshold;
		float _heldTime;


		/// <summary>
		/// Create a ButtonState to track the current state of a VR controller button
		/// </summary>
		/// <param name="threshold">The minimum analog value required for the button to be interpreted as active.</param>
		public ButtonState(float threshold = 0.5f)
		{
			_threshold = threshold;
		}


		/// <summary>
		/// For internal use.  Called every frame to update the current state of the button.
		/// </summary>
		/// <param name="analog">The analog value of the button, from 0.0 to 1.0</param>
		internal void Update(float analog)
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
		public bool IsActive
		{
			get { return _analog >= _threshold; }
		}

		/// <summary>
		/// Did the button become active this frame?
		/// </summary>
		public bool Began
		{
			get { return IsActive && !_wasActive; }
		}

		/// <summary>
		/// Did the button become inactive this frame?
		/// </summary>
		public bool Ended
		{
			get { return _wasActive && !IsActive; }
		}

		/// <summary>
		/// How long has the button been active?  Zero is returned if the button is not currently active.
		/// </summary>
		public float HeldTime
		{
			get { return _heldTime; }
		}
	}

}