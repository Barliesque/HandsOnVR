using System;
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
		[SerializeField] private string _grabPose;
		public string GrabPose => _grabPose;

		private int _grabPoseID;
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
		[SerializeField] private string _proximityPose;
		public string ProximityPose => _proximityPose;

		private int _proximityPoseID;
		public int ProximityPoseID
		{
			get => _proximityPoseID;
			private set => _proximityPoseID = value;
		}


		[SerializeField] private bool _orientToHand = true;
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
		[SerializeField] private SecondGrabBehavior _secondGrabBehavior = SecondGrabBehavior.TransferToSecondGrab;
		public SecondGrabBehavior SecondGrab { get { return _secondGrabBehavior; } }


		public Grabber GrabbedBy { get; private set; }
		public Grabber GrabbedBySecond { get; private set; }

		/// <summary>
		/// If called from a listener of Grabbable.OnGrabbed, then the grab action will be cancelled.
		/// </summary>
		public void CancelGrab() => _grabCancelled = true;
		private bool _grabCancelled;

		/// <summary>
		/// At the moment this object is grabbed, this event is fired.  If false is returned, the grab will be rejected.
		/// </summary>
		public event GrabHandler OnGrabbed;
		public delegate void GrabHandler(Grabbable grabbed, Grabber grabbedBy); //TODO  Instead of expecting a returned bool, create a flag called "BlockGrab"

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

		public bool HasExternalForce { get; set; }


		private Transform _xform;
		private IGrabAnchor[] _grabAnchors;
		private int _currentAnchor = -1;

		// Maximum distance the hand may be from a hand anchor for that anchor to be grabbable
		public float MaxAnchorDistance => 0.25f;
		//TODO  This distance should be considered before Grabber determines which object is really the nearest... maybe


		public bool SupportsHand(Hand hand) => true;
		public bool MirrorForOtherHand => false;
		public GrabAnchor.Order GrabOrder => GrabAnchor.Order.FirstOrSecond;
		public bool OverrideGrabPose => false;
		public bool OverrideOrientToHand => false;
		public bool OverrideProximityPose => false;

		Grabbable IGrabAnchor.Grabbable => this;


		private void Awake()
		{
			GrabPoseID = Animator.StringToHash(_grabPose);
			ProximityPoseID = Animator.StringToHash(_proximityPose);

			_xform = GetComponent<Transform>();
			Body = GetComponent<Rigidbody>();
			
			_grabAnchors = GetComponentsInChildren<IGrabAnchor>();
			// The Grabbable is only used as a GrabAnchor if no dedicated GrabAnchors have been created
			if (_grabAnchors.Length > 1)
			{
				// Remove the Grabbable itself from the list of GrabAnchors
				var all = _grabAnchors;
				_grabAnchors = new IGrabAnchor[all.Length - 1];
				Array.Copy(all, 1, _grabAnchors, 0, _grabAnchors.Length);
			}
		}


		private void InitiateGrab(Grabber grabbedBy)
		{
			if (GrabbedBy)
			{
				// This object is already grabbed...
				if (_secondGrabBehavior == SecondGrabBehavior.NoSecondGrab)
				{
					// ...Reject second grab.
					_grabCancelled = true;
					return;
				}
				if (_secondGrabBehavior == SecondGrabBehavior.TransferToSecondGrab)
				{
					// ...Release the first and re-grab with the second.
					GrabbedBy.Release();
				}
			}

			_grabCancelled = false;
			OnGrabbed?.Invoke(this, grabbedBy);
			if (!_grabCancelled)
			{
				// Is this the first hand grabbing this object?
				if (GrabbedBy == null)
				{
					GrabbedBy = grabbedBy;
				}
				else
				{
					GrabbedBySecond = grabbedBy;
				}
			}
			else
			{
				Debug.Log($"{name}: Grab blocked by OnGrabbed callback");
			}
		}


		/// <summary>
		/// This object is being grabbed. Confirmation is returned whether grab is allowed.
		/// </summary>
		/// <param name="grabbedBy">The Grabber attempting to grab this object.</param>
		/// <param name="anchor">A reference to the IGrabAnchor that will be grabbed.</param>
		/// <returns>True is returned if grab is allowed, or false if it is not.</returns>
		[Obsolete("For internal use only.  Use Grabber.Grab() instead.")]
		internal bool TryGrab(Grabber grabbedBy, out IGrabAnchor anchor)
		{
			InitiateGrab(grabbedBy);
			if (!_grabCancelled)
			{
				// By what anchor will it be grabbed?
				if (_grabAnchors.Length == 0)
				{
					anchor = this;
					_currentAnchor = -1;
				}
				else
				{
					_currentAnchor = FindClosestAnchor(grabbedBy, GrabbedBySecond ? GrabbedBy.GrabbedAnchor : null);  //anchorAlreadyGrabbed
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
						}
						else
						{
							GrabbedBy = null;
						}

						return false;
					}
				}
				return true;
			}
			else
			{
				anchor = null;
				_currentAnchor = -1;
				return false;
			}
		}


		/// <summary>
		/// Find which anchor is closest to the Grabber, excluding an anchor that's already grabbed.  Returns index to _grabAnchors[]
		/// </summary>
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
		private float OrientationScore(Quaternion a, Quaternion b)
		{
			// Find the angular delta, and convert from Quaternion to Angle/Axis
			var angDelta = a * Quaternion.Inverse(b);
			float angle;
			Vector3 axis;
			angDelta.ToAngleAxis(out angle, out axis);
			if (angle > 180f) angle -= 360f;
			return 1f - Mathf.Abs(angle / 180f);
		}

		public void Release()
		{
			if (GrabbedBySecond) Release(GrabbedBySecond);
			if (GrabbedBy) Release(GrabbedBy);
		}

		public void Release(Grabber fromGrabber)
		{
			if (fromGrabber.IsGrabbing)
			{
				// Release was called by something other than the Grabber
				fromGrabber.Release();
				return;
			}
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


		[Obsolete("For internal use only.  Use Grabber.Grab() instead.")]
		public bool TryForceGrab(Grabber grabbedBy, out IGrabAnchor anchor) => ForceGrab(this, grabbedBy, out anchor);

		[Obsolete("For internal use only.  Use Grabber.Grab() instead.")]
		public bool ForceGrab(IGrabAnchor anchor, Grabber grabbedBy, out IGrabAnchor grabbedAnchor)
		{
			grabbedAnchor = anchor;
			bool anchorIsThis = (anchor == (IGrabAnchor)this);

			InitiateGrab(grabbedBy);
			if (_grabCancelled) return false;
			
			int index = int.MaxValue;
			if (anchorIsThis)
			{
				// Only a Grabbable with no anchors can be used as a IGrabAnchor
				if (!_grabAnchors.IsNullOrEmpty())
				{
					index = FindClosestAnchor(grabbedBy, GrabbedBy == this ? null : GrabbedBy.GrabbedAnchor);
					if (index >= 0)
					{
						grabbedAnchor = _grabAnchors[index];
					} else
					{
						Debug.Log($"None of {anchor}'s anchors could be grabbed.");
					}
				}
			}
			else
			{
				// Get index of specified anchor
				for (index = _grabAnchors.Length - 1; index >= 0; index--)
				{
					if (_grabAnchors[index] == anchor) break;
				}
				if (index < 0)
				{
					// Anchor not found!
					Debug.LogError($"GrabAnchor ({anchor.Grabbable.name}) specified does not belong to this Grabbable ({name})");
				}
			}

			if (index < 0)
			{
				// Unsuccessful grab
				if (GrabbedBySecond == grabbedBy)
				{
					GrabbedBySecond = null;
				}
				else
				{
					GrabbedBy = null;
				}
				return false;
			}

			return true;
		}

	}

}