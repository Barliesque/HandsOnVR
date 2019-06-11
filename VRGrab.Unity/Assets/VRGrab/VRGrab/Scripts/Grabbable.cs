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
		//[Tooltip("How should this object and the grabbing hand be connected?")]
		//[SerializeField] GrabbableLockType _lockType;
		//public GrabbableLockType LockType { get { return _lockType; } }

		[Tooltip("A bool parameter name found in the Animator components of the player's hands.  While this object is being grabbed, the specified parameter will be set to true.")]
		[SerializeField] string _grabPose;
		public int GrabPoseID { get; private set; } // TODO Check for override in current anchor

		[SerializeField] bool _orientToHand = true;
		public bool OrientToHand { get { return _orientToHand; } } // TODO Check for override in current anchor

		public Grabber GrabbedBy {get; private set;}


		/// <summary>
		/// At the moment this object is grabbed, this event is fired.  If false is returned, the grab will be rejected.
		/// </summary>
		[Obsolete("Not implemented yet!")]
		public event GrabHandler OnGrabbed;
		public delegate bool GrabHandler(Grabbable grabbed, Grabber grabbedBy);

		public Rigidbody Body { get; private set; }

		GrabAnchor[] _grabAnchors;
		int _currentAnchor = -1;


		virtual protected void Start()
		{
			GrabPoseID = Animator.StringToHash(_grabPose);
			Body = GetComponent<Rigidbody>();
			_grabAnchors = GetComponentsInChildren<GrabAnchor>();
		}

		//TODO  Find all Child GrabAnchors.  When grab is initiated, pass to Grabber the transform of the nearest GrabAnchor to the Grabber's position (or the Grabber's transform if there are no GrabAnchors)
		//TODO  Consider adding option:  Allow one hand to grab object from the other?  Or allow *both* hands to grab simultaneously? ...or *require* both hands

		/// <summary>
		/// This object is being grabbed. Confirmation is returned whether grab is allowed.
		/// </summary>
		/// <param name="grabbedBy"></param>
		/// <returns></returns>
		internal virtual bool TryGrab(Grabber grabbedBy, out Transform anchor)
		{
			bool allowed = OnGrabbed?.Invoke(this, grabbedBy) ?? true;
			if (allowed) {
				GrabbedBy = grabbedBy;
				if (_grabAnchors.Length == 0)
				{
					anchor = this.transform;
					_currentAnchor = -1;
				}
				else
				{
					//TODO  find which anchor is closest to the Grabber
					anchor = _grabAnchors[0].transform;
					_currentAnchor = 0;
				}
			} else
			{
				anchor = null;
				_currentAnchor = -1;
			}
			return allowed;
		}

	}


	//public enum GrabbableLockType
	//{
	//	ObjectToHand, HandToObject
	//}


}