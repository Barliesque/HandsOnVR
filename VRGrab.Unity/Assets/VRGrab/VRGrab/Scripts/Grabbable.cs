using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 67  // Event is never used (yet)

namespace Barliesque.VRGrab
{

	[RequireComponent(typeof(Rigidbody))]
	public class Grabbable : MonoBehaviour
	{
		[Tooltip("Show should this object and the grabbing hand be connected?")]
		[SerializeField] GrabbableLockType _lockType;
		public GrabbableLockType LockType { get { return _lockType; } }

		[Tooltip("A bool parameter name found in the Animator components of the player's hands.  While this object is being grabbed, the specified parameter will be set to true.")]
		[SerializeField] string _grabPose;
		public int GrabPoseID { get; private set; }


		public HandController GrabbedBy {get; private set;}


		/// <summary>
		/// At the moment this object is grabbed, this event is fired.  If false is returned, the grab will be rejected.
		/// </summary>
		[Obsolete("Not implemented yet!")]
		public event GrabHandler OnGrabbed;
		public delegate bool GrabHandler(HandController grabbedBy);

		public Rigidbody Body { get; private set; }

		virtual protected void Start()
		{
			GrabPoseID = Animator.StringToHash(_grabPose);
			Body = GetComponent<Rigidbody>();
		}

	}


	public enum GrabbableLockType
	{
		ObjectToHand, HandToObject
	}


}