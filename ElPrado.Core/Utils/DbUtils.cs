namespace ElPrado.Core.Utils
{
    public static class DbUtils
    {
        public static string? NormalizeText(string s)
        {
            return string.IsNullOrWhiteSpace(s) ? null : s.Trim();
        }
    }
}
