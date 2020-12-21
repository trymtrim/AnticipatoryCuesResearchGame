using System;

public static class Util
{
	private static Random staticSeedRandom = new Random(1000);

	public static float Map(float value, float a1, float a2, float b1, float b2)
	{
		return b1 + (value - a1) * (b2 - b1) / (a2 - a1);
	}

	public static float RandomRangeWithStaticSeed(float min, float max)
	{
		return (float)(staticSeedRandom.NextDouble() * (max - min) + min);
	}
}
