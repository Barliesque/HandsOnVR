using System.Collections.Generic;
using UnityEngine;


namespace Barliesque.VRGrab
{

	/// <summary>
	/// Component in hands that handles the mechanics of grabbing objects.
	/// </summary>
	public class Grabber : MonoBehaviour
	{
		[SerializeField] HandController _hand;
		[SerializeField] Animator _handSolid;
		[SerializeField] Animator _handGhost;
		[SerializeField] GameObject _handColliders;
		[SerializeField] Grabber _otherHand;
		[SerializeField] LayerMask _grabbableLayers = 0x7FFFFFFF;

		List<Grabbable> _couldGrab = new List<Grabbable>();
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
				if (grabbable && !_couldGrab.Contains(grabbable))
				{
					Debug.Log($"You could grab {grabbable.name}");
					_couldGrab.Add(grabbable);
				}
			}
		}


		private void OnTriggerExit(Collider other)
		{
			// Is this a Grabbable object in our list?
			var grabbable = other.GetComponentInParent<Grabbable>();
			if (grabbable != null && _couldGrab.Contains(grabbable))
			{
				Debug.Log($"You can't grab {grabbable.name}");
				_couldGrab.Remove(grabbable);
				if (_couldGrab.Count == 0 && _grabbed == null)
				{
					_handColliders?.SetActive(true);
				}
			}
		}


		private void Update()
		{
			if (_hand.Grip.Began && _couldGrab.Count > 0)
			{
				if (_couldGrab.Count == 1)
				{
					_grabbed = _couldGrab[0];
				}
				else
				{
					_grabbed = FindClosest();
				}

				if (_grabbed != null)
				{
					//TODO  Tell _grabbed that it's been grabbed -- grab may be cancelled via OnGrabbed callback

					_joint.GrabbedBody = _grabbed.Body;
					_handColliders?.SetActive(false);

					//TODO  Don't allow grabbing of an object that is already grabbed?  
					//TODO  ...Or maybe the second hand *must* use HandToObject locking!
					//TODO  ...In which case, if the first hand releases, the second hand may be upgraded to ObjectToHand locking

					Debug.Log($"You just grabbed {_grabbed.name}");
				}
			}

			else if (_grabbed != null && _hand.Grip.Ended)
			{
				Debug.Log($"You just released {_grabbed?.name}");
				_grabbed = null;
				_joint.GrabbedBody = null;
				if (_couldGrab.Count == 0 && _grabbed == null)
				{
					_handColliders?.SetActive(true);
				}
			}
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
					var other = hit.collider.GetComponentInParent<Grabbable>();
					if (other == couldGrab && hit.distance < closestDist)
					{
						// Hit reached intended target, and it's closer than any other tested
						closest = i;
						closestDist = hit.distance;
					}
				}
			}

			return (closest >= 0) ? _couldGrab[closest] : null;
		}


	}
}