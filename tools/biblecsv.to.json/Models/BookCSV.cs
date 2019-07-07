using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace BibleCSVToJson
{
    /// <summary>
    /// Bible Book translation from BOOKS_language.txt
    /// </summary>
    [Serializable]
    public class BookCSV
    {
        [JsonIgnore]
        public int Id { get; set; }

        [JsonProperty("abbr")]
        public string Abbreviation { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

    }
}
