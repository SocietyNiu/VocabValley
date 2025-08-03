using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VocabValley.Core.Saving
{
    public record SaveState
    {
        public bool IsLearned { get; set; }
        public int ReviewCount { get; set; }
        public int WrongCount { get; set; }
    }
    public record Config
    {
        public string fileName { get; set; }
    }
    public record OtherState
    {
        public int Points { get; set; }
        public bool isWrongWordsPageLocked { get; set; }
        public bool isCellarLocked { get; set; }
    }
}
