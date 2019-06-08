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
		[Tooltip("Show should this object and the grabbing hand be connected?")]
		[SerializeField] GrabbableLockType _lockType;
		public GrabbableLockType LockType { get { return _lockType; } }

		[Tooltip("A bool parameter name found in the Animator components of the player's hands.  While this object is being grabbed, the specified parameter will be set to true.")]
		[SerializeField] string _grabPose;
		public int GrabPoseID { get; private set; }

		public Grabber GrabbedBy {get; private set;}


		/// <summary>
		/// At the moment this object is grabbed, this event is fired.  If false is returned, the grab will be rejected.
		/// </summary>
		[Obsolete("Not implemented yet!")]
		public event GrabHandler OnGrabbed;
		public delegate bool GrabHandler(Grabbable grabbed, Grabber grabbedBy);

		public Rigidbody Body { get; private set; }

		virtual protected void Start()
		{
			GrabPoseID = Animator.StringToHash(_grabPose);
			Body = GetComponent<Rigidbody>();
		}

		//TODO  Consider adding option:  Allow one hand to grab object from the other?  Or allow *both* hands to grab simultaneously?

		/// <summary>
		/// This object is being grabbed. Confirmation is returned whether grab is allowed.
		/// </summary>
		/// <param name="grabbedBy"></param>
		/// <returns></returns>
		internal virtual bool TryGrab(Grabber grabbedBy)
		{
			bool allowed = OnGrabbed?.Invoke(this, grabbedBy) ?? true;
			if (allowed) {
				GrabbedBy = grabbedBy;
			}
			return allowed;
		}

	}


	public enum GrabbableLockType
	{
		ObjectToHand, HandToObject
	}


}