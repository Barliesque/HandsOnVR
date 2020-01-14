using UnityEngine;

namespace HandsOnVR
{
	public interface IGrabAnchor
	{
		bool enabled { get; }

		bool SupportsHand(Hand hand);
		bool MirrorForOtherHand { get; }

		GrabAnchor.Order GrabOrder { get; }
		int GrabPoseID { get; }
		bool OrientToHand { get; }

		Vector3 GetPosition(Hand hand);
		Quaternion GetRotation(Hand hand);
		Vector3 GetUp(Hand hand);

		bool OverridePose { get; }
		bool OverrideOrientToHand { get; }

	}
}