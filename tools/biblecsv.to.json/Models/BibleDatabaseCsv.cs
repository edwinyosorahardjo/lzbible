using CsvHelper.Configuration.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibleCSVToJson
{
    /// <summary>
    /// csv row model for bibledatabase.org bibles
    /// </summary>
    [Serializable]
    public class BibleDatabaseCsv
    {
        [Index(0)]
        public int Id { get; set; }
        [Index(1)]
        public int Book { get; set; }
        [Index(2)]
        public int Chapter { get; set; }
        [Index(3)]
        public int Verse { get; set; }
        [Index(4)]
        public string Text { get; set; }
    }

    /// <summary>
    /// csv row model for bibledatabase.org bibles
    /// </summary>
    [Serializable]
    public class BibleRowCsv
    {
        [Index(0)]
        public int Book { get; set; }
        [Index(1)]
        public int Chapter { get; set; }
        [Index(2)]
        public int Verse { get; set; }
        [Index(3)]
        public string Text { get; set; }
    }
}
