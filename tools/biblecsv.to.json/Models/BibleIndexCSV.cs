using Newtonsoft.Json;
using System;

namespace BibleCSVToJson
{
    /// <summary>
    /// bibles Index
    /// </summary>
    [Serializable]
    public class BibleIndexCsv
    {
        public string name { get; set; }
        public string dbpath { get; set; }
        [JsonIgnore]
        public string copyright { get; set; }

        public string defaultIndex { get; set; }

        [JsonIgnore]
        public string source { get; set; }

        /// <summary>
        /// minimum bible index
        /// </summary>
        public int? minBookIdx { get; set; }
    }
}
