using UnityEngine;

namespace HandsOnVR
{
	abstract public class HandControllerBase : MonoBehaviour
	{
		abstract public bool IsConnected { get; }
		abstract public ButtonState AorX { get; protected set; }
		abstract public ButtonState BorY { get; protected set; }
		abstract public ButtonState Grip { get; protected set; }
		abstract public Hand Hand { get; }
		abstract public Vector3 Position { get; }
		abstract public Quaternion Rotation { get; }
		abstract public ButtonState ThumbRest { get; protected set; }
		abstract public ButtonState Trigger { get; protected set; }
	}
}