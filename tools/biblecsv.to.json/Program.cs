using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibleCSVToJson
{
    class Program
    {
        static void Main(string[] args)
        {

            //CreateBooksDefaultTranslation();

            //CreateBooksIndex();

            Extract_CSV();

            Console.WriteLine("Press Any Key To Exit...");
            Console.ReadLine();
        }

        private static void CreateBooksIndex()
        {
            var config = new Configuration()
            {
                Delimiter = ",",
                HasHeaderRecord = true,
                IgnoreQuotes = false
            };

            var outdir = $"../../../../db";
            Directory.CreateDirectory(outdir);

            foreach (var file in Directory.GetFiles("../../submodules/bibledatabasecsv/language_files/SysDefs", "bibles_index.csv"))
            {
                Console.WriteLine($"Extract.Bibles.Index: {file} -> {outdir}");
                using (var reader = new StreamReader(file))
                using (var csv = new CsvReader(reader, config))
                {

                    var books = csv.GetRecords<BibleIndexCSV>().ToArray();

                    var jsonStr = JsonConvert.SerializeObject(books);
                    var targetFile = $"{outdir}/bibles.json";

                    Console.WriteLine($"Creating Bible Index: {targetFile }");
                    FileHelper.CreateFile(targetFile, jsonStr);
                }
            }
        }

        /// <summary>
        /// Extract CSV
        /// </summary>
        private static void Extract_CSV()
        {
            foreach (var file in Directory.GetFiles("../../submodules/bibledatabasecsv/db", "b_*.txt"))
            {
                var dir = Path.GetFileNameWithoutExtension(file.Replace("b_", ""));
                var outdir = $"../../../../db/{dir}";

                Console.WriteLine($"Extract: {file} -> {outdir}");
                CreateJson(file, outdir);
            }
        }

        /// <summary>
        /// Extract default book translation
        /// </summary>
        private static void CreateBooksDefaultTranslation()
        {
            var config = new Configuration()
            {
                Delimiter = "\t",
                HasHeaderRecord = false,
                IgnoreQuotes = true
            };

            var outdir = $"../../../../db/defaults/books";
            Directory.CreateDirectory(outdir);

            foreach (var file in Directory.GetFiles("../../submodules/bibledatabasecsv/language_files/SysDefs", "books_*.txt"))
            {
                int codePage = 0;
                var fileNameWithNoExt = Path.GetFileNameWithoutExtension(file).ToLower().Replace("books_", "");
                switch (fileNameWithNoExt)
                {
                    case "afrikaans":
                        codePage = 28592;
                        break;
                    case "danish":
                        codePage = 28605;
                        break;
                    case "french":
                    case "dutch":
                    case "portuguese":
                    case "swedish":
                    case "haitian":
                    case "hungarian":
                    case "italian":
                        codePage = 1252;
                        break;
                        
                    case "douay_rheims":
                        //codePage = Encoding.GetEncoding("UTF-8").CodePage;
                    
                    case "german":
                    case "b_modern_greek_utf8":
                    case "b_ukrainian_utf8":
                    case "b_albanian_utf8":
                    case "b_greeknt_1550_utf8":
                    case "b_greeknt_1894_utf8":
                    case "b_greeknt_ubs3_utf8":
                    case "b_greeknt_wh_utf8":
                    case "b_hungarian_utf8":
                    case "b_septuagint_utf8":
                        codePage = 65001;
                        break;
                    case "thai":
                        codePage = 874;
                        break;
                    case "bulgarian":
                        codePage = 1251;
                        break;
                    case "haitian-creole":
                        codePage = 1252;
                        break;
                    case "chinese":
                        codePage = 950;
                        break;
                    case "b_chinese_union_gb":
                        codePage = 936;
                        break;
                    case "russian":
                        codePage = 20866;
                        break;
                    case "korean":
                        codePage = 949;
                        break;
                }
                var sourceEncode = Encoding.GetEncoding(codePage);
                Console.WriteLine($"ExtractDefaultBooks.Traslation: {file} -> {outdir}");
                using (var reader = new StreamReader(file, sourceEncode))
                using (var csv = new CsvReader(reader, config))
                {
                    var books = csv.GetRecords<BookCSV>().Where(r => r.Id < 67).ToArray();

                    var booksJsonStr = JsonConvert.SerializeObject(books);
                    var lzwString = LZString.compressToBase64(booksJsonStr);
                    var targetLzFile = $"{outdir}/{Path.GetFileNameWithoutExtension(file).ToLower().Replace("books_", "")}.txt";
                    if (!File.Exists(targetLzFile))
                    {
                        Console.WriteLine($"Creating Default Books: {targetLzFile}");
                        FileHelper.CreateFile(targetLzFile, lzwString);
                        //var targetJsonFile = $"{outdir}/{Path.GetFileNameWithoutExtension(file).ToLower().Replace("books_", "")}.json";
                        //FileHelper.CreateFile(targetJsonFile, booksJsonStr);
                    }
                    else
                    {
                        Console.WriteLine($"Skipping Default Books, File.Exists: {targetLzFile}");
                    }
                }
            }
        }

        /// <summary>
        /// Create LzJson
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        private static void CreateJson(string source, string target)
        {
            var fileNameWithNoExt = Path.GetFileNameWithoutExtension(source);
            int codePage = 0;
            switch (fileNameWithNoExt)
            {
                case "b_bulgarian_utf8":
                case "b_chinese_union_trad_utf8":
                case "b_modern_greek_utf8":
                case "b_ukrainian_utf8":
                case "b_albanian_utf8":
                case "b_greeknt_1550_utf8":
                case "b_greeknt_1894_utf8":
                case "b_greeknt_ubs3_utf8":
                case "b_greeknt_wh_utf8":
                case "b_hungarian_utf8":
                case "b_septuagint_utf8":
                    codePage = 65001;
                    break;
                case "b_thai":
                    codePage = 874;
                    break;
                case "b_bulgarian":
                case "b_ukrainian":
                    codePage = 1251;
                    break;
                case "b_hebrew_ot_h_":
                    codePage = 1255;
                    break; 
                case "b_chinese_union_b5":
                    codePage = 950;
                    break;
                case "b_chinese_union_gb":
                    codePage = 936;
                    break;
                case "b_russian_koi8r":
                    codePage = 20866;
                    break;
                case "b_korean":
                    codePage = 949;
                    break;
            }

            var sourceEncode = Encoding.GetEncoding(codePage);
            var config = new Configuration()
            {
                Delimiter = "\t",
                HasHeaderRecord = false,
                IgnoreQuotes = true,
                //Encoding = sourceEncode,
            };

            Directory.CreateDirectory(target);

            using (var reader = new StreamReader(source, sourceEncode))
            using (var csv = new CsvReader(reader, config))
            {
                var recs = csv.GetRecords<BibleRowCSV>().ToArray();

                var targetEncode = Encoding.Unicode;

                Parallel.ForEach(recs.GroupBy(b => b.Book).Select(g => g.FirstOrDefault().Book), book =>
                {
                    Parallel.ForEach(recs.Where(b => b.Book == book).GroupBy(v => v.Chapter).Select(c => c.First().Chapter), chapter =>
                    {
                        var verses = recs.Where(b => b.Book == book && b.Chapter == chapter).OrderBy(r => r.Verse).Select(r => r.Text);
                        var first = verses.First();
                        //if (codePage > 0)
                        //{
                        //    // convert to utf-16
                        //    verses = verses.Select(v => targetEncode.GetString(Encoding.Convert(sourceEncode, targetEncode, sourceEncode.GetBytes(v))));
                        //}

                        var versesJsonStr = JsonConvert.SerializeObject(verses);
                        var versesLzw = LZString.compressToBase64(versesJsonStr);
                        //var versesDecompress = LZString.compressToBase64(versesLzw);
                        var targetLzFile = $"{target}/{book}_{chapter}.txt";
                        var targetJsonFile = $"{target}/{book}_{chapter}.json";

                        if (verses.Any())
                        {
                            if (!File.Exists(targetLzFile))
                            {
                                Console.WriteLine($"Extracting: {targetLzFile}");
                                FileHelper.CreateFile(targetLzFile, versesLzw);
                                //FileHelper.CreateFile(targetJsonFile, versesJsonStr);
                                //var decoded = LZString.decompressFromUTF16(File.ReadAllText(targetLzFile));
                            }
                            else
                            {
                                Console.WriteLine($"Skipping|File.Exists: {targetLzFile}");
                            }
                        }
                        else
                        {
                            if (File.Exists(targetLzFile))
                            {
                                Console.WriteLine($"Deleting.Empty.Verse: {targetLzFile}");
                                File.Delete(targetLzFile);
                            }
                            else
                            {
                                Console.WriteLine($"Skipping|No.Verses: {targetLzFile}");
                            }
                        }
                    });
                });
            }
        }
    }
}


