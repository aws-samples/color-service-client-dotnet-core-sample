namespace ColorService.Utility
{
    public static class Extensions
    {
        public static bool IsBlank(this string str) 
            => string.IsNullOrWhiteSpace(str);

        public static string OrDefaultIfBlank(this string str, string defaultVal) 
            => str.IsBlank() ? defaultVal : str;
    }
}
