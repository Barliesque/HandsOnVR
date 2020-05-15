using UnityEngine;

namespace HandsOnVR
{
	static public class Vector3Extensions
	{
		static public float GetAxis(this Vector3 vec3, Axis axis)
		{
			return axis == Axis.X ? vec3.x : (axis == Axis.Y ? vec3.y : vec3.z);
		}
	}
}