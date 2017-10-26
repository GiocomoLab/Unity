using UnityEngine;
using System.Collections;

namespace RWDTools
{
	public static class ExtensionMethods
	{
		public static float ToAngleDeg(this Vector2 vector)
		{
			return Mathf.Atan2(vector.x, vector.y) * Mathf.Rad2Deg;
		}

		public static Vector2 DegToVector(this float angle)
		{
			return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
		}

		public static bool isPowerOfTwo(this int x)
		{
			return ((x != 0) && ((x & (~x + 1)) == x));
		}
	}
}
