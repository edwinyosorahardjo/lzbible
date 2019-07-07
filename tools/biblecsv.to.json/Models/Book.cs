using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace BibleCSVToJson
{
    /// <summary>
    /// Json Model for BOOKS_language.txt
    /// </summary>
    [Serializable]
    public class Book
    {
        [JsonProperty("abbr")]
        public string Abbreviation { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("ch")]
        public int Chapters { get; set; }
    }
}
