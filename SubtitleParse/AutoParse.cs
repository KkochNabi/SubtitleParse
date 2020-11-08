using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace SubtitleParse
{
    public class AutoParse : SubsParse

    {
        public AutoParse()
        {
            this.Locations = new List<string>();
            this.LocationFormat = new List<Format>();
            this.Name = new List<string>();
            this.Start = new List<string>();
            this.End = new List<string>();
            this.Style = new List<string>();
            this.Line = new List<string>();
        }

        public List<string> Locations { get; }
        private List<Format> LocationFormat { get; }
        public List<string> Name { get; }
        public List<string> Start { get; }
        public List<string> End { get; }
        public List<string> Style { get; }
        public List<string> Line { get; }

        /// <summary>
        /// Checks the file format of a path
        /// </summary>
        private Format CheckFormat(string path)
        {
            switch (File.ReadLines(path).First())
            {
                case "1":
                    return SubsParse.Format.Srt;
                case "[Script Info]":
                    return SubsParse.Format.Ass;
                default: // Unknown filetype
                    return SubsParse.Format.Unknown;
            }
        }

        /// <summary>
        /// Adds a single path to the class list of paths
        /// </summary>
        /// <remarks>Does not add the path if its' filetype if unknown</remarks>
        public override void AddLocation(string path)
        {
            if (CheckFormat(path) == Format.Unknown) return;
            Locations.Add(path);
            LocationFormat.Add(CheckFormat(path));
        }

        /// <summary>
        /// Adds any i-enumerable of strings datatype containing paths to the class list of paths
        /// </summary>
        /// <remarks>Does not add the path if its' filetype if unknown</remarks>
        public override void AddLocation(IList<string> paths)
        {
            foreach (var path in paths)
            {
                AddLocation(path);
            }
        }

        /// <summary>
        /// Copies over the name, start, end, style, and line from the manual parsers
        /// </summary>
        private void CopyFromParser(List<string> names, List<string> starts, List<string> ends, List<string> styles,
            List<string> lines)
        {
            Name.AddRange(names);
            Start.AddRange(starts);
            End.AddRange(ends);
            Style.AddRange(styles);
            Line.AddRange(lines);
        }

        public override void ParsePaths()
        {
            foreach (var path in Locations)
            {
                switch (LocationFormat[Locations.IndexOf(path)])
                {
                    case Format.Ass:
                        var ass = new AssParse();
                        ass.ParsePath(path);
                        CopyFromParser(ass.Name, ass.Start, ass.End, ass.Style, ass.Line);
                        break;
                    case Format.Srt:
                        var srt = new SrtParse();
                        srt.ParsePath(path);
                        var noStyle = Enumerable.Repeat(string.Empty, srt.Name.Count).ToList(); // srt has no style
                        CopyFromParser(srt.Name, srt.Start, srt.End, noStyle, srt.Line);
                        break;
                    case Format.Unknown: // Should not happen as adding a location would not allow unknown filetypes
                        Locations.Remove(path);
                        break;
                    default: // Should never reach unless there is an added possible value in enum Format
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        internal override void ParsePath(string path)
        {
            throw new NotImplementedException();
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
                    fs.Write(Name[i] + separator + Start[i] + separator + End[i] + separator + Style[i] + separator +
                             Line[i] + "\n");
                }
            }
        }
    }
}