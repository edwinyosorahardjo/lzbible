using BibleCSVToJson.Helper;
using CsvHelper;
using CsvHelper.Configuration;
using ICSharpCode.SharpZipLib.BZip2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace BibleCSVToJson
{
    class Program
    {

        static void Main(string[] args)
        {

            CreateBooksDefaultTranslation();


            // Test Sanitizer
            //var testText = " ¶ []   　 　 Et Dieu dit: sinetgubg                 z                 ";
            //var cleanedText = testText.Sanitize();
            //Console.WriteLine(cleanedText);
            //return;
            //ExtractBibleCsv("niv_fixed.csv"); //"b_*.txt"
            ExtractBibleCsv<BibleRowCsv>("niv_fixed.csv"); //"b_*.txt"

            //CreateBiblesList();

            //AddBibleList();

            Console.WriteLine("Press Any Key To Exit...");
            Console.ReadLine();
        }

        private static void AddBibleList()
        {
            var outdir = $"../../../../db";
            Directory.CreateDirectory(outdir);
            var str = File.ReadAllText($"{outdir}/bibles.json");

            var books = str.FromJson<List<BibleIndexCsv>>();
            books.Add(new BibleIndexCsv
            {
                copyright = "my-bible-study.appspot.com",
                dbpath = "niv",
                name = "New International Version (NIV)",
                defaultIndex = "english"

            });
            var jsonStr = books.ToJson();

            Console.WriteLine($"Creating Bible Index:");
            CreateJsonFile(overrideFile: true, targetFile: $"{outdir}/bibles.json", jsonStr: jsonStr);
            CreateLzwFile(overrideFile: true, targetFile: $"{outdir}/bibles.txt", jsonStr: jsonStr);
            CreateBZip2File(overrideFile: true, targetFile: $"{outdir}/bibles.bz2", jsonStr: jsonStr);


        }


        /// <summary>
        /// Create List Of Available Bibles
        /// </summary>
        private static void CreateBiblesList()
        {
            var config = new Configuration()
            {
                Delimiter = ",",
                HasHeaderRecord = true,
                IgnoreQuotes = false,
                HeaderValidated = null,
                MissingFieldFound = null
            };

            var outdir = $"../../../../db";
            Directory.CreateDirectory(outdir);

            foreach (var file in Directory.GetFiles("../../submodules/bibledatabasecsv/language_files/SysDefs", "bible_index.csv"))
            {
                Console.WriteLine($"Extract.Bibles.Index: {file} -> {outdir}");
                using (var reader = new StreamReader(file))
                using (var csv = new CsvReader(reader, config))
                {
                    var books = csv.GetRecords<BibleIndexCsv>().ToArray();

                    var bz2dir = $"../../../../db";
                    var dirs = Directory.GetDirectories(bz2dir);
                    foreach (var dir in dirs)
                    {
                        var dbDir = $"{dir}/bz2";
                        if (Directory.Exists(dbDir))
                        {
                            var bookfile = Directory.GetFiles(dbDir, "*.bz2").OrderBy(s => s).Select(s => s).FirstOrDefault();
                            var minBookIdx = int.Parse(Path.GetFileNameWithoutExtension(bookfile));
                            if (minBookIdx > 1)
                            {
                                var book = books.FirstOrDefault(b => b.dbpath == Path.GetFileNameWithoutExtension(dir));
                                book.minBookIdx = minBookIdx;
                            }
                        }
                    }

                    var jsonStr = books.ToJson();

                    Console.WriteLine($"Creating Bible Index:");
                    CreateJsonFile(overrideFile: true, targetFile: $"{outdir}/bibles.json", jsonStr: jsonStr);
                    CreateLzwFile(overrideFile: true, targetFile: $"{outdir}/bibles.txt", jsonStr: jsonStr);
                    CreateBZip2File(overrideFile: true, targetFile: $"{outdir}/bibles.bz2", jsonStr: jsonStr);
                }
            }
        }

        /// <summary>
        /// Find all bibledatabase.org files and extract each files
        /// </summary>
        private static void ExtractBibleCsv<T>(string fileParam = "b_*.txt")
        {
            var files = Directory.GetFiles("../../submodules/bibledatabasecsv/db", fileParam);
            //Parallel.ForEach(files, file =>
            foreach (var file in files)
            {
                var dir = Path.GetFileNameWithoutExtension(file.Replace("b_", ""));
                var outdir = $"../../../../db/{dir}";

                Console.WriteLine($"Extract.Bible.Csv: {file} -> {outdir}");

                // bible database
                ExtractBibleCsvFile<T>(file, outdir);
                
                // niv-csv - this creates niv_fixed directory - TODO: rewrite the parser
                //ExtractBibleCsvFile<T>(file, outdir, delimiter: ",", false, escape: '\\');
            }
            //);
        }

        /// <summary>
        /// Extract default book index translation e.g. GEN -> KEJ
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
                if (fileNameWithNoExt == string.Empty)
                    continue;
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

                    var booksJsonStr = books.ToJson();
                    //var listDict = new List<Dictionary<string, object>> { books.Select(b => (b.Abbreviation, b.Name)).ToDictionary(k => k.Abbreviation, v => (object)v.Name) };
                    ////var listDict = new List<Dictionary<string, object>> { books.Select(b => (b.Abbreviation, b.Name)).ToDictionary(b => b.Abbreviation) };
                    //var bestComp = JSONH.best(listDict);
                    //var compressedJSON = JSONC.compress(json);
                    //var packed = JSONH.pack(listDict);
                    var targetFileNoExt = $"{Path.GetFileNameWithoutExtension(file).ToLower().Replace("books_", "")}";

                    var targetLzFile = $"{outdir}/{targetFileNoExt}.txt";
                    CreateLzwFile(overrideFile: false, targetFile: targetLzFile, jsonStr: booksJsonStr);

                    var targetBzFile = $"{outdir}/{targetFileNoExt}.bz2";
                    CreateBZip2File(overrideFile: false, targetFile: targetBzFile, jsonStr: booksJsonStr);
                }
            }
        }

        /// <summary>
        /// Create LzwFile from given json
        /// </summary>
        /// <param name="targetFile"></param>
        /// <param name="jsonStr"></param>
        /// <param name="overrideFile"></param>
        private static void CreateJsonFile(string targetFile, string jsonStr, bool overrideFile = false)
        {
            if (overrideFile || !File.Exists(targetFile))
            {
                Console.WriteLine($"Creating: {targetFile}");
                FileHelper.CreateFile(targetFile, jsonStr);
            }
            else
            {
                Console.WriteLine($"File.Exists.Skipping: {targetFile}");
            }
        }

        /// <summary>
        /// Create LzwFile from given json
        /// </summary>
        /// <param name="targetFile"></param>
        /// <param name="jsonStr"></param>
        /// <param name="overrideFile"></param>
        private static void CreateLzwFile(string targetFile, string jsonStr, bool overrideFile = false)
        {
            if (overrideFile || !File.Exists(targetFile))
            {
                Console.WriteLine($"Creating: {targetFile}");
                var lzwString = LZString.compressToBase64(jsonStr);
                FileHelper.CreateFile(targetFile, lzwString);
            }
            else
            {
                Console.WriteLine($"File.Exists.Skipping: {targetFile}");
            }
        }

        /// <summary>
        /// Create BZip2 File from given JSon
        /// </summary>
        /// <param name="targetFile"></param>
        /// <param name="jsonStr"></param>
        /// <param name="overrideFile"></param>
        private static void CreateBZip2File(string targetFile, string jsonStr, bool overrideFile = false)
        {
            if (overrideFile || !File.Exists(targetFile))
            {
                Console.WriteLine($"Creating: {targetFile}");

                var uncompressedData = Encoding.UTF8.GetBytes(jsonStr);
                byte[] inputBytes = uncompressedData;

                byte[] targetByteArray;
                using (MemoryStream sourceStream = new MemoryStream(inputBytes))
                {
                    using (MemoryStream targetStream = new MemoryStream())
                    {
                        BZip2.Compress(sourceStream, targetStream, true, 4096);

                        targetByteArray = targetStream.ToArray();
                        var file = File.Create(targetFile);
                        file.Write(targetByteArray, 0, targetByteArray.Length);
                        file.Flush();
                        file.Close();
                    }
                }
            }
            else
            {
                Console.WriteLine($"File.Exists.Skipping: {targetFile}");
            }
        }


        /// <summary>
        /// MAIN EXTRACTION LOGIC - Read and Extract Bible Csv into multiple targets
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        private static void ExtractBibleCsvFile<T>(string source, string target, string delimiter = "\t", bool ignoreQuote = true, bool hasHeaderRecord = false, char escape='"')
        {
            var fileNameWithNoExt = Path.GetFileNameWithoutExtension(source);

            // get bibledatabase.org encoding specific to the file
            int codePage = GetBibleDatabaseOrgEncoding(fileNameWithNoExt);
            var sourceEncode = Encoding.GetEncoding(codePage);

            // csv reader configuration
            var config = new Configuration()
            {
                Delimiter = delimiter,
                HasHeaderRecord = hasHeaderRecord,
                IgnoreQuotes = ignoreQuote,
                BadDataFound = null
                ,Escape = escape
            };

            // Prepare directories
            Directory.CreateDirectory(target);
            string BZ2_OUT_DIR = $"{target}/bz2";
            Directory.CreateDirectory(BZ2_OUT_DIR);

            // read file into csv
            using (var reader = new StreamReader(source, sourceEncode))
            using (var csv = new CsvReader(reader, config))
            {
                // get all records
                var recs = csv.GetRecords<BibleRowCsv>().ToArray();

                // compress entire records as bz2
                var allRecs = recs.OrderBy(r => r.Book).ThenBy(r => r.Chapter).ThenBy(r => r.Verse)
                                .Select(r => (r.Book, r.Chapter, r.Verse, Text: r.Text.Sanitize())).Where(r => r.Text.Length > 0);
                var recsJson = allRecs.ToJson();
                var targetFile = $"{target}/{fileNameWithNoExt}.bz2";
                CreateBZip2File(overrideFile: false, targetFile: targetFile, jsonStr: recsJson);

                // compress bible per book as bz2 - averaging 30-50Kb per Bible Book e.g. Gen, EXO etc
                Parallel.ForEach(recs.GroupBy(b => b.Book).Select(g => g.FirstOrDefault().Book), book =>
                {
                    var book_verses = recs.Where(b => b.Book == book).GroupBy(c => c.Chapter).OrderBy(g => g.Key).Select(g => g.Select(v => v.Text.Sanitize()).ToArray());
                    var targetBzFile = $"{BZ2_OUT_DIR }/{book}.bz2";
                    if (book_verses.Any(bv => bv.Any(v => v.Length > 0)))
                    {
                        CreateBZip2File(overrideFile: false, targetFile: targetBzFile, jsonStr: book_verses.ToJson());
                    }
                    else
                    {
                        DeleteIfFileExists(targetBzFile);
                    }
                });

                // Lzw specific extraction - BZip2 is better per book, lzw is better to compress chapter per chapter up 8kb, avg 3-5kb per bible_chapter
                Parallel.ForEach(recs.GroupBy(b => b.Book).Select(g => g.FirstOrDefault().Book), book =>
                {
                    Parallel.ForEach(recs.Where(b => b.Book == book).GroupBy(v => v.Chapter).Select(c => c.First().Chapter), chapter =>
                    {
                        var verses = recs.Where(b => b.Book == book && b.Chapter == chapter).OrderBy(r => r.Verse).Select(r => r.Text.Sanitize());
                        var first = verses.First();

                        var versesJsonStr = verses.ToJson();
                        //var versesLzw = LZString.compressToBase64(versesJsonStr);

                        var targetLzFile = $"{target}/{book}_{chapter}.txt";
                        var targetJsonFile = $"{target}/{book}_{chapter}.json";
                        //var targetBzFile = $"{BZ2_OUT_DIR}/{book}_{chapter}.bz2";

                        if (verses.Any(v => v.Length > 0))
                        {
                            CreateLzwFile(overrideFile: true, targetFile: targetLzFile, jsonStr: versesJsonStr);

                            // BZip2 is better for large dataset, best compression is per book, the total File Size is almost the same compared to entire bible in BZip2 format
                            // Chapter per chapter will resulting 1.5x bigger compared to entire bible compressed. LZW in total is 2x BZip2 FileSize
                            //CreateBZip2File(overrideFile: true, targetFile: targetBzFile, jsonStr: versesJsonStr);
                        }
                        else
                        {
                            DeleteIfFileExists(targetLzFile);
                        }
                    });
                });
            }
        }

        /// <summary>
        /// Delete File when Exists
        /// </summary>
        /// <param name="targetFile"></param>
        /// <param name="reason"></param>
        private static void DeleteIfFileExists(string targetFile, string reason = "Empty.Verse")
        {
            if (File.Exists(targetFile))
            {
                Console.WriteLine($"Deleting: {targetFile}, reason: {reason}");
                File.Delete(targetFile);
            }
            else
            {
                Console.WriteLine($"Skip.Deletion|File.Not.Found: {targetFile}");
            }
        }

        /// <summary>
        /// Get Encoding stored for bibledatabase.org files
        /// </summary>
        /// <param name="fileNameWithNoExt"></param>
        /// <returns></returns>
        private static int GetBibleDatabaseOrgEncoding(string fileNameWithNoExt)
        {
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
                    return 65001;
                case "b_thai":
                    return 874;
                case "b_bulgarian":
                case "b_ukrainian":
                    return 1251;
                case "b_hebrew_ot_h_":
                    return 1255;
                case "b_chinese_union_b5":
                    return 950;
                case "b_chinese_union_gb":
                    return 936;
                case "b_russian_koi8r":
                    return 20866;
                case "b_korean":
                    return 949;
                default:
                    return 0;
            }
        }
    }
}


