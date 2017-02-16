using System;

namespace MovieConnections.Framework.Extensions {
    public static class StringExtensions {
        public static string RemoveWordFromString(this string str, string word) {
            var matchIndex = str.IndexOf(word, StringComparison.InvariantCulture);
            var wordLength = word.Length;
            var formerWord = str.Substring(0, matchIndex);
            var latterWord = str.Substring(matchIndex + wordLength);
            return formerWord + latterWord;
        }

        public static string AddSlashBetweenWords(this string formerWord, string latterWord, bool prefix = false) {
            return (prefix ? "/" : "") + formerWord + "/" + latterWord;
        }

        public static string WithoutQueryString(this string url) {
            if (string.IsNullOrEmpty(url))
                return string.Empty;

            var urlWithoutQueryString = url.Split('?')[0];

            return urlWithoutQueryString;
        }
    }
}