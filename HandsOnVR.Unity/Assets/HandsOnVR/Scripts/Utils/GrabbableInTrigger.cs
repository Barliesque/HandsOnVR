using System.Collections.Generic;
using UnityEngine;

namespace HandsOnVR
{

	/// <summary>
	/// A class to manage the entry and exit of colliders belonging to a Grabbable object.
	/// </summary>
	public class GrabbableInTrigger
	{

		Dictionary<Grabbable, int> _colliderCount = new Dictionary<Grabbable, int>();
		Dictionary<Collider, Grabbable> _colliders = new Dictionary<Collider, Grabbable>();


		/// <summary>
		/// To be called from OnTriggerEnter().  If a Grabbable has just entered the trigger, it will be returned.  Otherwise, null is returned.
		/// </summary>
		/// <param name="other">The other collider, provided by OnTriggerEnter()</param>
		/// <returns>When a Grabbable enters the trigger, it will be returned.  Otherwise, null is returned.</returns>
		public Grabbable Enter(Collider other)
		{
			var grabbable = other.GetComponentInParent<Grabbable>();
			if (!grabbable) return null;

			_colliders.Add(other, grabbable);

			if (_colliderCount.ContainsKey(grabbable))
			{
				_colliderCount[grabbable]++;
				return null;
			}
			else
			{
				_colliderCount.Add(grabbable, 1);
				return grabbable;
			}
		}


		/// <summary>
		/// To be called from OnTriggerExit().  If a Grabbable has completely exited the trigger, it will be returned.  Otherwise, null is returned.
		/// </summary>
		/// <param name="other">The other collider, provided by OnTriggerExit()</param>
		/// <returns>If a Grabbable has completely exited the trigger, it will be returned.   Otherwise, null is returned.</returns>
		public Grabbable Exit(Collider other)
		{
			if (!_colliders.ContainsKey(other)) return null;
			var grabbable = _colliders[other];
			_colliders.Remove(other);
			if (--_colliderCount[grabbable] == 0)
			{
				_colliderCount.Remove(grabbable);
				return grabbable;
			}
			return null;
		}

		public void Clear()
		{
			_colliderCount.Clear();
			_colliders.Clear();
		}

	}
}