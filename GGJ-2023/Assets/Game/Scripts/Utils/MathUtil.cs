using UnityEngine;

public static class MathUtil
{
	public static int LoopedValue(int value, int minValue, int maxValue)
	{
		if (value > maxValue) return minValue;
		if (value < minValue) return maxValue;
		return value;
	}

	public static int LoopedValue(int value, Vector2 bounds)
	{
		if (value > bounds.y) return Mathf.RoundToInt(bounds.x);
		if (value < bounds.x) return Mathf.RoundToInt(bounds.y);
		return value;
	}

	public static float LoopedValue(float value, float minValue, float maxValue)
	{
		if (value > maxValue) return minValue;
		if (value < minValue) return maxValue;
		return value;
	}

	public static float LoopedValue(float value, Vector2 bounds)
	{
		if (value > bounds.y) return bounds.x;
		if (value < bounds.x) return bounds.y;
		return value;
	}
}
