using System;
using System.IO;
using SubtitleParse;

namespace Tests
{
    internal class Program
    {
        public static void Main(string[] args) // Tests for the library because I'm too lazy to make unit tests
        {
            var srtBatchMonolingual =
                Directory.GetFiles(@"C:\Users\User\Desktop\Example Subtitles\Monolingual .srt", @"*.srt");
            var assBatchMonolingual =
                Directory.GetFiles(@"C:\Users\User\Desktop\Example Subtitles\Monolingual .ass\", @"*.ass");
            var assBatchBilingual =
                Directory.GetFiles(@"C:\Users\User\Desktop\Example Subtitles\Bilingual .ass\", "*.ass");
            var srtSingleMonolingual = srtBatchMonolingual[0];
            var assSingleMonolingual = assBatchMonolingual[0];
            var assSingleBilingual = assBatchBilingual[0];
            
            var insert = new SrtParse();
            insert.AddLocation(srtBatchMonolingual);
            
            Console.WriteLine("First file: " + insert.Locations[0] + "\nPaths: " + insert.Locations.Count);
            
            insert.ParsePaths();
            Console.WriteLine("Lengths of fields are:\nName: " + insert.Name.Count + "\nStart: " + insert.Start.Count +
                              "\nEnd: " + insert.End.Count + "\nLine: " + insert.Line.Count);
            File.Delete(@"C:\Users\User\Desktop\Example Subtitles\output.txt");
            insert.Export(@"C:\Users\User\Desktop\Example Subtitles\output.txt", "\t");
        }
    }
}