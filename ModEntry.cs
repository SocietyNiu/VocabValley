using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using xTile.Tiles;
using SObject = StardewValley.Object;
using StardewValley.GameData.Objects;
using VocabValley.UI;
using VocabValley.Utils;
using Microsoft.Xna.Framework;
using VocabValley.Core.Statistics;
using VocabValley.Core.Setting;
using VocabValley.Core.Saving;
using VocabValley.Core.Reward;
using VocabValley.Core.Points;
using VocabValley.Core.Model;
using VocabValley.Core.Level;
using VocabValley.Core;

namespace VocabValley
{
    public class ModEntry : Mod
    {
        private CoreManager coreManager = null;

        public override void Entry(IModHelper helper)
        {
            coreManager = new CoreManager(helper, Monitor);
        }
    }
}
