using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BibleCSVToJson.Helper
{
    public static class Extensions
    {
        public static string ToBase64(this byte[] obj) => Convert.ToBase64String(obj);

        const string REGEX_CLEANER = @"^(\[|\]|¶|\s)*|(\[|\]|¶|\s)*$";
        public static string Sanitize(this string txt) => Regex.Replace(txt, REGEX_CLEANER, String.Empty, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Multiline);

        public static string ToJson(this Object obj, Formatting formatting = Formatting.None)
        {
            return JsonConvert.SerializeObject(obj, formatting, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }
}
}
