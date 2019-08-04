using System.Collections.Generic;
using UnityEngine;


namespace Barliesque.VRGrab
{

	/// <summary>
	/// Component in hands that handles the mechanics of grabbing objects.  (See: Grabbable.cs)
	/// </summary>
	[RequireComponent(typeof(GrabJoint), typeof(CapsuleCollider))]
	public class Grabber : MonoBehaviour
	{
		[SerializeField] HandController _controller;
		public HandController Controller { get { return _controller; } }

		[SerializeField] Transform _focusPoint;
		[SerializeField] Animator _handSolid;
		[SerializeField] Animator _handGhost;
		[SerializeField] MatchTransform _solidHandMatcher;
		[SerializeField] GameObject _handColliders;
		[SerializeField] Grabber _otherHand;
		[SerializeField] LayerMask _grabbableLayers = 0x7FFFFFFF;

		public delegate void GrabberEvent(Grabbable item);
		public event GrabberEvent OnGrabbableEnter;
		public event GrabberEvent OnGrabbableExit;
		public event GrabberEvent OnGrabBegin;
		public event GrabberEvent OnGrabEnd;


		/// <summary>Colliders currently within the trigger volume, and the Grabbable to which they belong</summary>
		Dictionary<Collider, Grabbable> _inRange = new Dictionary<Collider, Grabbable>();

		/// <summary>Grabbables currently within the trigger volume</summary>
		List<GrabbableStats> _canGrab = new List<GrabbableStats>();

		struct GrabbableStats
		{
			/// <summary>A Grabbable in range of this Grabber</summary>
			public Grabbable grabbable;
			/// <summary>A count of how many of the Grabbable's colliders are within the trigger volume</summary>
			public int colliders;
			/// <summary>The shortest distance to a collider belonging to the Grabbable</summary>
			public float distance;

			public int IncrementColliders() { return ++colliders; }
			public int DecrementColliders() { return --colliders; }
			public void ResetDistance() { distance = float.MaxValue; }
			public void TryDistance(float value) {
				if (value < distance) distance = value;
			}
		}


		Grabbable _grabbed;
		RaycastHit[] _hits = new RaycastHit[3];
		GrabJoint _joint;
		CapsuleCollider _triggerVolume;


		public Transform GrabbedAnchor { get { return _joint.GrabbedAnchor; } }


		private void Awake()
		{
			_joint = GetComponent<GrabJoint>();
			_triggerVolume = GetComponent<CapsuleCollider>();
		}


		private void OnTriggerEnter(Collider other)
		{
			// Maintain a list of currently grabbable objects

			// Test LayerMask before proceeding
			if (_grabbableLayers.Contains(other))
			{
				// Is this a Grabbable object?
				var grabbable = other.GetComponentInParent<Grabbable>();
				if (grabbable && !_inRange.ContainsKey(other))
				{
					_inRange.Add(other, grabbable);
					bool found = false;
					for (int i = 0, len = _canGrab.Count; i < len; i++)
					{
						var item = _canGrab[i];
						if (item.grabbable == grabbable)
						{
							item.IncrementColliders();
							found = true;
							break;
						}
					}
					if (!found)
					{
						_canGrab.Add(new GrabbableStats() { grabbable = grabbable, colliders = 1 });
						OnGrabbableEnter?.Invoke(grabbable);
					}
				}
			}
		}


		private void OnTriggerExit(Collider other)
		{
			// Is this a Grabbable collider in our list?
			if (_inRange.ContainsKey(other))
			{
				////? Debug.Log($"You can't grab {grabbable.name}");
				var grabbable = _inRange[other];
				_inRange.Remove(other);

				if (_inRange.Count == 0)
				{
					// No colliders left?  Then there's nothing to grab...
					_canGrab.Clear();
				}
				else
				{
					// Remove collider count from appropriate Grabbable
					for (int i = 0, len = _canGrab.Count; i < len; i++)
					{
						var item = _canGrab[i];
						if (item.grabbable == grabbable)
						{
							if (item.DecrementColliders() == 0)
							{
								_canGrab.RemoveAt(i);
								OnGrabbableExit?.Invoke(grabbable);
								break;
							}
						}
					}
				}
			}
		}


