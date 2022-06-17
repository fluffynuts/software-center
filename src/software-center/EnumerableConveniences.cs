namespace software_center
{
    public static class EnumerableConveniences
    {
        public static T At<T>(
            this T[] source,
            int position,
            T fallback = default
        )
        {
            return position >= source.Length
                ? fallback
                : source[position];
        }
    }
}