using System.Collections.Generic;
using UnityEngine;


namespace HandsOnVR
{

	/// <summary>
	/// Component in hands that handles the mechanics of grabbing objects.  (See: Grabbable.cs)
	/// </summary>
	[RequireComponent(typeof(GrabAttacher), typeof(CapsuleCollider))]
	public class Grabber : MonoBehaviour
	{
		[SerializeField] private GameObject _controller;
		public IHandController Controller { get; private set; }

		[SerializeField] private Transform _focusPoint;
		[SerializeField] private Animator _handSolid;
		[SerializeField] private Animator _handGhost;
		[SerializeField] private SolidHandMover _solidHandMover;
		[SerializeField] private GameObject _handColliders;
		[SerializeField] private Grabber _otherHand;
		[SerializeField] private LayerMask _grabbableLayers = 0x7FFFFFFF;

		public Animator HandSolid => _handSolid;
		public Animator HandGhost => _handGhost;

		public delegate void GrabberEvent(Grabbable item);
		public event GrabberEvent OnGrabbableEnter;
		public event GrabberEvent OnGrabbableExit;
		public event GrabberEvent OnGrabBegin;
		public event GrabberEvent OnGrabEnd;

		public bool IsGrabbing => _grabbed;

		/// <summary>Colliders currently intersecting the trigger volume, and the Grabbable to which they belong</summary>
		private Dictionary<Collider, Grabbable> _grabbables = new Dictionary<Collider, Grabbable>();

		/// <summary>Grabbables currently intersecting the trigger volume</summary>
		private List<GrabbableStats> _canGrab = new List<GrabbableStats>();

		public GrabAttacher Attacher => _attacher;
		private GrabAttacher _attacher;
		
		
		private Grabbable _grabbed;
		private RaycastHit[] _hits = new RaycastHit[64];
		private CapsuleCollider _triggerVolume;

		private PoseTrigger _poseTrigger;
		private int _proximityPoseID;


		private struct GrabbableStats
		{
			/// <summary>A Grabbable in range of this Grabber</summary>
			public Grabbable grabbable;
			/// <summary>A count of how many of the Grabbable's colliders are within the trigger volume</summary>
			public int colliders;
			/// <summary>The shortest distance to a collider belonging to the Grabbable</summary>
			public float distance;

			public GrabbableStats IncrementColliders()
			{
				++colliders;
				return this;
			}

			public GrabbableStats DecrementColliders()
			{
				--colliders;
				return this;
			}

			public GrabbableStats ResetDistance()
			{
				distance = float.MaxValue;
				return this;
			}

			public GrabbableStats TryDistance(float value)
			{
				if (value < distance) distance = value;
				return this;
			}
		}


		public IGrabAnchor GrabbedAnchor { get { return _attacher.GrabbedAnchor; } }

		public float MaxRadius { get; private set; }

		private void Awake()
		{
			Controller = _controller ? _controller.GetComponent<IHandController>() : GetComponentInParent<IHandController>();
			_attacher = GetComponent<GrabAttacher>();
			_triggerVolume = GetComponent<CapsuleCollider>();
			MaxRadius = _triggerVolume.bounds.size.magnitude;
		}


		private void OnTriggerEnter(Collider other)
		{
			// Test LayerMask before proceeding
			if (_grabbableLayers.Contains(other))
			{
				if (other.isTrigger)
				{
					// We're not interested in pose triggers while grabbing an object, or already in another pose trigger
					if (_grabbed || _proximityPoseID != 0) return;

					// Is this a Pose Trigger?
					var trigger = other.GetComponent<PoseTrigger>();
					if (trigger)
					{
						if (trigger.SupportsHand(Controller.Hand))
						{
							SetProximityPose(trigger.ProximityPoseID, trigger);
						}
						return;
					}
				}

				// Is this a Grabbable object?
				var grabbable = other.GetComponentInParent<Grabbable>();
				if (grabbable && !_grabbables.ContainsKey(other))
				{
					// Maintain the list of currently grabbable objects
					_grabbables.Add(other, grabbable);
					bool found = false;
					for (int i = 0, len = _canGrab.Count; i < len; i++)
					{
						var item = _canGrab[i];
						if (item.grabbable == grabbable)
						{
							_canGrab[i] = item.IncrementColliders();
							found = true;
							break;
						}
					}
					if (!found)
					{
						_canGrab.Add(new GrabbableStats() { grabbable = grabbable, colliders = 1 });
						OnGrabbableEnter?.Invoke(grabbable);
						grabbable.GrabberApproach(this);

						if (!_grabbed && _proximityPoseID == 0)
						{
							SetProximityPose(grabbable.ProximityPoseID);
						}
					}
				}
			}
		}


