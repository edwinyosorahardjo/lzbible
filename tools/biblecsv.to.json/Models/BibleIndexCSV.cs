using System;

namespace BibleCSVToJson
{
    /// <summary>
    /// bibles Index
    /// </summary>
    [Serializable]
    public class BibleIndexCSV
    {
        public string name { get; set; }
        public string dbpath { get; set; }
        public string copyright { get; set; }
        public string source { get; set; }
        public string books { get; set; }
    }
}
