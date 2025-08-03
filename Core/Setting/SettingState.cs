using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VocabValley.Core.Setting
{
    public record SettingState
    {
        public bool isPause = false;
        public bool isWrongWordsPageLocked = true;
        public bool isCellarLocked = true;
    }
}