		private void OnTriggerExit(Collider other)
		{
			if (other.isTrigger && _poseTrigger && _poseTrigger.Collider == other)
			{
				ClearProximityPose();
				return;
			}

			// Is this a Grabbable collider in our list?
			if (_grabbables.ContainsKey(other))
			{
				////? Debug.Log($"You can't grab {grabbable.name}");
				var grabbable = _grabbables[other];
				_grabbables.Remove(other);

				// Remove collider count from appropriate Grabbable
				for (int i = 0, len = _canGrab.Count; i < len; i++)
				{
					var item = _canGrab[i];
					if (item.grabbable == grabbable)
					{
						_canGrab[i] = item.DecrementColliders();

						if (item.colliders == 0)
						{
							_canGrab.RemoveAt(i);
							OnGrabbableExit?.Invoke(grabbable);
							grabbable.GrabberDepart(this);

							if (!_poseTrigger && _proximityPoseID != 0 && grabbable.ProximityPoseID == _proximityPoseID)
							{
								ClearProximityPose();
							}
							break;
						}
					}
				}
			}
		}


		private void Update()
		{
			if (Controller.Grip.Began)
			{
				// Start grabbing an object?
				if (Controller.Grip.Began && _grabbables.Count > 0)
				{
					SelectAnchorAndGrab();
				}
			}
			// Grab release?
			else if (_grabbed && Controller.Grip.Ended)
			{
				EndGrab();
			}

			if (_grabbed)
			{
				_attacher.DisableForce = _grabbed.HasExternalForce;
			}

			// After grab has been released, wait for hand to move away before reactivating colliders
			if (!_grabbed && _grabbables.Count == 0 && !_handColliders.activeSelf)
			{
				_handColliders.SetActive(true);
			}
		}


		private void SelectAnchorAndGrab()
		{
			if (_canGrab.Count > 1)
			{
				SortGrabbables();
			}

			for (int i = 0, len = _canGrab.Count; i < len; i++)
			{
				var item = _canGrab[i];
				if (item.distance > MaxRadius) return;

				Grabbable grabbed = item.grabbable;

				IGrabAnchor anchor;
#pragma warning disable 618  // Allow use of internal method Grabbable.TryGrab()
				if (grabbed.TryGrab(this, out anchor))
#pragma warning restore 618
				{
					GrabByAnchor(anchor);
					return;
				}
				// Grab was unsuccessful, so try the next closest anchor
				//Debug.Log($"COULD NOT GRAB {grabbed.name}  ...next!");
			}
		}

		private void GrabByAnchor(IGrabAnchor anchor)
		{
			Grabbable grabbed = anchor.Grabbable;
			_grabbed = grabbed;

			if (grabbed.GrabbedBySecond == this)
			{
				// The first hand to grab will control the object for both hands
				grabbed.GrabbedBy.SetSecondGrab(anchor, _attacher.Target);
			}
			else
			{
				// Control the object to follow the hand
				SetGrab(_grabbed.Body, anchor);
			}
			_solidHandMover.Anchor = anchor;
			_handColliders.SetActive(false);
			if (_grabbed.GrabPoseID != 0)
			{
				SetHandPose(_grabbed.GrabPoseID, true);
			}

			// Grab was successful, so we are done
			OnGrabBegin?.Invoke(grabbed);
			ClearProximityPose();
		}

