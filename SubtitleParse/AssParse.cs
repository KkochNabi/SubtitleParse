using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;

namespace SubtitleParse
{
    public class AssParse : SubsParse
    {
        private Format Format { get; } = Format.Ass;
        
        public AssParse()
        {
            this.Locations = new List<string>();
            this.Name = new List<string>();
            this.Start = new List<string>();
            this.End = new List<string>();
            this.Style = new List<string>();
            this.Line = new List<string>();
        }

        public List<string> Locations { get; }
        public List<string> Name { get; }
        public List<string> Start { get; }
        public List<string> End { get; }
        public List<string> Style { get; }
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


        private enum AssStage
        {
            PreRead = 0,
            Format = 1,
            Line = 2
        }
        
        /// <summary>
        /// Start parsing the paths of the files present in Locations and add appropriate parsed information in Name, Start, End, and Line.
        /// </summary>
        public override void ParsePaths()
        {
            foreach (var path in Locations)
            {
                var currentStage = AssStage.PreRead;
                var currentLineSplit = new List<string>();
                var formatting = new List<string>();
                var sb = new StringBuilder();
                var startIndex = new int();
                var endIndex = new int();
                var styleIndex = new int();
                //var effectIndex = new int(); // TODO: Implement skipping of karaoke effects, etc.
                var lineIndex = new int();
                
                using (var fs = File.OpenText(path))
                {
                    var currentLine = "";
                    while ((currentLine = fs.ReadLine()) != null)
                    {
                        switch (currentStage)
                        {
                            case AssStage.PreRead:
                                if (currentLine.StartsWith("[Events]"))
                                {
                                    currentStage = AssStage.Format;
                                }

                                break;
                            case AssStage.Format:
                                if (currentLine.StartsWith("Format:")) 
                                { // Pre-dialogue/subtitle format template parsing
                                    currentLine = Convert.ToString(Regex.Match(currentLine, "Format: (.*)").Groups[1]); 
                                    formatting = currentLine.Split(new string[1] { ", "}, StringSplitOptions.None).ToList();
                                    
                                    startIndex = formatting.IndexOf("Start"); 
                                    endIndex = formatting.IndexOf("End"); 
                                    styleIndex = formatting.IndexOf("Style"); 
                                    lineIndex = formatting.IndexOf("Text"); 
                                    currentStage = AssStage.Line; 
                                }
                                
                                break;
                            case AssStage.Line:
                                if (currentLine.StartsWith("Dialogue:"))
                                {
                                    currentLine = Regex.Replace(currentLine, "({.*?})", ""); // Regex before splitting because ex: {\fad(0,350)}
                                    currentLineSplit = currentLine.Split(',').ToList();
                                    Name.Add(Path.GetFileNameWithoutExtension(path));
                                    Start.Add("0" + currentLineSplit[startIndex] + "0");
                                    End.Add("0" + currentLineSplit[endIndex] + "0");
                                    Style.Add(currentLineSplit[styleIndex]);
                                    
                                    sb.Append(currentLineSplit[lineIndex]);
                                    
                                    sb.Replace(@"\n", "<br>"); // Linebreaks and hard spaces removal, the only thing not in brackets
                                    sb.Replace(@"\N", "<br>");
                                    sb.Replace(@"\h", " ");
                                    
                                    Line.Add(sb.ToString());
                                    sb.Clear();
                                }
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
        public override void Export(string path, string separator)
        {
            separator = separator == "" ? "\t" : separator;
            using (var fs = new StreamWriter(path))
            {
                for (var i = 0; i < Name.Count; i++) // All lists from parsing subtitles should have the same length
                {
                    fs.Write(Name[i] + separator + Start[i] + separator + End[i] + separator + Style[i] + separator + Line[i] + "\n");
                }
            }
        }
    }
}