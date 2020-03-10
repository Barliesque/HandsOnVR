using System;

namespace HandsOnVR
{
	public static class ArrayExtensions
	{
		public static bool IsNullOrEmpty(this Array array) => (array == null || array.Length == 0);
	}
}