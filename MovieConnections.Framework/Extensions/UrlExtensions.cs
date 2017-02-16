
namespace MovieConnections.Framework.Extensions
{
    public static class UrlExtensions {
        public static string SlugifyUrl(this string stringUrl) {
            if(string.IsNullOrEmpty(stringUrl))
                return string.Empty;
            return string.Empty;
        }
        public static string SlashAdder(this string url, bool prefix = true, bool suffix = true) {
            if (string.IsNullOrEmpty(url))
                return string.Empty;

            if (prefix)
                url = "/" + url;

            if (suffix)
                url = url + "/";
            return url;
        }
    }
}
