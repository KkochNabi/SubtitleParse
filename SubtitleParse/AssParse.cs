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
        
        
    }
}