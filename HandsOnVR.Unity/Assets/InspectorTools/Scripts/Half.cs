using System;
using UnityEngine;

/// <summary>
/// <para>Half-precision (16-bit) floating point value.</para>
/// </summary> 
/// 
/// Author(s): 
/// - 10/5/2018		David Barlia
/// -
///
[Serializable]
public struct Half {
	[SerializeField] ushort _half;

	public Half(float f)
	{
		_half = Mathf.FloatToHalf(f);
	}

	public static implicit operator float(Half h)
	{
		return Mathf.HalfToFloat(h._half);
	}

	public static implicit operator Half(float f)
	{
		return new Half(f);
	}

	public static Half operator +(Half h1, Half h2)
	{
		return new Half(Mathf.HalfToFloat(h1._half) + Mathf.HalfToFloat(h2._half));
	}

	public static Half operator -(Half h1, Half h2)
	{
		return new Half(Mathf.HalfToFloat(h1._half) - Mathf.HalfToFloat(h2._half));
	}

	public static Half operator *(Half h1, Half h2)
	{
		return new Half(Mathf.HalfToFloat(h1._half) * Mathf.HalfToFloat(h2._half));
	}

	public static Half operator /(Half h1, Half h2)
	{
		return new Half(Mathf.HalfToFloat(h1._half) / Mathf.HalfToFloat(h2._half));
	}

	public override string ToString()
	{
		return Mathf.HalfToFloat(_half).ToString();
	}
	public string ToString(string format)
	{
		return Mathf.HalfToFloat(_half).ToString(format);
	}

	public static Half Parse(string value)
	{
		return new Half(float.Parse(value));
	}

	public byte[] GetBytes()
	{
		return BitConverter.GetBytes(_half);
	}

	public static Half FromBytes(byte[] bytes)
	{
		return new Half() { _half = BitConverter.ToUInt16(bytes, 0) };
	}
}
