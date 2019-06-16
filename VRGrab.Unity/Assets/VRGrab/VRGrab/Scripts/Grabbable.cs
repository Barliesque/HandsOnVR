using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Barliesque.VRGrab
{

	/// <summary>
	/// Make an object available to be picked up with the player's hands.  (See: Grabber.cs)
	/// </summary>
	[RequireComponent(typeof(Rigidbody))]
	public class Grabbable : MonoBehaviour
	{

		[Tooltip("A bool parameter name found in the Animator components of the player's hands.  While this object is being grabbed, the specified parameter will be set to true.")]
		[SerializeField] string _grabPose;
		public int GrabPoseID { get; private set; } // TODO Check for override in current anchor

		[SerializeField] bool _orientToHand = true;
		public bool OrientToHand { get { return _orientToHand; } } // TODO Check for override in current anchor

		public Grabber GrabbedBy { get; private set; }


		/// <summary>
		/// At the moment this object is grabbed, this event is fired.  If false is returned, the grab will be rejected.
		/// </summary>
		public event GrabHandler OnGrabbed;
		public delegate bool GrabHandler(Grabbable grabbed, Grabber grabbedBy);

		/// <summary>
		/// At the moment this object is released, this event is fired.
		/// </summary>
		public event ReleaseHandler OnReleased;
		public delegate void ReleaseHandler(Grabbable grabbed, Grabber fromGrabber);


		public Rigidbody Body { get; private set; }

		GrabAnchor[] _grabAnchors;
		int _currentAnchor = -1;

		// Maximum distance the hand may be from a hand anchor for that anchor to be grabbable
		public float MaxAnchorDistance { get; } = 0.25f;
		//TODO  This distance should be considered before Grabber determines which object is really the nearest... maybe


		virtual protected void Start()
		{
			GrabPoseID = Animator.StringToHash(_grabPose);
			Body = GetComponent<Rigidbody>();
			_grabAnchors = GetComponentsInChildren<GrabAnchor>();
		}

		//TODO  Consider adding option:  Allow one hand to grab object from the other?  Or allow *both* hands to grab simultaneously? ...or *require* both hands

		/// <summary>
		/// This object is being grabbed. Confirmation is returned whether grab is allowed.
		/// </summary>
		/// <param name="grabbedBy">The Grabber attempting to grab this object</param>
		/// <returns>True is returned if grab is allowed, or false if it is not.</returns>
		internal virtual bool TryGrab(Grabber grabbedBy, out Transform anchor)
		{
			if (GrabbedBy != null)
			{
				//TODO  Add options to:  Swap from one hand to the other;  Object stays in first hand that grabbed;  ...or object that requires both hands to be lifted

				anchor = null;
				return false;
			}
			bool allowed = OnGrabbed?.Invoke(this, grabbedBy) ?? true;
			if (allowed)
			{
				GrabbedBy = grabbedBy;
				if (_grabAnchors.Length == 0)
				{
					anchor = this.transform;
					_currentAnchor = -1;
				}
				else
				{
					_currentAnchor = FindClosestAnchor();
					anchor = _grabAnchors[_currentAnchor].transform;
				}
			}
			else
			{
				anchor = null;
				_currentAnchor = -1;
			}
			return allowed;
		}



		private int FindClosestAnchor()
		{
			//  Find which anchor is closest to the Grabber
			int closest = -1;
			float bestScore = float.MinValue;
			
			var grabberPos = GrabbedBy.transform.position;
			var grabberRot = GrabbedBy.transform.rotation;

			for (int i = 0; i < _grabAnchors.Length; i++)
			{
				var anchor = _grabAnchors[i];

				// Calculate a score for the anchor, based on distance and orientation deltas
				float distScore = 1f - Mathf.Clamp01((anchor.transform.position - grabberPos).magnitude / MaxAnchorDistance);
				float oriScore;
				if (!OrientToHand)
				{
					oriScore = 1f;
				} else
				{
					oriScore = OrientationScore(anchor.transform.rotation, grabberRot);
				}

				float score = distScore * oriScore;
				if (score > bestScore)
				{
					bestScore = score;
					closest = i;
				}
			}
			return closest;
		}


		/// <summary>
		/// How similar are the specified Quaternions?  1 = perfect match, 0 = totally different
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		float OrientationScore(Quaternion a, Quaternion b)
		{
			// Find the angular delta, and convert from Quaternion to Angle/Axis
			var angDelta = a * Quaternion.Inverse(b);
			float angle;
			Vector3 axis;
			angDelta.ToAngleAxis(out angle, out axis);
			if (angle > 180f) angle -= 360f;
			return 1f - Mathf.Abs(angle / 180f);
		}


		internal virtual void Release(Grabber fromGrabber)
		{
			if (GrabbedBy == fromGrabber)
			{
				GrabbedBy = null;
				OnReleased?.Invoke(this, fromGrabber);
			}
		}

	}
}