		private void SetGrab(Rigidbody grabbedBody, IGrabAnchor anchor)
		{
			_attacher.SetGrab(grabbedBody, anchor, Controller.Hand);
		}

		private void SetSecondGrab(IGrabAnchor anchor, Transform target)
		{
			_attacher.SetSecondGrab(anchor, target);
		}

		private void EndGrab()
		{
			SetHandPose(_grabbed.GrabPoseID, false);
			if (_grabbed.GrabbedBySecond == this)
			{
				// This is the second hand being released.  Clear second grab from the first's joint
				_grabbed.GrabbedBy.SetSecondGrab(null, null);
			}
			else if (_grabbed.GrabbedBySecond != null)
			{
				// This is the first hand being released.  The second must now use its own joint to grab
				_grabbed.GrabbedBySecond.SetGrab(_grabbed.Body, _attacher.SecondAnchor);
			}
			var grabbed = _grabbed;
			_grabbed = null;
			grabbed.Release(this);
			SetGrab(null, null);
			SetSecondGrab(null, null);
			_solidHandMover.Anchor = null;
			OnGrabEnd?.Invoke(grabbed);
		}


		public void Release()
		{
			if (_grabbed != null) EndGrab();
		}


		/// <summary>
		/// Manually grab an object regardless of its distance from the hand.  False is returned if the specified object could not be grabbed.
		/// </summary>
		/// <param name="anchor">A GrabAnchor or Grabbable</param>
		/// <returns></returns>
		public bool Grab(IGrabAnchor anchor)
		{
			// Already grabbing another object?  Release it!
			if (_grabbed != null) EndGrab();

#pragma warning disable 618  // Allow use of Internal method ForceGrab
			if (anchor.TryForceGrab(this, out anchor))
#pragma warning restore 618
			{
				GrabByAnchor(anchor);
				return true;
			}
			return false;
		}
		 

		private void SortGrabbables()
		{
			// Reset distances of Grabbables for testing
			for (int i = 0, len = _canGrab.Count; i < len; i++)
			{
				_canGrab[i] = _canGrab[i].ResetDistance();
			}

			// Run through list of colliders to find the shortest distance to each Grabbable
			var here = _focusPoint.position;
			foreach (var item in _grabbables)
			{
				var collider = item.Key;

				// Evaluate distance by raycasting from here to the collider.
				var direction = collider.bounds.center - here;
				var maxDist = direction.magnitude;
				direction /= maxDist;

				int count = Physics.RaycastNonAlloc(here, direction, _hits, maxDist, _grabbableLayers);

				// Find nearest hit
				var hit = new RaycastHit() { distance = float.MaxValue };
				for (int h = 0; h < count; h++)
				{
					if (_hits[h].distance < hit.distance)
					{
						hit = _hits[h];
					}
				}

				// Did raycast reach its intended target?
				if (hit.collider == collider)
				{
					// Is it closer than any other tested for the given Grabbable?
					var grabbable = item.Value;
					var i = _canGrab.FindIndex((a) => a.grabbable == grabbable);
					_canGrab[i] = _canGrab[i].TryDistance(hit.distance);
				}
			}

			// Finally, sort the list by shortest distance
			_canGrab.Sort(CompareGrabbables);
		}

		private int CompareGrabbables(GrabbableStats x, GrabbableStats y)
		{
			return x.distance.CompareTo(y.distance);
		}


		private void SetProximityPose(int poseID, PoseTrigger trigger = null)
		{
			_proximityPoseID = poseID;
			_poseTrigger = trigger;
			SetHandPose(_proximityPoseID, true);
		}

		private void ClearProximityPose()
		{
			SetHandPose(_proximityPoseID, false);
			_proximityPoseID = 0;
			_poseTrigger = null;
		}


		private void SetHandPose(int id, bool active)
		{
			if (id == 0) return;
			_handGhost.SetBool(id, active);
			_handSolid.SetBool(id, active);
		}

	}
}
