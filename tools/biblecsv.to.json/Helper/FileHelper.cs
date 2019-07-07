using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibleCSVToJson
{
    public class FileHelper
    {

        public static void CreateFile(string filename, string data)
        {
            Console.WriteLine($"Creating {filename}");
            var file = File.CreateText(filename);
            file.Write(data);
            file.Flush();
            file.Close();
        }

        /// <summary>
        /// default json/txt utf-8
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="data"></param>
        /// <param name="encoding"></param>
        public static void CreateFile(string filename, string data, Encoding encoding)
        {
            using (var sw = new BinaryWriter(File.Open(filename, FileMode.Create), Encoding.Unicode))
            {
                sw.Write(data);
                sw.Flush();
                sw.Close();
            }

        }
    }
}
