using UnityEngine;

namespace HandsOnVR
{
	public interface IHandController
	{
		bool IsConnected { get; }
		ButtonState AorX { get; }
		ButtonState BorY { get; }
		ButtonState Grip { get; }
		Hand Hand { get; }
		Vector3 Position { get; }
		Quaternion Rotation { get; }
		ButtonState Trigger { get; }
		T GetComponent<T>();
	}
}