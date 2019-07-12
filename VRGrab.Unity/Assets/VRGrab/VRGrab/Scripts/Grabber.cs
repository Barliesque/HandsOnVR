using System.Collections.Generic;
using UnityEngine;


namespace Barliesque.VRGrab
{

	/// <summary>
	/// Component in hands that handles the mechanics of grabbing objects.  (See: Grabbable.cs)
	/// </summary>
	[RequireComponent(typeof(GrabJoint))]
	public class Grabber : MonoBehaviour
	{
		[SerializeField] HandController _hand;
		public HandController Hand { get { return _hand; } }

		[SerializeField] Animator _handSolid;
		[SerializeField] Animator _handGhost;
		[SerializeField] MatchTransform _solidHandMatcher;
		[SerializeField] GameObject _handColliders;
		[SerializeField] Grabber _otherHand;
		[SerializeField] LayerMask _grabbableLayers = 0x7FFFFFFF;

		List<Collider> _couldGrab = new List<Collider>();
		//TODO  Keep a Dictionary of Grabbables as well -- with a counter of the number of colliders

		Grabbable _grabbed;
		RaycastHit[] _hits = new RaycastHit[3];
		GrabJoint _joint;

		public Transform GrabbedAnchor { get { return _joint.GrabbedAnchor; } }


		private void Awake()
		{
			_joint = GetComponent<GrabJoint>();
		}

		private void OnTriggerEnter(Collider other)
		{
			// Maintain a list of currently grabbable objects

			// Test LayerMask before proceeding
			var layer = 1 << other.gameObject.layer;
			if ((_grabbableLayers & layer) == layer)
			{
				// Is this a Grabbable object?
				var grabbable = other.GetComponentInParent<Grabbable>();
				if (grabbable && !_couldGrab.Contains(other))
				{
					//Debug.Log($"You could grab {grabbable.name}");
					_couldGrab.Add(other);
					//TODO  Add event: OnGrabbableEnter
					//TODO  Add a small haptic click (as long as we're not grabbing something right now) -- do this in a separate component using above event
				}
			}
		}


		private void OnTriggerExit(Collider other)
		{
			// Is this a Grabbable collider in our list?
			if (_couldGrab.Contains(other))
			{
				//Debug.Log($"You can't grab {grabbable.name}");
				_couldGrab.Remove(other);

				//TODO  Add event: OnGrabbableExit

			}
		}


		private void Update()
		{
			// Start grabbing an object?
			if (_hand.Grip.Began && _couldGrab.Count > 0)
			{
				Grabbable grabbed;
				if (_couldGrab.Count == 1)
				{
					// Only one object to grab
					grabbed = _couldGrab[0].GetComponentInParent<Grabbable>();
				}
				else
				{
					// Find the closest object to grab
					grabbed = FindClosest();
				}

				if (grabbed != null)
				{
					BeginGrab(grabbed);
				}
			}

			// Grab release?
			else if (_grabbed != null && _hand.Grip.Ended)
			{
				EndGrab();
			}

			// After grab has been released, wait for hand to move away before reactivating colliders
			if (_grabbed == null && !_handColliders.activeSelf)
			{
				if (_couldGrab.Count == 0)
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


		void BeginGrab(Grabbable grabbed)
		{
			//TODO  Add event: OnBeginGrab
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
			}
		}

		void SetGrab(Rigidbody grabbedBody, Transform anchor)
		{
			_joint.GrabbedBody = grabbedBody;
			_joint.GrabbedAnchor = anchor;
		}

		void SetSecondGrab(Transform anchor, Transform target)
		{
			_joint.SecondAnchor = anchor;
			_joint.SecondTarget = target;
		}

		void EndGrab()
		{
			//TODO  Add event: OnEndGrab
			//Debug.Log($"You just released {_grabbed?.name}");
			_handSolid.SetBool(_grabbed.GrabPoseID, false);
			if (_grabbed.GrabbedBySecond == this)
			{
				// This is the second hand being released.  Clear second grab from the first's joint
				_grabbed.GrabbedBy.SetSecondGrab(null, null);
			} else if (_grabbed.GrabbedBySecond != null)
			{
				// This is the first hand being released.  The second must now use its own joint to grab
				_grabbed.GrabbedBySecond.SetGrab(_grabbed.Body, _joint.SecondAnchor);
			}
			_grabbed.Release(this);
			_grabbed = null;
			SetGrab(null, null);
			SetSecondGrab(null, null);
			_solidHandMatcher.SecondTarget = _solidHandMatcher.transform;
		}


		public void Release()
		{
			if (_grabbed != null) EndGrab();
		}


		private Grabbable FindClosest()
		{
			// Grab whichever is closest
			int closest = -1;
			float closestDist = float.MaxValue;
			var here = transform.position;
			for (int i = 0, len = _couldGrab.Count; i < len; i++)
			{
				// Evaluate distance by raycasting from here to the grabbable.
				var couldGrab = _couldGrab[i];
				var direction = couldGrab.transform.position - here;
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

				if (hit.collider != null)
				{
					if (hit.collider == couldGrab && hit.distance < closestDist)
					{
						// Hit reached intended target, and it's closer than any other tested
						closest = i;
						closestDist = hit.distance;
					}
				}
			}

			if (closest >= 0)
			{
				return _couldGrab[closest].GetComponentInParent<Grabbable>();
			}

			Debug.LogError($"Couldn't find closest Grabbable out of {_couldGrab.Count} possible!");
			return null;
		}


	}
}