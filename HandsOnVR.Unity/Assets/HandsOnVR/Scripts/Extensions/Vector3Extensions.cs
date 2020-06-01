using System.Runtime.CompilerServices;
using UnityEngine;

namespace HandsOnVR
{
	static public class Vector3Extensions
	{
		static public float GetAxis(this Vector3 vec3, Axis axis)
		{
			return axis == Axis.X ? vec3.x : (axis == Axis.Y ? vec3.y : vec3.z);
		}

		static public Vector3 SetAxis(this Vector3 vec3, Axis axis, float value)
		{
			if (axis == Axis.X) vec3.x = value;
			else if (axis == Axis.Y) vec3.y = value;
			else vec3.z = value;
			return vec3;
		}
		
	}
}