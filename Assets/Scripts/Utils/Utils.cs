public static class Utils
{
    public static bool Between(int value, int minInclusive, int maxExclusive)
    {
        return minInclusive <= value && value < maxExclusive;
    }

    public static bool Between(float value, float min, float max)
    {
        return min <= value && value <= max;
    }
}