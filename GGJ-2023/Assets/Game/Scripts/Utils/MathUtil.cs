public static class MathUtil
{
	public static int LoopedValue(int value, int minValue, int maxValue)
	{
		if (value > maxValue) return minValue;
		if (value < minValue) return maxValue;
		return value;
	}

	public static float LoopedValue(float value, float minValue, float maxValue)
	{
		if (value > maxValue) return minValue;
		if (value < minValue) return maxValue;
		return value;
	}
}
