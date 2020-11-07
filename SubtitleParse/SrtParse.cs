using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;

namespace SubtitleParse
{
    public class SrtParse : SubsParse
    {
        private Format Format { get; } = Format.Srt;

        public SrtParse()
        {
            this.Locations = new List<string>();
            this.Name = new List<string>();
            this.Start = new List<string>();
            this.End = new List<string>();
            this.Line = new List<string>();
            
        }

        public List<string> Locations { get; }
        public List<string> Name { get; }
        public List<string> Start { get; }
        public List<string> End { get; }
        public List<string> Line { get; }
        
        ///<summary>
        ///Adds a single path to the class list of paths
        ///</summary>
        public override void AddLocation(string path)
        {
            Locations.Add(path);
        }

        /// <summary>
        /// Adds any i-enumerable of strings datatype containing paths to the class list of paths
        /// </summary>
        public override void AddLocation(IEnumerable<string> paths)
        {
            foreach (var path in paths)
            {
                Locations.Add(path);
            }
        }
        
        
        /// <summary>
        /// Stages of parsing a srt file
        /// </summary>
        private enum SrtStage
        {
            Counter = 0,
            Timings = 1,
            Line = 2,
        }
        
        /// <summary>
        /// Start parsing the paths of the files present in Locations and add appropriate parsed information in Name, Start, End, and Line.
        /// </summary>
        public override void ParsePaths() //TODO: Implement custom name support
        {
            foreach (var path in Locations)
            {
                var currentStage = SrtStage.Counter;
                var currentLine = "";
                var currentSubLine = new StringBuilder();
                var firstLineDone = false;
                
                using (var fs = File.OpenText(path))
                {
                    while ((currentLine = fs.ReadLine()) != null)
                    {
                        switch (currentStage)
                        {
                            case SrtStage.Counter: // Counters are unnecessary, we can just use indexing in the list
                                //currentSubLine++; // If reactivating this line, change currentSubLine to be int
                                currentStage = SrtStage.Timings;
                                break;
                            case SrtStage.Timings:
                                Start.Add(Convert.ToString(Regex.Match(currentLine, @"(.+) --> (.+\d)").Groups[1])
                                    .Replace(",", "."));
                                End.Add(Convert.ToString(Regex.Match(currentLine, @"(.+) --> (.+\d)").Groups[2])
                                    .Replace(",", "."));
                                currentStage = SrtStage.Line;
                                break;
                            case SrtStage.Line:
                                if (currentLine == "") // When we surpassed all the lines in the subtitle
                                {
                                    //currentStage = SrtStage.Spacing;
                                    Line.Add(currentSubLine.ToString());
                                    currentSubLine.Clear();
                                    Name.Add(Path.GetFileNameWithoutExtension(path));
                                    firstLineDone = false;
                                    currentStage = SrtStage.Counter;
                                    break;
                                }

                                currentSubLine.Append(firstLineDone ? "<br>" + currentLine : currentLine); // <br> is used for newline because Anki uses html tags
                                firstLineDone = true;
                                break;
                            default: // Should never happen, there are no other possible values in SrtStage enum
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }
            
        }

        /// <summary>
        /// Exports all parsed lines into a text file with a format of your choosing
        /// </summary>
        /// <param name="path">The path for the exported file</param>
        /// <param name="separator">The character separating the exported fields (default = tab)</param>
        public override void Export(string path, string separator) //TODO: Export different files based on show/searchgroup
        {
            separator = separator == "" ? "\t" : separator;
            using (var fs = new StreamWriter(path))
            {
                for (var i = 0; i < Name.Count; i++) // All lists from parsing subtitles should have the same length
                {
                    fs.Write(Name[i] + separator + Start[i] + separator + End[i] + separator + Line[i] + "\n");
                }
            }
        }
        
    }
}