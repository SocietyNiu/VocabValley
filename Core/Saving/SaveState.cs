using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocabValley.Core.Model;
using VocabValley.Core.Setting;

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
        public string fileName { get; set; } = "CET4.txt";

        public SettingState SettingState { get; set; } = new SettingState();
    }
    public record OtherState
    {
        public int Points { get; set; } = new int();
        
        public StatisticsState StatisticsState { get; set; } = new StatisticsState();
    }
}
