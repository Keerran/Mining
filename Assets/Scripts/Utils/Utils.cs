public static class Utils
{
    public static bool Between(int value, int minInclusive, int maxExclusive)
    {
        return minInclusive <= value && value < maxExclusive;
    }
}