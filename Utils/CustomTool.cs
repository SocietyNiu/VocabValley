using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VocabValley.Utils
{
    static class CustomTool
    {
        public static int Key2Int(Keys key)
        {
            return key switch
            {
                Keys.D1 or Keys.NumPad1 => 0,
                Keys.D2 or Keys.NumPad2 => 1,
                Keys.D3 or Keys.NumPad3 => 2,
                Keys.D4 or Keys.NumPad4 => 3,
                Keys.Enter or Keys.Space => 88,
                _ => -1
            };
        }
    }
}
