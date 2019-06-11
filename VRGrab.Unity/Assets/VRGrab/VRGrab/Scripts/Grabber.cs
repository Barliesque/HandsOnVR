using System.Collections.Generic;
using UnityEngine;


namespace Barliesque.VRGrab
{

	/// <summary>
	/// Component in hands that handles the mechanics of grabbing objects.  (See: Grabbable.cs)
	/// </summary>
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
		Grabbable _grabbed;
		RaycastHit[] _hits = new RaycastHit[3];
		GrabJoint _joint;


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
					//TODO  Add a small haptic click (as long as we're not grabbing something right now)
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
			}
		}


		private void Update()
		{
			if (_hand.Grip.Began && _couldGrab.Count > 0)
			{
				Grabbable grabbed;
				if (_couldGrab.Count == 1)
				{
					grabbed = _couldGrab[0].GetComponentInParent<Grabbable>();
				}
				else
				{
					grabbed = FindClosest();
				}

				if (grabbed != null)
				{
					BeginGrab(grabbed);
				}
			}

			else if (_grabbed != null && _hand.Grip.Ended)
			{
				EndGrab();
			}

			if (_grabbed == null && !_handColliders.activeSelf)
			{
				if (_couldGrab.Count == 0)
				{
					_handColliders.SetActive(true);
				}
			}

			if (_grabbed != null && _solidHandMatcher.Transition < 1f)
			{
				_solidHandMatcher.Transition = Mathf.Clamp01(_solidHandMatcher.Transition + Time.unscaledDeltaTime * 2f);
			}
			if (_grabbed == null && _solidHandMatcher.Transition > 0f)
			{
				_solidHandMatcher.Transition = Mathf.Clamp01(_solidHandMatcher.Transition - Time.unscaledDeltaTime * 2f);
				if (_solidHandMatcher.Transition == 0f)
				{
					_solidHandMatcher.SecondTarget = null;
				}
			}
		}


		void BeginGrab(Grabbable grabbed)
		{
			Transform anchor;
			if (grabbed.TryGrab(this, out anchor))
			{
				_grabbed = grabbed;
				_joint.GrabbedBody = _grabbed.Body;
				_joint.GrabbedAnchor = anchor;
				_solidHandMatcher.SecondTarget = anchor;
				_handColliders.SetActive(false);
				_handSolid.SetBool(_grabbed.GrabPoseID, true);
			}
		}


		void EndGrab()
		{
			//Debug.Log($"You just released {_grabbed?.name}");
			_handSolid.SetBool(_grabbed.GrabPoseID, false);
			_grabbed = null;
			_joint.GrabbedBody = null;
			_solidHandMatcher.SecondTarget = _solidHandMatcher.transform;
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