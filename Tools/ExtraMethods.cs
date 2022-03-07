namespace InfoReader.Tools
{
    public static class ExtraMethods
    {
        public static string Format(this string s, params object[] args) => string.Format(s, args);
    }
}
