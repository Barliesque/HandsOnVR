using System;
using UnityEngine;

namespace HandsOnVR
{
	public interface IGrabAnchor
	{
		bool enabled { get; }

		bool SupportsHand(Hand hand);
		bool MirrorForOtherHand { get; }
		GrabAnchor.Order GrabOrder { get; }

		Vector3 GetPosition(Hand hand);
		Quaternion GetRotation(Hand hand);
		Vector3 GetUp(Hand hand);

		bool OverrideGrabPose { get; }
		bool OverrideProximityPose { get; }
		bool OverrideOrientToHand { get; }

		int GrabPoseID { get; }
		int ProximityPoseID { get; }
		bool OrientToHand { get; }

		[Obsolete("For internal use only.  Use Grabber.Grab() instead.")]
		bool TryForceGrab(Grabber grabbedBy, out IGrabAnchor anchor);

		Grabbable Grabbable { get; }
	}
}