namespace Exceptionless.WebHook.DingTalk.Utilitys
{
    public class StringUtilitys
    {
        public static string ToCamelCase(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            var first = str.Substring(0, 1);
            var last = str.Substring(1);
            return first.ToLower() + last;
        }
    }
}