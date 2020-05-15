using System;
using UnityEngine;

namespace HandsOnVR
{

	[Serializable]
	public struct AngularLimits
	{
		/// <summary>
		/// Start point of the angular range.  Must be -180 to +180 degrees.
		/// </summary>
		[Tooltip("Start point of the angular range.  Must be -180 to +180 degrees.")]
		[SerializeField] private float _start;
		public float Start
		{
			get => _start;
			set => _start = FixAngle(value);
		}
		
		/// <summary>
		/// End point of the angular range.  Must be -180 to +180 degrees.
		/// </summary>
		[Tooltip("End point of the angular range.  Must be -180 to +180 degrees.")]
		[SerializeField] private float _end;
		public float End
		{
			get => _end;
			set => _end = FixAngle(value);
		}

		/// <summary>
		/// Is the angular range specified in counter-clockwise order?  Angular values increase travelling counter-clockwise.  This option can be used to invert the angular range.
		/// </summary>
		[Tooltip("Is the angular range specified in counter-clockwise order?  Angular values increase travelling counter-clockwise.  This option can be used to invert the angular range.")]
		public bool CCW;

		public AngularLimits(float low, float high, bool ccw = false)
		{
			_start = Mathf.Repeat(low + 180f, 360f) - 180f;
			_end = Mathf.Repeat(high + 180f, 360f) - 180f;
			CCW = ccw;
		}

		public bool RangeIsWrapped => CCW ? (_start > _end) : (_end > _start);
		
		public bool IsInside(float value)
		{
			value = FixAngle(value);
			if (RangeIsWrapped)
			{
				return CCW ? (value >= _start || value <= _end) : (value >= _end || value <= _start);
			}
			return CCW ? (value <= _end && value >= _start) : (value <= _start && value >= _end);
		}

		public float Clamp(float value)
		{
			value = FixAngle(value);
			if (RangeIsWrapped)
			{
				return (value < 0f) ? Mathf.Clamp(value, -180f, CCW ? _start : _end) : Mathf.Clamp(value, CCW ? _end : _start, 180f);
			}
			return CCW ? Mathf.Clamp(value, _start, _end) : Mathf.Clamp(value, _end, _start);
		}

		public float Range => RangeIsWrapped ? (360f - Mathf.Abs(_end - _start)) : Mathf.Abs(_end - _start);

		public float Normalize(float value)
		{
			value = FixAngle(value);
			if (RangeIsWrapped)
			{
				if (CCW) _end += 360f;
				else _start += 360f;
				if (value < 0f) value += 360f;
			}
			return Mathf.InverseLerp(_start, _end, value);
		}

		public float Lerp(float t)
		{
			if (RangeIsWrapped)
			{
				if (CCW) _end += 360f;
				else _start += 360f;
				return FixAngle(Mathf.Lerp(_start, _end, t));
			}
			return Mathf.Lerp(_start, _end, t);
		}

		/// <summary>
		/// Wrap an angle to the range -180 to 180
		/// </summary>
		static public float FixAngle(float angle) => Mathf.Repeat(angle + 180f, 360f) - 180f;
		
		/// <summary>
		/// Wrap angle values to the range -180 to 180
		/// </summary>
		static public Vector3 FixAngles(Vector3 angles) => new Vector3(FixAngle(angles.x), FixAngle(angles.y), FixAngle(angles.z));

		override public string ToString() => $"[AngularLimits: Start={Start}° End={End}° CCW={CCW}]";
		
	}
}