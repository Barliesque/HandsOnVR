using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandsOnVR
{
	static public class TransformExtensions
	{

		/// <summary>
		/// Transform a local rotation to world space. 
		/// </summary>
		/// <param name="xform"></param>
		/// <param name="local">A rotation in the local space of this transform.</param>
		/// <returns></returns>
		static public Quaternion TransformRotation(this Transform xform, Quaternion local)
		{
			return xform.rotation * local;
		}
		
		/// <summary>
		/// Transform a local rotation to world space. 
		/// </summary>
		/// <param name="xform"></param>
		/// <param name="local">A rotation in the local space of this transform.</param>
		/// <returns></returns>
		static public Vector3 TransformRotation(this Transform xform, Vector3 local)
		{
			return (xform.rotation * Quaternion.Euler(local)).eulerAngles;
		}

		
		/// <summary>
		/// Transform a world space rotation to local space.
		/// </summary>
		/// <param name="xform"></param>
		/// <param name="world">A rotation in world space.</param>
		/// <returns></returns>
		static public Quaternion InverseTransformRotation(this Transform xform, Quaternion world)
		{
			return Quaternion.Inverse(xform.rotation) * world;
		}

		/// <summary>
		/// Transform a world space rotation to local space.
		/// </summary>
		/// <param name="xform"></param>
		/// <param name="world">A rotation in world space.</param>
		/// <returns></returns>
		static public Vector3 InverseTransformRotation(this Transform xform, Vector3 world)
		{
			return (Quaternion.Inverse(xform.rotation) * Quaternion.Euler(world)).eulerAngles;
		}
		
		
	}
}