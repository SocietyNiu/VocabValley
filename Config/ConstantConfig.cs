using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VocabValley.Config
{
    internal static class ConstantConfig
    {
        public static readonly HashSet<string> TowerMaps = new()
        {
            "BabelTower",
            "BabelTowerNormalLevel",
            "BabelTowerStairLevel",
            "BabelTowerRewardLevel",
            "BabelTowerBossLevel",
            "BabelTowerCellar"
        };
    }
}
