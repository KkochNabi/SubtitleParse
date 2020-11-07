using System.Collections.Generic;

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

        private List<string> Locations { get; }
        private List<string> Name { get; }
        private List<string> Start { get; }
        private List<string> End { get; }
        private List<string> Style { get; }
        private List<string> Line { get; }
        
        ///<summary>
        ///Adds a single path to the class list of paths
        ///</summary>
        public override void AddLocation(string path)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Adds any i-enumerable of strings datatype containing paths to the class list of paths
        /// </summary>
        public override void AddLocation(IEnumerable<string> paths)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Start parsing the paths of the files present in Locations and add appropriate parsed information in Name, Start, End, and Line.
        /// </summary>
        public override void ParsePaths()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Exports all parsed lines into a text file with a format of your choosing
        /// </summary>
        /// <param name="path">The path for the exported file</param>
        /// <param name="separator">The character separating the exported fields (default = tab)</param>
        public override void Export(string path, string separator)
        {
            throw new System.NotImplementedException();
        }
    }
}