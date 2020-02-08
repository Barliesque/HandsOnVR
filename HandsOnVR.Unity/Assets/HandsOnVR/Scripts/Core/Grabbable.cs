using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HandsOnVR
{

	/// <summary>
	/// Make an object available to be picked up with the player's hands.  (See: Grabber.cs)
	/// </summary>
	[RequireComponent(typeof(Rigidbody))]
	public class Grabbable : MonoBehaviour, IGrabAnchor
	{

		[Tooltip("A bool parameter name found in the Animator components of the player's hands.  While this object is being grabbed, the specified parameter will be set to true.")]
		[SerializeField] string _grabPose;
		public string GrabPose => _grabPose;

		int _grabPoseID;
		public int GrabPoseID
		{
			get {
				// Check for override in current anchor
				if (_currentAnchor >= 0 && _grabAnchors[_currentAnchor].OverrideGrabPose)
				{
					return _grabAnchors[_currentAnchor].GrabPoseID;
				}
				else
				{
					return _grabPoseID;
				}
			}
			private set { _grabPoseID = value; }
		}


		[Tooltip("A bool parameter name found in the Animator components of the player's hands.  While the player's hand is near this object, the specified parameter will be set to true.")]
		[SerializeField] string _proximityPose;
		public string ProximityPose => _proximityPose;

		int _proximityPoseID;
		public int ProximityPoseID
		{
			get => _proximityPoseID;
			private set => _proximityPoseID = value;
		}


		[SerializeField] bool _orientToHand = true;
		public bool OrientToHand
		{
			get {
				// Check for override in current anchor
				if (_currentAnchor >= 0 && _grabAnchors[_currentAnchor].OverrideGrabPose)
				{
					return _grabAnchors[_currentAnchor].OrientToHand;
				}
				else
				{
					return _orientToHand;
				}
			}
		}

		public enum SecondGrabBehavior
		{
			NoSecondGrab, TransferToSecondGrab, AllowBothHands  //, RequireBothHands
		}
		[SerializeField] SecondGrabBehavior _secondGrabBehavior = SecondGrabBehavior.TransferToSecondGrab;
		public SecondGrabBehavior SecondGrab { get { return _secondGrabBehavior; } }


		public Grabber GrabbedBy { get; private set; }
		public Grabber GrabbedBySecond { get; private set; }


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

		/// <summary>
		/// Invoked when this object enters the trigger volume of the grabber.
		/// </summary>
		public event GrabberApproachHandler OnGrabberApproach;
		public delegate void GrabberApproachHandler(Grabbable grabbable, Grabber grabber);
		internal void GrabberApproach(Grabber grabber) => OnGrabberApproach?.Invoke(this, grabber);

		/// <summary>
		/// Invoked when this object exits the trigger volume of the grabber.
		/// </summary>
		public event GrabberDepartHandler OnGrabberDepart;
		public delegate void GrabberDepartHandler(Grabbable grabbable, Grabber grabber);
		internal void GrabberDepart(Grabber grabber) => OnGrabberDepart?.Invoke(this, grabber);

		public Rigidbody Body { get; private set; }

		Transform _xform;
		IGrabAnchor[] _grabAnchors;
		int _currentAnchor = -1;

		// Maximum distance the hand may be from a hand anchor for that anchor to be grabbable
		public float MaxAnchorDistance => 0.25f;
		//TODO  This distance should be considered before Grabber determines which object is really the nearest... maybe


		public bool SupportsHand(Hand hand) => true;
		public bool MirrorForOtherHand => false;
		public GrabAnchor.Order GrabOrder => GrabAnchor.Order.FirstOrSecond;
		public bool OverrideGrabPose => false;
		public bool OverrideOrientToHand => false;
		public bool OverrideProximityPose => false;


		virtual protected void Start()
		{
			GrabPoseID = Animator.StringToHash(_grabPose);
			ProximityPoseID = Animator.StringToHash(_proximityPose);

			Body = GetComponent<Rigidbody>();
			_grabAnchors = GetComponentsInChildren<GrabAnchor>();
			_xform = GetComponent<Transform>();
		}


		/// <summary>
		/// This object is being grabbed. Confirmation is returned whether grab is allowed.
		/// </summary>
		/// <param name="grabbedBy">The Grabber attempting to grab this object</param>
		/// <returns>True is returned if grab is allowed, or false if it is not.</returns>
		internal virtual bool TryGrab(Grabber grabbedBy, out IGrabAnchor anchor)
		{
			if (GrabbedBy != null)
			{
				// This object is already grabbed...
				if (_secondGrabBehavior == SecondGrabBehavior.NoSecondGrab)
				{
					// ...Reject second grab.
					anchor = null;
					return false;
				}
				if (_secondGrabBehavior == SecondGrabBehavior.TransferToSecondGrab)
				{
					// ...Release the first and re-grab with the second.
					GrabbedBy.Release();
				}
			}

			bool allowed = OnGrabbed?.Invoke(this, grabbedBy) ?? true;
			if (allowed)
			{
				IGrabAnchor anchorAlreadyGrabbed = null;

				// Is this the first hand grabbing this object?
				if (GrabbedBy == null)
				{
					GrabbedBy = grabbedBy;
				}
				else
				{
					GrabbedBySecond = grabbedBy;
					anchorAlreadyGrabbed = grabbedBy.GrabbedAnchor;
				}

				// By what anchor will it be grabbed?
				if (_grabAnchors.Length == 0)
				{
					anchor = this;
					_currentAnchor = -1;
				}
				else
				{
					_currentAnchor = FindClosestAnchor(grabbedBy, anchorAlreadyGrabbed);
					if (_currentAnchor >= 0)
					{
						anchor = _grabAnchors[_currentAnchor];
					}
					else
					{
						// No valid GrabAnchor available.
						anchor = null;
						Debug.Log($"{name}: No GrabAnchor available");

						if (GrabbedBySecond)
						{
							GrabbedBySecond = null;
						} else
						{
							GrabbedBy = null;
						}

						return false;
					}
				}
			}
			else
			{
				Debug.Log($"{name}: Grab not allowed by callback");
				anchor = null;
				_currentAnchor = -1;
			}
			return allowed;
		}



		private int FindClosestAnchor(Grabber grabbedBy, IGrabAnchor alreadyGrabbed)
		{
			//  Find which anchor is closest to the Grabber
			int closest = -1;
			float bestScore = float.MinValue;

			var grabbedByHand = grabbedBy.Controller.Hand;
			var grabberPos = grabbedBy.transform.position;
			var grabberRot = grabbedBy.transform.rotation;

			bool isSecond = (GrabbedBySecond != null);

			for (int i = 0; i < _grabAnchors.Length; i++)
			{
				var anchor = _grabAnchors[i];

				// Make sure the anchor is available to this Grabber
				if (!anchor.enabled) continue;
				if (anchor == alreadyGrabbed) continue;
				if (anchor.GrabOrder == GrabAnchor.Order.FirstOnly && isSecond) continue;
				if (anchor.GrabOrder == GrabAnchor.Order.SecondOnly && !isSecond) continue;
				if (!anchor.SupportsHand(grabbedByHand)) continue;

				// Calculate a score for the anchor, based on distance and orientation deltas
				float distScore = 1f - Mathf.Clamp01((anchor.GetPosition(grabbedByHand) - grabberPos).magnitude / MaxAnchorDistance);
				float oriScore;
				if (!OrientToHand)
				{
					oriScore = 1f;
				}
				else
				{
					oriScore = OrientationScore(anchor.GetRotation(grabbedByHand), grabberRot);
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
			if (GrabbedBySecond == fromGrabber)
			{
				// Second hand released only
				GrabbedBySecond = null;
			}
			else if (GrabbedBy == fromGrabber)
			{
				if (GrabbedBySecond)
				{
					// First hand released, so second is now first and only
					GrabbedBy = GrabbedBySecond;
					GrabbedBySecond = null;
				}
				else
				{
					// First (and only) hand released
					GrabbedBy = null;
					OnReleased?.Invoke(this, fromGrabber);
				}
			}
		}


		public Vector3 GetPosition(Hand hand) //TODO  Consider passing the hand controller instead.  Then the grabbable could return the hand's point of contact as the position.
		{
			return _xform.position;
		}

		public Quaternion GetRotation(Hand hand)
		{
			return _xform.rotation;
		}

		public Vector3 GetUp(Hand hand)
		{
			return _xform.up;
		}
	}
}