namespace TakeSword
{
    public static class StringExtensions
    {
        public static string? StripPrefix(this string text, string prefix)
        {
            if (text.StartsWith(prefix))
                return text.Substring(prefix.Length);
            else
                return null;
        }
    }
}
