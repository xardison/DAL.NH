namespace DAL.NH.Extensions
{
    internal static class StringExtension
    {
        public static string ReplaceStr(this string text, string oldValue, string newValue)
        {
            return text == null ? null : text.Replace(oldValue, newValue);
        }
    }
}