		private void Update()
		{
			// Start grabbing an object?
			if (_controller.Grip.Began && _inRange.Count > 0)
			{
				BeginGrab();
			}

			// Grab release?
			else if (_grabbed != null && _controller.Grip.Ended)
			{
				EndGrab();
			}

			// After grab has been released, wait for hand to move away before reactivating colliders
			if (_grabbed == null && !_handColliders.activeSelf)
			{
				if (_inRange.Count == 0)
				{
					_handColliders.SetActive(true);
				}
			}

			if (_grabbed != null && _solidHandMatcher.Transition < 1f)
			{
				// Move the solid hand to the grabbed object
				_solidHandMatcher.Transition = Mathf.Clamp01(_solidHandMatcher.Transition + Time.unscaledDeltaTime * 2f);
			}

			if (_grabbed == null && _solidHandMatcher.Transition > 0f)
			{
				// Move the solid hand back to HandController
				_solidHandMatcher.Transition = Mathf.Clamp01(_solidHandMatcher.Transition - Time.unscaledDeltaTime * 2f);
				if (_solidHandMatcher.Transition == 0f)
				{
					_solidHandMatcher.SecondTarget = null;
				}
			}
		}


		void BeginGrab()
		{
			if (_canGrab.Count > 1)
			{
				SortGrabbables();
			}

			for (int i = 0, len = _canGrab.Count; i < len; i++)
			{
				Grabbable grabbed = _canGrab[i].grabbable;

				Transform anchor;
				if (grabbed.TryGrab(this, out anchor))
				{
					_grabbed = grabbed;

					if (grabbed.GrabbedBySecond == this)
					{
						// The first hand to grab will control the object for both hands
						grabbed.GrabbedBy.SetSecondGrab(anchor, _joint.Target);
					}
					else
					{
						// Control the object to follow the hand
						SetGrab(_grabbed.Body, anchor);
					}
					_solidHandMatcher.SecondTarget = anchor;
					_handColliders.SetActive(false);
					if (_grabbed.GrabPoseID != 0)
					{
						_handSolid.SetBool(_grabbed.GrabPoseID, true);
					}

					// Grab was successful, so we are done
					OnGrabBegin?.Invoke(grabbed);
					return;
				}
				// Grab was unsuccessful, so try the next closest
			}
		}

		void SetGrab(Rigidbody grabbedBody, Transform anchor)
		{
			_joint.SetGrab(grabbedBody, anchor);
		}

		void SetSecondGrab(Transform anchor, Transform target)
		{
			_joint.SetSecondGrab(anchor, target);
		}

		void EndGrab()
		{
			_handSolid.SetBool(_grabbed.GrabPoseID, false);
			if (_grabbed.GrabbedBySecond == this)
			{
				// This is the second hand being released.  Clear second grab from the first's joint
				_grabbed.GrabbedBy.SetSecondGrab(null, null);
			}
			else if (_grabbed.GrabbedBySecond != null)
			{
				// This is the first hand being released.  The second must now use its own joint to grab
				_grabbed.GrabbedBySecond.SetGrab(_grabbed.Body, _joint.SecondAnchor);
			}
			var grabbed = _grabbed;
			_grabbed.Release(this);
			_grabbed = null;
			SetGrab(null, null);
			SetSecondGrab(null, null);
			_solidHandMatcher.SecondTarget = _solidHandMatcher.transform;
			OnGrabEnd?.Invoke(grabbed);
		}


		public void Release()
		{
			if (_grabbed != null) EndGrab();
		}


		private void SortGrabbables()
		{
			// Reset distances of Grabbables for testing
			for (int i = 0, len = _canGrab.Count; i < len; i++)
			{
				_canGrab[i].ResetDistance();
			}

			// Run through list of colliders to find the shortest distance to each Grabbable
			var here = _focusPoint.position;
			foreach (var item in _inRange)
			{
				var collider = item.Key;

				// Evaluate distance by raycasting from here to the collider.
				var direction = collider.bounds.center - here;  //TODO  Try using collider.bounds.center
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
					var stats = _canGrab.Find((a) => a.grabbable == grabbable);
					stats.TryDistance(hit.distance);
				}
			}

			// Finally, sort the list by shortest distance
			_canGrab.Sort(CompareGrabbables);
		}

		private int CompareGrabbables(GrabbableStats x, GrabbableStats y)
		{
			return x.distance.CompareTo(y.distance);
		}
	}
}