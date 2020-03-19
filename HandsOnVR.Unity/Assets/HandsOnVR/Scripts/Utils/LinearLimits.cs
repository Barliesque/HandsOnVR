using System;
using UnityEngine;

namespace HandsOnVR
{

	[Serializable]
	public struct LinearLimits
	{
		public float Low;
		public float High;

		public LinearLimits(float low, float high)
		{
			Low = low;
			High = high;
		}

		public bool IsInside(float value) => (Low < High) ? (value >= Low && value <= High) : (value <= Low && value >= High);
		public float Clamp(float value) => (Low < High) ? Mathf.Clamp(value, Low, High) : Mathf.Clamp(value, High, Low);
		public float Range => High - Low;
		public float Normalize(float value) => Mathf.InverseLerp(Low, High, value);
		public float Lerp(float value) => Mathf.Lerp(Low, High, value);
	}
}