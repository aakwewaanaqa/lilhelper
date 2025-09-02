using System.Text.RegularExpressions;

namespace Lilhelper.Objs {
    public static class StringExts {

        private static readonly Regex camelPattern = new(@"( |_)([a-zA-Z])", RegexOptions.Compiled);

        public static string ToLowerCamelCase(this string self) {
            return camelPattern.Replace(self, m => m.Groups[2].Value.ToUpper());
        }
    }
}
