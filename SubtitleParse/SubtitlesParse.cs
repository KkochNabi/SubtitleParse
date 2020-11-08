using System.CodeDom;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection.Emit;
using System.Security.Cryptography;

namespace SubtitleParse
{
    public abstract class SubsParse
    {
        protected SubsParse()
        {
            Locations = new List<string>();
            Name = new List<string>();
            Start = new List<string>();
            End = new List<string>();
            Style = new List<string>();
            Line = new List<string>();
        }


        protected enum Format
        {
            Ass,
            Srt,
            Unknown
        }
        
        private List<string> Locations { get; }
        private List<string> Name { get;  }
        private List<string> Start { get; }
        private List<string> End { get;  }
        private List<string> Style { get;  }
        private List<string> Line { get;  }
        //TODO: Option to ignore certain types of lines 

        public abstract void AddLocation(string path);
        public abstract void AddLocation(IList<string> paths);
        public abstract void ParsePaths();
        internal abstract void ParsePath(string path);
        public abstract void Export(string path, string separator);
    }
}